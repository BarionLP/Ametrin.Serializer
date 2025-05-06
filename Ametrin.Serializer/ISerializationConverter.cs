using System;

namespace Ametrin.Serializer;

public interface ISerializationConverter<T>
{
    public abstract static void WriteProperty(IAmetrinWriter writer, ReadOnlySpan<char> name, T value);
    public abstract static T ReadProperty(IAmetrinReader reader, ReadOnlySpan<char> name);
}
