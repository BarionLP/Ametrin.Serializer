using System;
using System.Security.Cryptography;
using Ametrin.Optional;

namespace Ametrin.Serializer.Converters;

public sealed class HashAlgorithmConverter : ISerializationConverter<HashAlgorithmName>
{
    public static HashAlgorithmName ReadValue(IAmetrinReader reader)
    {
        return TryReadValue(reader).OrThrow();
    }

    public static void WriteValue(IAmetrinWriter writer, HashAlgorithmName value)
    {
        writer.WriteStringValue(value.Name);
    }

    public static Result<HashAlgorithmName, DeserializationError> TryReadValue(IAmetrinReader reader)
    {
        return reader.TryReadStringValue().Map(static name => new HashAlgorithmName(name));
    }
}