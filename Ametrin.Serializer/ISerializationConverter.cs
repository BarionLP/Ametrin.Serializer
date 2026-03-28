using System;
using Ametrin.Optional;

namespace Ametrin.Serializer;

public interface ISerializationConverter<T>
{
    public abstract static Result<T, DeserializationError> TryReadValue(IAmetrinReader reader);
    public abstract static void WriteValue(IAmetrinWriter writer, T value);
}

public static class SerializationConverterExtension
{
    extension<TConverter, TValue>(TConverter) where TConverter : ISerializationConverter<TValue>
    {
        public static Result<TValue, DeserializationError> TryReadProperty(IAmetrinReader reader, ReadOnlySpan<char> name)
        {
            return reader.TryReadPropertyName(name).ToResult(() => TConverter.TryReadValue(reader)).Map(static v => v);
        }
        public static TValue ReadValue(IAmetrinReader reader) => TConverter.TryReadValue(reader).Or(static e => e.Throw<TValue>());
    }
}