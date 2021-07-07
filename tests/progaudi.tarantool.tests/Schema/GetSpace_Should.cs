using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

using Shouldly;

namespace ProGaudi.Tarantool.Client.Tests.Schema
{
    public class GetSpace_Should : TestBase
    {
        [Fact]
        public async Task throw_expection_for_non_existing_space_by_name()
        {
            using (var tarantoolClient = await Client.Box.Connect(await ConnectionStringFactory.GetReplicationSource_1_7()))
            {
                var schema = tarantoolClient.GetSchema();

                Should.Throw<ArgumentException>(() =>
                {
                    var _ = schema["non-existing"];
                });
            }
        }

        [Fact]
        public async Task throw_expection_for_non_existing_space_by_id()
        {
            using (var tarantoolClient = await Client.Box.Connect(await ConnectionStringFactory.GetReplicationSource_1_7()))
            {
                var schema = tarantoolClient.GetSchema();

                Should.Throw<ArgumentException>(() =>
                {
                    var _ = schema[12341234];
                });
            }
        }

        [Fact]
        public async Task returns_space_by_id()
        {
            const uint VSpaceId = 0x119; // that space always exist and contains other spaces.
            using (var tarantoolClient = await Client.Box.Connect(await ConnectionStringFactory.GetReplicationSource_1_7()))
            {
                var schema = tarantoolClient.GetSchema();

                var space = schema[VSpaceId];

                space.Id.ShouldBe(VSpaceId);
            }
        }

        [Fact]
        public async Task returns_space_by_name()
        {
            const string VSpaceName = "_vspace"; // that space always exist and contains other spaces.
            using (var tarantoolClient = await Client.Box.Connect(await ConnectionStringFactory.GetReplicationSource_1_7()))
            {
                var schema = tarantoolClient.GetSchema();

                var space = schema[VSpaceName];

                space.Name.ShouldBe(VSpaceName);
            }
        }

        [Fact]
        public async Task read_multiple_spaces_in_a_row()
        {
            const string VSpaceName = "_vspace"; // that space always exist and contains other spaces.
            using (var tarantoolClient = await Client.Box.Connect(await ConnectionStringFactory.GetReplicationSource_1_7()))
            {
                var schema = tarantoolClient.GetSchema();

                for (int i = 0; i < 10; i++)
                {
                    var space = schema[VSpaceName];

                    space.Name.ShouldBe(VSpaceName);
                }
            }
        }

        [Fact]
        public async Task iterating_over_schema()
        {
            using (var tarantoolClient = await Client.Box.Connect(ConnectionStringFactory.GetReplicationSource_1_7()))
            {
                foreach (var pair in (IReadOnlyDictionary<string, ISpace>)tarantoolClient.Schema)
                {
                    pair.Value.Name.ShouldBe(pair.Key);
                }
            }
        }
    }
}