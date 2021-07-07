using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProGaudi.Tarantool.Client
{
    public interface ISchema : IReadOnlyDictionary<string, ISpace>, IReadOnlyDictionary<uint, ISpace>
    {
        [Obsolete("Use indexer")]
        Task<ISpace> GetSpace(string name);
        [Obsolete("Use indexer")]
        Task<ISpace> GetSpace(uint id);

        Task Reload();

        DateTimeOffset LastReloadTime { get; }

        IReadOnlyCollection<ISpace> Spaces { get; }
    }
}