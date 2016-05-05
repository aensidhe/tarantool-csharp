﻿using System;
using System.Collections.Generic;
using System.Linq;
using iproto;
using iproto.Data;
using iproto.Data.Packets;
using iproto.Data.UpdateOperations;

namespace tarantool_client
{
    public class Index
    {
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

        public Connection Connection { get; set; }

        public IEnumerable<TResult> Pairs<TValue, TResult>(TValue value, Iterator iterator)
            where TResult : ITuple
        {
            throw new NotImplementedException();
        }

        public ResponsePacket<TTuple[]> Select<TTuple, TKey>(TKey key, SelectOptions options = null)
            where TKey : ITuple
            where TTuple : ITuple
        {
            var selectRequest = new SelectPacket<TKey>(
                SpaceId,
                Id,
                options?.Limit ?? uint.MaxValue,
                options?.Offset ?? 0,
                options?.Iterator ?? Iterator.Eq,
                key);

            return Connection.SendPacket<SelectPacket<TKey>, TTuple[]>(selectRequest);
        }

        public TTuple Min<TTuple, TKey>(TKey key = null)
            where TTuple : ITuple
            where TKey : class, ITuple
        {
            if (Type != IndexType.Tree)
            {
                throw new NotSupportedException("Only TREE indicies support min opration.");
            }
            var iterator = key == null ? Iterator.Eq : Iterator.Ge;

            var selectPacket = new SelectPacket<TKey>(SpaceId, Id, 1, 0, iterator, key);

            return Connection.SendPacket<SelectPacket<TKey>, TTuple[]>(selectPacket).Data.SingleOrDefault();
        }

        public TTuple Max<TTuple, TKey>(TKey key = null)
            where TTuple : ITuple
            where TKey : class, ITuple
        {
            if (Type != IndexType.Tree)
            {
                throw new NotSupportedException("Only TREE indicies support max opration.");
            }
            var iterator = key == null ? Iterator.Req : Iterator.Le;

            var selectPacket = new SelectPacket<TKey>(SpaceId, Id, 1, 0, iterator, key);

            return Connection.SendPacket<SelectPacket<TKey>, TTuple[]>(selectPacket).Data.SingleOrDefault();
        }

        public TTuple Random<TTuple>(int randomValue)
            where TTuple : ITuple
        {
            throw new NotImplementedException();
        }

        public uint Count<TKey>(TKey key = null, Iterator it = Iterator.Eq)
            where TKey : class, ITuple
        {
            throw new NotImplementedException();
        }

        public ResponsePacket<TTuple[]> Update<TTuple, TKey, TUpdate>(TKey key, UpdateOperation<TUpdate> updateOperation)
            where TKey : ITuple
        {
            var updateRequest = new UpdatePacket<TKey, TUpdate>(
                SpaceId,
                Id,
                key,
                updateOperation);

            return Connection.SendPacket<UpdatePacket<TKey, TUpdate>, TTuple[]>(updateRequest);
        }

        public ResponsePacket<TTuple[]> Delete<TTuple, TKey>(TKey key)
            where TKey : ITuple
        {
            var deleteRequest = new DeletePacket<TKey>(SpaceId, Id, key);

            return Connection.SendPacket<DeletePacket<TKey>, TTuple[]>(deleteRequest);
        }

        public void Alter(IndexCreationOptions options)
        {
            throw new NotImplementedException();
        }

        public void Drop()
        {
            throw new NotImplementedException();
        }

        public void Rename(string indexName)
        {
            throw new NotImplementedException();
        }

        public uint BSize()
        {
            throw new NotImplementedException();
        }
    }
}