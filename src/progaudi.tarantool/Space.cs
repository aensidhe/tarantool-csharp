using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Enums;
using ProGaudi.Tarantool.Client.Model.Requests;
using ProGaudi.Tarantool.Client.Model.Responses;
using ProGaudi.Tarantool.Client.Model.UpdateOperations;
using ProGaudi.Tarantool.Client.Utils;

namespace ProGaudi.Tarantool.Client
{
    public class Space : ISpace
    {
        private Dictionary<string, IIndex> _indexByName = new Dictionary<string, IIndex>();
        private Dictionary<uint, IIndex> _indexById = new Dictionary<uint, IIndex>();

        public ILogicalConnection LogicalConnection { get; set; }

        public Space(uint id, uint fieldCount, string name, StorageEngine engine, IReadOnlyCollection<SpaceField> fields)
        {
            Id = id;
            FieldCount = fieldCount;
            Name = name;
            Engine = engine;
            Fields = fields;
        }

        public uint Id { get; }

        public uint FieldCount { get; }

        public string Name { get; }

        public StorageEngine Engine { get; }

        public IReadOnlyCollection<IIndex> Indices => _indexByName.Values;

        internal void SetIndices(IReadOnlyCollection<Index> value)
        {
            var byName = new Dictionary<string, IIndex>();
            var byId = new Dictionary<uint, IIndex>();

            if (value != null)
            {
                foreach (var index in value)
                {
                    byName[index.Name] = index;
                    byId[index.Id] = index;
                    index.LogicalConnection = LogicalConnection;
                }
            }

            Interlocked.Exchange(ref _indexByName, byName);
            Interlocked.Exchange(ref _indexById, byId);
        }

        public IReadOnlyCollection<SpaceField> Fields { get; }

        public Task<IIndex> GetIndex(string name) => Task.FromResult(this[name]);

        public Task<IIndex> GetIndex(uint id) => Task.FromResult(this[id]);

        public IIndex this[string name] => _indexByName.TryGetValue(name, out var index)
            ? index
            : throw ExceptionHelper.InvalidIndexName(name, Name);

        public IIndex this[uint id] => _indexById.TryGetValue(id, out var index)
            ? index
            : throw ExceptionHelper.InvalidIndexId(id, Name);

        public async Task<DataResponse<TTuple[]>> Insert<TTuple>(TTuple tuple)
        {
            var insertRequest = new InsertRequest<TTuple>(Id, tuple);
            return await LogicalConnection.SendRequest<InsertReplaceRequest<TTuple>, TTuple>(insertRequest).ConfigureAwait(false);
        }

        public async Task<DataResponse<TTuple[]>> Select<TKey, TTuple>(TKey selectKey)
        {
            var selectRequest = new SelectRequest<TKey>(Id, Schema.PrimaryIndexId, uint.MaxValue, 0, Iterator.Eq, selectKey);
            return await LogicalConnection.SendRequest<SelectRequest<TKey>, TTuple>(selectRequest).ConfigureAwait(false);
        }

        public async Task<TTuple> Get<TKey, TTuple>(TKey key)
        {
            var selectRequest = new SelectRequest<TKey>(Id, Schema.PrimaryIndexId, 1, 0, Iterator.Eq, key);
            var response = await LogicalConnection.SendRequest<SelectRequest<TKey>, TTuple>(selectRequest).ConfigureAwait(false);
            return response.Data.SingleOrDefault();
        }

        public async Task<DataResponse<TTuple[]>> Replace<TTuple>(TTuple tuple)
        {
            var replaceRequest = new ReplaceRequest<TTuple>(Id, tuple);
            return await LogicalConnection.SendRequest<InsertReplaceRequest<TTuple>, TTuple>(replaceRequest).ConfigureAwait(false);
        }

        public async Task<T> Put<T>(T tuple)
        {
            var response = await Replace(tuple).ConfigureAwait(false);
            return response.Data.First();
        }

        public async Task<DataResponse<TTuple[]>> Update<TKey, TTuple>(TKey key, UpdateOperation[] updateOperations)
        {
            var updateRequest = new UpdateRequest<TKey>(Id, Schema.PrimaryIndexId, key, updateOperations);
            return await LogicalConnection.SendRequest<UpdateRequest<TKey>, TTuple>(updateRequest).ConfigureAwait(false);
        }

        public async Task Upsert<TTuple>(TTuple tuple, UpdateOperation[] updateOperations)
        {
            var upsertRequest = new UpsertRequest<TTuple>(Id, tuple, updateOperations);
            await LogicalConnection.SendRequestWithEmptyResponse(upsertRequest).ConfigureAwait(false);
        }

        public async Task<DataResponse<TTuple[]>> Delete<TKey, TTuple>(TKey key)
        {
            var deleteRequest = new DeleteRequest<TKey>(Id, Schema.PrimaryIndexId, key);
            return await LogicalConnection.SendRequest<DeleteRequest<TKey>, TTuple>(deleteRequest).ConfigureAwait(false);
        }

        public Task<DataResponse<TTuple[]>> Increment<TTuple, TKey>(TKey key)
        {
            // Currently we can't impelment that method because Upsert returns void.
           throw new NotImplementedException();
        }

        public Task<DataResponse<TTuple[]>> Decrement<TTuple, TKey>(TKey key)
        {
            // Currently we can't impelment that method because Upsert returns void.
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Name}, id={Id}";
        }

        IEnumerator<KeyValuePair<uint, IIndex>> IEnumerable<KeyValuePair<uint, IIndex>>.GetEnumerator() => _indexById.GetEnumerator();

        IEnumerator<KeyValuePair<string, IIndex>> IEnumerable<KeyValuePair<string, IIndex>>.GetEnumerator() => _indexByName.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException();

        int IReadOnlyCollection<KeyValuePair<string, IIndex>>.Count => _indexById.Count;

        bool IReadOnlyDictionary<string, IIndex>.ContainsKey(string key) => _indexByName.ContainsKey(key);

        bool IReadOnlyDictionary<string, IIndex>.TryGetValue(string key, out IIndex value) => _indexByName.TryGetValue(key, out value);

        bool IReadOnlyDictionary<uint, IIndex>.ContainsKey(uint key) => _indexById.ContainsKey(key);

        bool IReadOnlyDictionary<uint, IIndex>.TryGetValue(uint key, out IIndex value) => _indexById.TryGetValue(key, out value);

        IEnumerable<uint> IReadOnlyDictionary<uint, IIndex>.Keys => _indexById.Keys;

        IEnumerable<IIndex> IReadOnlyDictionary<uint, IIndex>.Values => _indexById.Values;

        IEnumerable<string> IReadOnlyDictionary<string, IIndex>.Keys => _indexByName.Keys;

        IEnumerable<IIndex> IReadOnlyDictionary<string, IIndex>.Values => _indexByName.Values;

        int IReadOnlyCollection<KeyValuePair<uint, IIndex>>.Count => _indexById.Count;
    }
}