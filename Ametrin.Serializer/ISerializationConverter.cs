using System;
using Ametrin.Optional;

namespace Ametrin.Serializer;

public interface ISerializationConverter<T>
{
    public abstract static T ReadValue(IAmetrinReader reader);
    public abstract static Result<T, DeserializationError> TryReadValue(IAmetrinReader reader);
    public abstract static void WriteValue(IAmetrinWriter writer, T value);
}
