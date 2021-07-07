using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProGaudi.Tarantool.Client.Model.Enums;
using ProGaudi.Tarantool.Client.Model.Requests;
using ProGaudi.Tarantool.Client.Utils;

namespace ProGaudi.Tarantool.Client
{
    public class Schema : ISchema
    {
        private const int VSpace = 0x119;

        private const int VIndex = 0x121;

        internal const uint PrimaryIndexId = 0;

        private readonly ILogicalConnection _logicalConnection;

        private Dictionary<string, ISpace> _indexByName = new Dictionary<string, ISpace>();

        private Dictionary<uint, ISpace> _indexById = new Dictionary<uint, ISpace>();

        public Schema(ILogicalConnection logicalConnection)
        {
            _logicalConnection = logicalConnection;
        }

        public Task<ISpace> GetSpace(string name) => Task.FromResult(this[name]);

        public Task<ISpace> GetSpace(uint id) => Task.FromResult(this[id]);

        public ISpace this[string name] => _indexByName.TryGetValue(name, out var space)
            ? space
            : throw ExceptionHelper.InvalidSpaceName(name);

        public ISpace this[uint id] => _indexById.TryGetValue(id, out var space)
            ? space
            : throw ExceptionHelper.InvalidSpaceId(id);

        public DateTimeOffset LastReloadTime { get; private set; }

        public async Task Reload()
        {
            var byName = new Dictionary<string, ISpace>();
            var byId = new Dictionary<uint, ISpace>();

            var spaces = await Select<Space>(VSpace).ConfigureAwait(false);
            foreach (var space in spaces)
            {
                byName[space.Name] = space;
                byId[space.Id] = space;
                space.LogicalConnection = _logicalConnection;
                space.SetIndices(await Select<Index>(VIndex, Iterator.Eq, space.Id).ConfigureAwait(false));
            }

            Interlocked.Exchange(ref _indexByName, byName);
            Interlocked.Exchange(ref _indexById, byId);
            LastReloadTime = DateTimeOffset.UtcNow;
        }

        public IReadOnlyCollection<ISpace> Spaces => _indexByName.Values;

        private async Task<T[]> Select<T>(uint spaceId, Iterator iterator = Iterator.All, uint id = 0u)
        {
            var request = new SelectRequest<ValueTuple<uint>>(spaceId, PrimaryIndexId, uint.MaxValue, 0, iterator, ValueTuple.Create(id));

            var response = await _logicalConnection
                .SendRequest<SelectRequest<ValueTuple<uint>>, T>(request)
                .ConfigureAwait(false);
            return response.Data;
        }

        IEnumerator<KeyValuePair<uint, ISpace>> IEnumerable<KeyValuePair<uint, ISpace>>.GetEnumerator() => _indexById.GetEnumerator();

        IEnumerator<KeyValuePair<string, ISpace>> IEnumerable<KeyValuePair<string, ISpace>>.GetEnumerator() => _indexByName.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException();

        int IReadOnlyCollection<KeyValuePair<string, ISpace>>.Count => _indexById.Count;

        bool IReadOnlyDictionary<string, ISpace>.ContainsKey(string key) => _indexByName.ContainsKey(key);

        bool IReadOnlyDictionary<string, ISpace>.TryGetValue(string key, out ISpace value) => _indexByName.TryGetValue(key, out value);

        bool IReadOnlyDictionary<uint, ISpace>.ContainsKey(uint key) => _indexById.ContainsKey(key);

        bool IReadOnlyDictionary<uint, ISpace>.TryGetValue(uint key, out ISpace value) => _indexById.TryGetValue(key, out value);

        IEnumerable<uint> IReadOnlyDictionary<uint, ISpace>.Keys => _indexById.Keys;

        IEnumerable<ISpace> IReadOnlyDictionary<uint, ISpace>.Values => _indexById.Values;

        IEnumerable<string> IReadOnlyDictionary<string, ISpace>.Keys => _indexByName.Keys;

        IEnumerable<ISpace> IReadOnlyDictionary<string, ISpace>.Values => _indexByName.Values;

        int IReadOnlyCollection<KeyValuePair<uint, ISpace>>.Count => _indexById.Count;
    }
}