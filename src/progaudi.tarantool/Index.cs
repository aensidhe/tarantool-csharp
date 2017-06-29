﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Enums;
using ProGaudi.Tarantool.Client.Model.Requests;
using ProGaudi.Tarantool.Client.Model.Responses;
using ProGaudi.Tarantool.Client.Model.UpdateOperations;
using ProGaudi.Tarantool.Client.Utils;

namespace ProGaudi.Tarantool.Client
{
    public class Index : IIndex
    {
        public ILogicalConnection LogicalConnection { get; set; }

        public Index(uint id, uint spaceId, string name, bool unique, IndexType type, IReadOnlyList<IndexPart> parts)
        {
            Id = id;
            SpaceId = spaceId;
            Name = name;
            Unique = unique;
            Type = type;
            Parts = parts;
        }

        public uint Id { get; }

        public uint SpaceId { get; }

        public string Name { get; }

        public bool Unique { get; }

        public IndexType Type { get; }

        public IReadOnlyList<IndexPart> Parts { get; }

        public Task<IEnumerable<TResult>> Pairs<TValue, TResult>(TValue value, Iterator iterator)
            where TResult : ITarantoolTuple
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<TTuple[]>> Select<TKey, TTuple>(TKey key, SelectOptions options = null)
            where TKey : ITarantoolTuple
            where TTuple : ITarantoolTuple
        {
            var selectRequest = new SelectRequest<TKey>(
                SpaceId,
                Id,
                options?.Limit ?? uint.MaxValue,
                options?.Offset ?? 0,
                options?.Iterator ?? Iterator.Eq,
                key);

            return LogicalConnection.SendRequest<SelectRequest<TKey>, TTuple>(selectRequest);
        }

        ///Note: there is no such method in specification http://tarantool.org/doc/book/box/box_index.html.
        ///But common sense, and sources https://github.com/tarantool/tarantool/blob/1.7/src/box/lua/index.c says that that method sould be
        public Task<DataResponse<TTuple[]>> Insert<TTuple>(TTuple tuple)
            where TTuple : ITarantoolTuple
        {
            var insertRequest = new InsertRequest<TTuple>(SpaceId, tuple);

            return LogicalConnection.SendRequest<InsertReplaceRequest<TTuple>, TTuple>(insertRequest);
        }

        ///Note: there is no such method in specification http://tarantool.org/doc/book/box/box_index.html.
        ///But common sense, and sources https://github.com/tarantool/tarantool/blob/1.7/src/box/lua/index.c says that that method sould be
        public Task<DataResponse<TTuple[]>> Replace<TTuple>(TTuple tuple)
            where TTuple : ITarantoolTuple
        {
            var replaceRequest = new ReplaceRequest<TTuple>(SpaceId, tuple);

            return LogicalConnection.SendRequest<InsertReplaceRequest<TTuple>, TTuple>(replaceRequest);
        }

        public Task<TTuple> Min<TTuple>()
           where TTuple : ITarantoolTuple
        {
            return Min<TTuple, TarantoolTuple>(TarantoolTuple.Empty);
        }

        public async Task<TTuple> Min<TTuple, TKey>(TKey key)
            where TTuple : ITarantoolTuple
            where TKey : class, ITarantoolTuple
        {
            if (Type != IndexType.Tree)
            {
                throw ExceptionHelper.WrongIndexType("TREE", "min");
            }
            var iterator = key == null ? Iterator.Eq : Iterator.Ge;

            var selectPacket = new SelectRequest<TKey>(SpaceId, Id, 1, 0, iterator, key);

            var minResponse = await LogicalConnection.SendRequest<SelectRequest<TKey>, TTuple>(selectPacket);
            return minResponse.Data.SingleOrDefault();
        }

        public Task<TTuple> Max<TTuple>()
            where TTuple : ITarantoolTuple
        {
            return Max<TTuple, TarantoolTuple>(TarantoolTuple.Empty);
        }

        public async Task<TTuple> Max<TTuple, TKey>(TKey key = null)
            where TTuple : ITarantoolTuple
            where TKey : class, ITarantoolTuple
        {
            if (Type != IndexType.Tree)
            {
                throw ExceptionHelper.WrongIndexType("TREE", "max");
            }
            var iterator = key == null ? Iterator.Req : Iterator.Le;

            var selectPacket = new SelectRequest<TKey>(SpaceId, Id, 1, 0, iterator, key);

            var maxResponse = await LogicalConnection.SendRequest<SelectRequest<TKey>, TTuple>(selectPacket);
            return maxResponse.Data.SingleOrDefault();
        }

        public TTuple Random<TTuple>(int randomValue)
            where TTuple : ITarantoolTuple
        {
            throw new NotImplementedException();
        }

        public uint Count<TKey>(TKey key = null, Iterator it = Iterator.Eq)
            where TKey : class, ITarantoolTuple
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<TTuple[]>> Update<TTuple, TKey>(TKey key, UpdateOperation[] updateOperations)
            where TKey : ITarantoolTuple
            where TTuple : ITarantoolTuple
        {
            var updateRequest = new UpdateRequest<TKey>(
                SpaceId,
                Id,
                key,
                updateOperations);

            return LogicalConnection.SendRequest<UpdateRequest<TKey>, TTuple>(updateRequest);
        }

        public Task Upsert<TKey>(TKey key, UpdateOperation[] updateOperations)
            where TKey : ITarantoolTuple
        {
            var updateRequest = new UpsertRequest<TKey>(
                SpaceId,
                key,
                updateOperations);

            return LogicalConnection.SendRequestWithEmptyResponse(updateRequest);
        }

        public Task<DataResponse<TTuple[]>> Delete<TKey, TTuple>(TKey key)
            where TKey : ITarantoolTuple
            where TTuple : ITarantoolTuple
        {
            var deleteRequest = new DeleteRequest<TKey>(SpaceId, Id, key);

            return LogicalConnection.SendRequest<DeleteRequest<TKey>, TTuple>(deleteRequest);
        }

        public Task Alter(IndexCreationOptions options)
        {
            throw new NotImplementedException();
        }

        public Task Drop()
        {
            throw new NotImplementedException();
        }

        public Task Rename(string indexName)
        {
            throw new NotImplementedException();
        }

        public Task<uint> BSize()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Name}, id={Id}, spaceId={SpaceId}";
        }
    }
}