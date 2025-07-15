using System;

namespace ValueOf.Extensions
{
    public static class BuiltinTypeParser
    {
        private static class Cache<T>
        {
            public static Func<string, T> Convert;
        }

        static BuiltinTypeParser()
        {
            Cache<bool>.Convert = Convert.ToBoolean;
            Cache<byte>.Convert = Convert.ToByte;
            Cache<sbyte>.Convert = Convert.ToSByte;
            Cache<ushort>.Convert = Convert.ToUInt16;
            Cache<short>.Convert = Convert.ToInt16;
            Cache<uint>.Convert = Convert.ToUInt32;
            Cache<int>.Convert = Convert.ToInt32;
            Cache<ulong>.Convert = Convert.ToUInt64;
            Cache<long>.Convert = Convert.ToInt64;
            Cache<float>.Convert = s => (float)Convert.ToDouble(s);
            Cache<double>.Convert = Convert.ToDouble;
            Cache<decimal>.Convert = Convert.ToDecimal;
            Cache<Guid>.Convert = Guid.Parse;
            Cache<DateTime>.Convert = DateTime.Parse;
            Cache<DateTimeOffset>.Convert = DateTimeOffset.Parse;
            Cache<string>.Convert = s => s;
#if NET6_0_OR_GREATER
            Cache<DateOnly>.Convert = DateOnly.Parse;
            Cache<TimeOnly>.Convert = TimeOnly.Parse;
#endif
        }

        public static T Parse<T>(string s)
        {
            var converter = Cache<T>.Convert;
            if (converter == null)
                throw new InvalidOperationException($"ValueConverter does not parsing to type: {typeof(T).FullName}");
            return converter(s);
        }
    }
}