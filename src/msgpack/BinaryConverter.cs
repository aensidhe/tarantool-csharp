using System;

namespace TarantoolDnx.MsgPack
{
    internal class BinaryConverter : IMsgPackConverter<byte[]>
    {
        public void Write(byte[] value, IMsgPackWriter writer, MsgPackContext context)
        {
            if (value == null)
            {
                context.NullConverter.Write(value, writer, context);
                return;
            }

            WriteBinaryHeaderAndLength(value.Length, writer);

            writer.Write(value);
        }

        // We will have problem with binary blobs greater than int.MaxValue bytes.
        public byte[] Read(IMsgPackReader reader, MsgPackContext context, Func<byte[]> creator)
        {
            var type = reader.ReadDataType();

            uint length;
            switch (type)
            {
                case DataTypes.Null:
                    return null;

                case DataTypes.Bin8:
                    length = IntConverter.ReadUInt8(reader);
                    break;

                case DataTypes.Bin16:
                    length = IntConverter.ReadUInt16(reader);
                    break;

                case DataTypes.Bin32:
                    length = IntConverter.ReadUInt32(reader);
                    break;

                default:
                    throw ExceptionUtils.BadTypeException(type, DataTypes.Bin8, DataTypes.Bin16, DataTypes.Bin32, DataTypes.Null);
            }

            return ReadByteArray(reader, length);
        }

        internal static byte[] ReadByteArray(IMsgPackReader reader, uint length)
        {
            var buffer = new byte[length];

            reader.ReadBytes(buffer);

            return buffer;
        }

        private void WriteBinaryHeaderAndLength(int length, IMsgPackWriter writer)
        {
            if (length <= byte.MaxValue)
            {
                writer.Write(DataTypes.Bin8);
                IntConverter.WriteValue((byte)length, writer);
                return;
            }

            if (length <= ushort.MaxValue)
            {
                writer.Write(DataTypes.Bin16);
                IntConverter.WriteValue((ushort)length, writer);
            }
            else
            {
                writer.Write(DataTypes.Bin32);
                IntConverter.WriteValue((uint)length, writer);
            }
        }
    }
}