using System;
using Ametrin.Optional;

namespace Ametrin.Serializer;

public interface ISerializationConverter<T>
{
    public abstract static void WriteProperty(IAmetrinWriter writer, ReadOnlySpan<char> name, T value);
    public abstract static T ReadProperty(IAmetrinReader reader, ReadOnlySpan<char> name);
    public abstract static Result<T, DeserializationError> TryReadProperty(IAmetrinReader reader, ReadOnlySpan<char> name);
}
