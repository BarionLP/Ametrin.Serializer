using System;
using System.Security.Cryptography;
using Ametrin.Optional;

namespace Ametrin.Serializer.Converters;

public sealed class HashAlgorithmConverter : ISerializationConverter<HashAlgorithmName>
{
    public static HashAlgorithmName ReadProperty(IAmetrinReader reader, ReadOnlySpan<char> name)
    {
        return new HashAlgorithmName(reader.ReadStringProperty(name));
    }

    public static void WriteProperty(IAmetrinWriter writer, ReadOnlySpan<char> name, HashAlgorithmName value)
    {
        writer.WriteStringProperty(name, value.Name);
    }

    public static Result<HashAlgorithmName, DeserializationError> TryReadProperty(IAmetrinReader reader, ReadOnlySpan<char> name)
    {
        return reader.TryReadStringProperty(name).Map(static name => new HashAlgorithmName(name));
    }
}