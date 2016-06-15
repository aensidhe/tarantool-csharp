﻿using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace tarantool_client
{
    public class RequestWriter : IRequestWriter
    {
        private readonly Stream _networkStream;

        private readonly ILog _log;

        private readonly ConcurrentDictionary<ulong, TaskCompletionSource<byte[]>> _pendingRequests = new ConcurrentDictionary<ulong, TaskCompletionSource<byte[]>>();

        public RequestWriter(Stream stream, ILog log)
        {
            _networkStream = stream;
            _log = log;
        }

        public void EndRequest(ulong requestId, byte[] result)
        {
            TaskCompletionSource<byte[]> pendingRequest;
            if (_pendingRequests.TryGetValue(requestId, out pendingRequest))
            {
                _log.Trace(
                    pendingRequest.TrySetResult(result)
                        ? $"Successfully completed request with id:{requestId}"
                        : $"Can't complete request with id:{requestId}");
            }
            else
            {
                _log.Trace($"Can't find matching request for response with id: {requestId}.");
            }
        }

        public async Task<byte[]> WriteRequest(byte[] request, ulong requestId)
        {
            var requestTask = new TaskCompletionSource<byte[]>();

            if (_pendingRequests.TryAdd(requestId, requestTask))
            {
                await _networkStream.WriteAsync(request, 0, request.Length);
            }
            else
            {
                _log.Trace($"Request with such id ({requestId}) is already sent!");
                requestTask.SetResult(null);
            }

            return await requestTask.Task;
        }
    }
}