﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ProGaudi.MsgPack.Light;

using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Headers;
using ProGaudi.Tarantool.Client.Model.Requests;
using ProGaudi.Tarantool.Client.Model.Responses;
using ProGaudi.Tarantool.Client.Utils;

namespace ProGaudi.Tarantool.Client
{
    internal class LogicalConnection : ILogicalConnection
    {
        private readonly MsgPackContext _msgPackContext;

        private readonly ClientOptions _clientOptions;

        private readonly RequestIdCounter _requestIdCounter;

        private readonly IPhysicalConnection _physicalConnection;

        private readonly IResponseReader _responseReader;

        private readonly IRequestWriter _requestWriter;

        private readonly ILog _logWriter;

        private bool _disposed;

        public LogicalConnection(ClientOptions options, RequestIdCounter requestIdCounter)
        {
            _clientOptions = options;
            _requestIdCounter = requestIdCounter;
            _msgPackContext = options.MsgPackContext;
            _logWriter = options.LogWriter;

            _physicalConnection = new NetworkStreamPhysicalConnection();
            _responseReader = new ResponseReader(_clientOptions, _physicalConnection);
            _requestWriter = new RequestWriter(_clientOptions, _physicalConnection);
        }

        public uint PingsFailedByTimeoutCount
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _responseReader.Dispose();
            _requestWriter.Dispose();
            _physicalConnection.Dispose();
        }

        public async Task Connect()
        {
            await _physicalConnection.Connect(_clientOptions).ConfigureAwait(false);

            var greetingsResponseBytes = new byte[128];
            var readCount = await _physicalConnection.ReadAsync(greetingsResponseBytes, 0, greetingsResponseBytes.Length).ConfigureAwait(false);
            if (readCount != greetingsResponseBytes.Length)
            {
                throw ExceptionHelper.UnexpectedGreetingBytesCount(readCount);
            }

            var greetings = new GreetingsResponse(greetingsResponseBytes);

            _clientOptions.LogWriter?.WriteLine($"Greetings received, salt is {Convert.ToBase64String(greetings.Salt)} .");

            PingsFailedByTimeoutCount = 0;

            _responseReader.BeginReading();
            _requestWriter.BeginWriting();

            _clientOptions.LogWriter?.WriteLine("Server responses reading started.");

            await LoginIfNotGuest(greetings).ConfigureAwait(false);
        }

        public bool IsConnected()
        {
            if (_disposed)
            {
                return false;
            }

            return _responseReader.IsConnected && _requestWriter.IsConnected && _physicalConnection.IsConnected;
        }

        public async Task SendRequestWithEmptyResponse<TRequest>(TRequest request, TimeSpan? timeout = null)
            where TRequest : IRequest
        {
            await SendRequestImpl(request, timeout).ConfigureAwait(false);
        }

        public async Task<DataResponse<TResponse[]>> SendRequest<TRequest, TResponse>(TRequest request, TimeSpan? timeout = null)
            where TRequest : IRequest
        {
            var stream = await SendRequestImpl(request, timeout).ConfigureAwait(false);
            return MsgPackSerializer.Deserialize<DataResponse<TResponse[]>>(stream, _msgPackContext);
        }

        public async Task<DataResponse> SendRequest<TRequest>(TRequest request, TimeSpan? timeout = null)
            where TRequest : IRequest
        {
            var stream = await SendRequestImpl(request, timeout).ConfigureAwait(false);
            return MsgPackSerializer.Deserialize<DataResponse>(stream, _msgPackContext);
        }

        public async Task<byte[]> SendRawRequest<TRequest>(TRequest request, TimeSpan? timeout = null)
            where TRequest : IRequest
        {
            return (await SendRequestImpl(request, timeout).ConfigureAwait(false)).ToArray();
        }

        private async Task LoginIfNotGuest(GreetingsResponse greetings)
        {
            if (! _clientOptions.ConnectionOptions.Nodes.Any()) 
                throw new ClientSetupException("There are no any configured tarantool nodes");

            var singleNode = _clientOptions.ConnectionOptions.Nodes.Single();

            if (string.IsNullOrEmpty(singleNode.Uri.UserName))
            {
                _clientOptions.LogWriter?.WriteLine("Guest mode, no authentication attempt.");
                return;
            }

            var authenticateRequest = AuthenticationRequest.Create(greetings, singleNode.Uri);

            await SendRequestWithEmptyResponse(authenticateRequest).ConfigureAwait(false);
            _clientOptions.LogWriter?.WriteLine($"Authentication request send: {authenticateRequest}");
        }

        private async Task<MemoryStream> SendRequestImpl<TRequest>(TRequest request, TimeSpan? timeout)
            where TRequest : IRequest
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(LogicalConnection));
            }

            var bodyBuffer = MsgPackSerializer.Serialize(request, _msgPackContext);

            var requestId = _requestIdCounter.GetRequestId();
            var responseTask = _responseReader.GetResponseTask(requestId);

            var headerBuffer = CreateAndSerializeHeader(request, requestId, bodyBuffer);
            _requestWriter.Write(
                headerBuffer,
                new ArraySegment<byte>(bodyBuffer, 0, bodyBuffer.Length));

            try
            {
                if (timeout.HasValue)
                {
                    var cts = new CancellationTokenSource(timeout.Value);
                    responseTask = responseTask.WithCancellation(cts.Token);
                }

                var responseStream = await responseTask.ConfigureAwait(false);
                _logWriter?.WriteLine($"Response with requestId {requestId} is recieved, length: {responseStream.Length}.");

                return responseStream;
            }
            catch (ArgumentException)
            {
                _logWriter?.WriteLine($"Response with requestId {requestId} failed, header:\n{headerBuffer.ToReadableString()} \n body: \n{bodyBuffer.ToReadableString()}");
                throw;
            }
            catch (TimeoutException)
            {
                PingsFailedByTimeoutCount++;
                throw;
            }
        }

        private ArraySegment<byte> CreateAndSerializeHeader<TRequest>(
            TRequest request,
            RequestId requestId,
            byte[] serializedRequest) where TRequest : IRequest
        {
            var packetSizeBuffer = new byte[Constants.PacketSizeBufferSize + Constants.MaxHeaderLength];
            var stream = new MemoryStream(packetSizeBuffer);

            var requestHeader = new RequestHeader(request.Code, requestId);
            stream.Seek(Constants.PacketSizeBufferSize, SeekOrigin.Begin);
            MsgPackSerializer.Serialize(requestHeader, stream, _msgPackContext);

            var lengthAndHeaderLengthByteCount = (int)stream.Position;
            var headerLength = lengthAndHeaderLengthByteCount - Constants.PacketSizeBufferSize;
            var packetLength = new PacketSize((uint) (headerLength + serializedRequest.Length));
            stream.Seek(0, SeekOrigin.Begin);
            MsgPackSerializer.Serialize(packetLength, stream, _msgPackContext);
            return new ArraySegment<byte>(packetSizeBuffer, 0, lengthAndHeaderLengthByteCount);
        }
    }
}