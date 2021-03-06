using ProGaudi.MsgPack.Light;

using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Utils;

namespace ProGaudi.Tarantool.Client.Converters
{
    internal class TupleConverter : IMsgPackConverter<TarantoolTuple>
    {
        private IMsgPackConverter<object> _nullConverter;

        public void Initialize(MsgPackContext context)
        {
            _nullConverter = context.NullConverter;
        }

        public void Write(TarantoolTuple value, IMsgPackWriter writer)
        {
            if (value == null)
            {
                _nullConverter.Write(null, writer);
                return;
            }

            writer.WriteArrayHeader(0);
        }

        public TarantoolTuple Read(IMsgPackReader reader)
        {
            var actual = reader.ReadArrayLength();
            if (actual == null)
            {
                return null;
            }

            const uint expected = 0u;
            if (actual != expected)
            {
                throw ExceptionHelper.InvalidArrayLength(expected, actual);
            }

            return TarantoolTuple.Empty;
        }
    }

    public class TupleConverter<T1> : IMsgPackConverter<TarantoolTuple<T1>>
    {
        private IMsgPackConverter<object> _nullConverter;
        private IMsgPackConverter<T1> _t1Converter;

        public void Initialize(MsgPackContext context)
        {
            _nullConverter = context.NullConverter;
            _t1Converter = context.GetConverter<T1>();
        }

        public void Write(TarantoolTuple<T1> value, IMsgPackWriter writer)
        {
            if (value == null)
            {
                _nullConverter.Write(null, writer);
                return;
            }

            writer.WriteArrayHeader(1);
            _t1Converter.Write(value.Item1, writer);
        }

        public TarantoolTuple<T1> Read(IMsgPackReader reader)
        {
            var actual = reader.ReadArrayLength();
            if (actual == null)
            {
                return null;
            }


            const uint expected = 1u;
            if (actual != expected)
            {
                throw ExceptionHelper.InvalidArrayLength(expected, actual);
            }

            var item1 = _t1Converter.Read(reader);

            return TarantoolTuple.Create(item1);
        }
    }

    public class TupleConverter<T1, T2> : IMsgPackConverter<TarantoolTuple<T1, T2>>
    {
        private IMsgPackConverter<object> _nullConverter;
        private IMsgPackConverter<T1> _t1Converter;
        private IMsgPackConverter<T2> _t2Converter;

        public void Initialize(MsgPackContext context)
        {
            _nullConverter = context.NullConverter;
            _t1Converter = context.GetConverter<T1>();
            _t2Converter = context.GetConverter<T2>();
        }

        public void Write(TarantoolTuple<T1, T2> value, IMsgPackWriter writer)
        {
            if (value == null)
            {
                _nullConverter.Write(null, writer);
                return;
            }

            writer.WriteArrayHeader(2);
            _t1Converter.Write(value.Item1, writer);
            _t2Converter.Write(value.Item2, writer);
        }

        public TarantoolTuple<T1, T2> Read(IMsgPackReader reader)
        {
            var actual = reader.ReadArrayLength();
            if (actual == null)
            {
                return null;
            }

            const uint expected = 2u;
            if (actual != expected)
            {
                throw ExceptionHelper.InvalidArrayLength(expected, actual);
            }

            var item1 = _t1Converter.Read(reader);
            var item2 = _t2Converter.Read(reader);

            return TarantoolTuple.Create(item1, item2);
        }
    }

    public class TupleConverter<T1, T2, T3> : IMsgPackConverter<TarantoolTuple<T1, T2, T3>>
    {
        private IMsgPackConverter<object> _nullConverter;
        private IMsgPackConverter<T1> _t1Converter;
        private IMsgPackConverter<T2> _t2Converter;
        private IMsgPackConverter<T3> _t3Converter;

        public void Initialize(MsgPackContext context)
        {
            _nullConverter = context.NullConverter;
            _t1Converter = context.GetConverter<T1>();
            _t2Converter = context.GetConverter<T2>();
            _t3Converter = context.GetConverter<T3>();
        }

        public void Write(TarantoolTuple<T1, T2, T3> value, IMsgPackWriter writer)
        {
            if (value == null)
            {
                _nullConverter.Write(null, writer);
                return;
            }

            writer.WriteArrayHeader(3);
            _t1Converter.Write(value.Item1, writer);
            _t2Converter.Write(value.Item2, writer);
            _t3Converter.Write(value.Item3, writer);
        }

        public TarantoolTuple<T1, T2, T3> Read(IMsgPackReader reader)
        {
            var actual = reader.ReadArrayLength();
            if (actual == null)
            {
                return null;
            }


            const uint expected = 3u;
            if (actual != expected)
            {
                throw ExceptionHelper.InvalidArrayLength(expected, actual);
            }

            var item1 = _t1Converter.Read(reader);
            var item2 = _t2Converter.Read(reader);
            var item3 = _t3Converter.Read(reader);

            return TarantoolTuple.Create(item1, item2, item3);
        }
    }

    public class TupleConverter<T1, T2, T3, T4> : IMsgPackConverter<TarantoolTuple<T1, T2, T3, T4>>
    {
        private IMsgPackConverter<object> _nullConverter;
        private IMsgPackConverter<T1> _t1Converter;
        private IMsgPackConverter<T2> _t2Converter;
        private IMsgPackConverter<T3> _t3Converter;
        private IMsgPackConverter<T4> _t4Converter;

        public void Initialize(MsgPackContext context)
        {
            _nullConverter = context.NullConverter;
            _t1Converter = context.GetConverter<T1>();
            _t2Converter = context.GetConverter<T2>();
            _t3Converter = context.GetConverter<T3>();
            _t4Converter = context.GetConverter<T4>();
        }

        public void Write(TarantoolTuple<T1, T2, T3, T4> value, IMsgPackWriter writer)
        {
            if (value == null)
            {
                _nullConverter.Write(null, writer);
                return;
            }

            writer.WriteArrayHeader(4);
            _t1Converter.Write(value.Item1, writer);
            _t2Converter.Write(value.Item2, writer);
            _t3Converter.Write(value.Item3, writer);
            _t4Converter.Write(value.Item4, writer);
        }

        public TarantoolTuple<T1, T2, T3, T4> Read(IMsgPackReader reader)
        {
            var actual = reader.ReadArrayLength();
            if (actual == null)
            {
                return null;
            }


            const uint expected = 4u;
            if (actual != expected)
            {
                throw ExceptionHelper.InvalidArrayLength(expected, actual);
            }

            var item1 = _t1Converter.Read(reader);
            var item2 = _t2Converter.Read(reader);
            var item3 = _t3Converter.Read(reader);
            var item4 = _t4Converter.Read(reader);

            return TarantoolTuple.Create(item1, item2, item3, item4);
        }
    }

    public class TupleConverter<T1, T2, T3, T4, T5> : IMsgPackConverter<TarantoolTuple<T1, T2, T3, T4, T5>>
    {
        private IMsgPackConverter<object> _nullConverter;
        private IMsgPackConverter<T1> _t1Converter;
        private IMsgPackConverter<T2> _t2Converter;
        private IMsgPackConverter<T3> _t3Converter;
        private IMsgPackConverter<T4> _t4Converter;
        private IMsgPackConverter<T5> _t5Converter;

        public void Initialize(MsgPackContext context)
        {
            _nullConverter = context.NullConverter;
            _t1Converter = context.GetConverter<T1>();
            _t2Converter = context.GetConverter<T2>();
            _t3Converter = context.GetConverter<T3>();
            _t4Converter = context.GetConverter<T4>();
            _t5Converter = context.GetConverter<T5>();
        }

        public void Write(TarantoolTuple<T1, T2, T3, T4, T5> value, IMsgPackWriter writer)
        {
            if (value == null)
            {
                _nullConverter.Write(null, writer);
                return;
            }

            writer.WriteArrayHeader(5);
            _t1Converter.Write(value.Item1, writer);
            _t2Converter.Write(value.Item2, writer);
            _t3Converter.Write(value.Item3, writer);
            _t4Converter.Write(value.Item4, writer);
            _t5Converter.Write(value.Item5, writer);
        }

        public TarantoolTuple<T1, T2, T3, T4, T5> Read(IMsgPackReader reader)
        {
            var actual = reader.ReadArrayLength();
            if (actual == null)
            {
                return null;
            }


            const uint expected = 5u;
            if (actual != expected)
            {
                throw ExceptionHelper.InvalidArrayLength(expected, actual);
            }

            var item1 = _t1Converter.Read(reader);
            var item2 = _t2Converter.Read(reader);
            var item3 = _t3Converter.Read(reader);
            var item4 = _t4Converter.Read(reader);
            var item5 = _t5Converter.Read(reader);

            return TarantoolTuple.Create(item1, item2, item3, item4, item5);
        }
    }

    public class TupleConverter<T1, T2, T3, T4, T5, T6> : IMsgPackConverter<TarantoolTuple<T1, T2, T3, T4, T5, T6>>
    {
        private IMsgPackConverter<object> _nullConverter;
        private IMsgPackConverter<T1> _t1Converter;
        private IMsgPackConverter<T2> _t2Converter;
        private IMsgPackConverter<T3> _t3Converter;
        private IMsgPackConverter<T4> _t4Converter;
        private IMsgPackConverter<T5> _t5Converter;
        private IMsgPackConverter<T6> _t6Converter;

        public void Initialize(MsgPackContext context)
        {
            _nullConverter = context.NullConverter;
            _t1Converter = context.GetConverter<T1>();
            _t2Converter = context.GetConverter<T2>();
            _t3Converter = context.GetConverter<T3>();
            _t4Converter = context.GetConverter<T4>();
            _t5Converter = context.GetConverter<T5>();
            _t6Converter = context.GetConverter<T6>();
        }

        public void Write(TarantoolTuple<T1, T2, T3, T4, T5, T6> value, IMsgPackWriter writer)
        {
            if (value == null)
            {
                _nullConverter.Write(null, writer);
                return;
            }

            writer.WriteArrayHeader(6);
            _t1Converter.Write(value.Item1, writer);
            _t2Converter.Write(value.Item2, writer);
            _t3Converter.Write(value.Item3, writer);
            _t4Converter.Write(value.Item4, writer);
            _t5Converter.Write(value.Item5, writer);
            _t6Converter.Write(value.Item6, writer);
        }

        public TarantoolTuple<T1, T2, T3, T4, T5, T6> Read(
            IMsgPackReader reader)
        {
            var actual = reader.ReadArrayLength();
            if (actual == null)
            {
                return null;
            }


            const uint expected = 6u;
            if (actual != expected)
            {
                throw ExceptionHelper.InvalidArrayLength(expected, actual);
            }

            var item1 = _t1Converter.Read(reader);
            var item2 = _t2Converter.Read(reader);
            var item3 = _t3Converter.Read(reader);
            var item4 = _t4Converter.Read(reader);
            var item5 = _t5Converter.Read(reader);
            var item6 = _t6Converter.Read(reader);

            return TarantoolTuple.Create(item1, item2, item3, item4, item5, item6);
        }
    }

    public class TupleConverter<T1, T2, T3, T4, T5, T6, T7> : IMsgPackConverter<TarantoolTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        private IMsgPackConverter<object> _nullConverter;
        private IMsgPackConverter<T1> _t1Converter;
        private IMsgPackConverter<T2> _t2Converter;
        private IMsgPackConverter<T3> _t3Converter;
        private IMsgPackConverter<T4> _t4Converter;
        private IMsgPackConverter<T5> _t5Converter;
        private IMsgPackConverter<T6> _t6Converter;
        private IMsgPackConverter<T7> _t7Converter;

        public void Initialize(MsgPackContext context)
        {
            _nullConverter = context.NullConverter;
            _t1Converter = context.GetConverter<T1>();
            _t2Converter = context.GetConverter<T2>();
            _t3Converter = context.GetConverter<T3>();
            _t4Converter = context.GetConverter<T4>();
            _t5Converter = context.GetConverter<T5>();
            _t6Converter = context.GetConverter<T6>();
            _t7Converter = context.GetConverter<T7>();
        }

        public void Write(TarantoolTuple<T1, T2, T3, T4, T5, T6, T7> value, IMsgPackWriter writer)
        {
            if (value == null)
            {
                _nullConverter.Write(null, writer);
                return;
            }

            writer.WriteArrayHeader(7);
            _t1Converter.Write(value.Item1, writer);
            _t2Converter.Write(value.Item2, writer);
            _t3Converter.Write(value.Item3, writer);
            _t4Converter.Write(value.Item4, writer);
            _t5Converter.Write(value.Item5, writer);
            _t6Converter.Write(value.Item6, writer);
            _t7Converter.Write(value.Item7, writer);
        }

        public TarantoolTuple<T1, T2, T3, T4, T5, T6, T7> Read(
            IMsgPackReader reader)
        {
            var actual = reader.ReadArrayLength();
            if (actual == null)
            {
                return null;
            }


            const uint expected = 7u;
            if (actual != expected)
            {
                throw ExceptionHelper.InvalidArrayLength(expected, actual);
            }

            var item1 = _t1Converter.Read(reader);
            var item2 = _t2Converter.Read(reader);
            var item3 = _t3Converter.Read(reader);
            var item4 = _t4Converter.Read(reader);
            var item5 = _t5Converter.Read(reader);
            var item6 = _t6Converter.Read(reader);
            var item7 = _t7Converter.Read(reader);

            return TarantoolTuple.Create(item1, item2, item3, item4, item5, item6, item7);
        }
    }

    public class TupleConverter<T1, T2, T3, T4, T5, T6, T7, TRest> : IMsgPackConverter<TarantoolTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
    {
        private IMsgPackConverter<object> _nullConverter;
        private IMsgPackConverter<T1> _t1Converter;
        private IMsgPackConverter<T2> _t2Converter;
        private IMsgPackConverter<T3> _t3Converter;
        private IMsgPackConverter<T4> _t4Converter;
        private IMsgPackConverter<T5> _t5Converter;
        private IMsgPackConverter<T6> _t6Converter;
        private IMsgPackConverter<T7> _t7Converter;
        private IMsgPackConverter<TRest> _t8Converter;

        public void Initialize(MsgPackContext context)
        {
            _nullConverter = context.NullConverter;
            _t1Converter = context.GetConverter<T1>();
            _t2Converter = context.GetConverter<T2>();
            _t3Converter = context.GetConverter<T3>();
            _t4Converter = context.GetConverter<T4>();
            _t5Converter = context.GetConverter<T5>();
            _t6Converter = context.GetConverter<T6>();
            _t7Converter = context.GetConverter<T7>();
            _t8Converter = context.GetConverter<TRest>();
        }

        public void Write(TarantoolTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value, IMsgPackWriter writer)
        {
            if (value == null)
            {
                _nullConverter.Write(null, writer);
                return;
            }

            writer.WriteArrayHeader(8);
            _t1Converter.Write(value.Item1, writer);
            _t2Converter.Write(value.Item2, writer);
            _t3Converter.Write(value.Item3, writer);
            _t4Converter.Write(value.Item4, writer);
            _t5Converter.Write(value.Item5, writer);
            _t6Converter.Write(value.Item6, writer);
            _t7Converter.Write(value.Item7, writer);
            _t8Converter.Write(value.Item8, writer);
        }

        public TarantoolTuple<T1, T2, T3, T4, T5, T6, T7, TRest> Read(
            IMsgPackReader reader)
        {
            var actual = reader.ReadArrayLength();
            if (actual == null)
            {
                return null;
            }


            const uint expected = 8u;
            if (actual != expected)
            {
                throw ExceptionHelper.InvalidArrayLength(expected, actual);
            }

            var item1 = _t1Converter.Read(reader);
            var item2 = _t2Converter.Read(reader);
            var item3 = _t3Converter.Read(reader);
            var item4 = _t4Converter.Read(reader);
            var item5 = _t5Converter.Read(reader);
            var item6 = _t6Converter.Read(reader);
            var item7 = _t7Converter.Read(reader);
            var item8 = _t8Converter.Read(reader);

            return new TarantoolTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, item8);
        }
    }
}