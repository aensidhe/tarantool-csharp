﻿using System.Runtime.Serialization;
using ProGaudi.MsgPack.Light;
using ProGaudi.Tarantool.Client.Utils;

namespace ProGaudi.Tarantool.Client.Model
{
    public class IndexPart
    {
        public IndexPart(uint fieldNo, FieldType type)
        {
            FieldNo = fieldNo;
            Type = type;
        }

        public uint FieldNo { get; }

        public FieldType Type { get; }

        public override string ToString()
        {
            return $"[{FieldNo}, {Type}]";
        }

        public sealed class Formatter : IMsgPackConverter<IndexPart>
        {
            private IMsgPackConverter<uint> _uintConverter;
            private IMsgPackConverter<FieldType> _indexPartTypeConverter;
            private IMsgPackConverter<string> _stringConverter;

            public void Initialize(MsgPackContext context)
            {
                _uintConverter = context.GetConverter<uint>();
                _indexPartTypeConverter = context.GetConverter<FieldType>();
                _stringConverter = context.GetConverter<string>();
            }

            public void Write(IndexPart value, IMsgPackWriter writer) => throw new System.NotImplementedException();

            public IndexPart Read(IMsgPackReader reader)
            {
                var type = reader.ReadDataType();
                switch (type)
                {
                    case DataTypes.Map16:
                        return ReadFromMap(reader, ReadUInt16(reader));

                    case DataTypes.Map32:
                        return ReadFromMap(reader, ReadUInt32(reader));

                    case DataTypes.Array16:
                        return ReadFromArray(reader, ReadUInt16(reader));

                    case DataTypes.Array32:
                        return ReadFromArray(reader, ReadUInt32(reader));
                }

                var length = TryGetLengthFromFixMap(type);
                if (length.HasValue)
                {
                    return ReadFromMap(reader, length.Value);
                }

                length = TryGetLengthFromFixArray(type);
                if (length != null)
                {
                    return ReadFromArray(reader, length.Value);
                }

                throw ExceptionUtils.BadTypeException(type, DataTypes.Map16, DataTypes.Map32, DataTypes.FixMap, DataTypes.Array16, DataTypes.Array32, DataTypes.FixArray);
            }

            private IndexPart ReadFromArray(IMsgPackReader reader, uint length)
            {
                if (length != 2u)
                {
                    throw ExceptionHelper.InvalidArrayLength(2u, length);
                }

                var fieldNo = _uintConverter.Read(reader);
                var indexPartType = _indexPartTypeConverter.Read(reader);

                return new IndexPart(fieldNo, indexPartType);
            }

            private IndexPart ReadFromMap(IMsgPackReader reader, uint length)
            {
                uint? fieldNo = null;
                FieldType? indexPartType = null;

                for (var i = 0; i < length; i++)
                {
                    switch (_stringConverter.Read(reader))
                    {
                        case "field":
                            fieldNo = _uintConverter.Read(reader);
                            break;
                        case "type":
                            indexPartType = _indexPartTypeConverter.Read(reader);
                            break;
                        default:
                            reader.SkipToken();
                            break;
                    }
                }

                if (fieldNo.HasValue && indexPartType.HasValue)
                {
                    return new IndexPart(fieldNo.Value, indexPartType.Value);
                }

                throw new SerializationException("Can't read fieldNo or indexPart from map of index metadata");
            }

            private static uint? TryGetLengthFromFixArray(DataTypes type)
            {
                var length = type - DataTypes.FixArray;
                return type.GetHighBits(4) == DataTypes.FixArray.GetHighBits(4) ? length : (uint?)null;
            }

            private static uint? TryGetLengthFromFixMap(DataTypes type)
            {
                var length = type - DataTypes.FixMap;
                return type.GetHighBits(4) == DataTypes.FixMap.GetHighBits(4) ? length : (uint?)null;
            }

            internal static ushort ReadUInt16(IMsgPackReader reader)
            {
                return (ushort)((reader.ReadByte() << 8) + reader.ReadByte());
            }

            internal static uint ReadUInt32(IMsgPackReader reader)
            {
                var temp = (uint)(reader.ReadByte() << 24);
                temp += (uint)reader.ReadByte() << 16;
                temp += (uint)reader.ReadByte() << 8;
                temp += reader.ReadByte();

                return temp;
            }
        }
    }
}