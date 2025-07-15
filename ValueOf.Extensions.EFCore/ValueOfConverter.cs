using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ValueOf.Extensions.EFCore
{
    public sealed class ValueOfConverter<TU, T> : ValueConverter<T, TU> where T : ValueOf<TU, T>, new()
    {
        public ValueOfConverter() : base(wrapped => wrapped.Value, val => ValueOf<TU, T>.From(val))
        {
        }
    }
}