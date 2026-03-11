using System;
using System.Collections.Frozen;
using Ametrin.Optional;

namespace Ametrin.Serializer.Converters;

public sealed class EnumConverter<TEnum> : ISerializationConverter<TEnum> where TEnum : struct, Enum
{
    private static readonly FrozenDictionary<string, TEnum> values = Enum.GetValues<TEnum>().ToFrozenDictionary(static v => v.ToString(), StringComparer.OrdinalIgnoreCase);

    public static void WriteValue(IAmetrinWriter writer, TEnum value)
    {
        writer.WriteStringValue(value.ToString());
    }

    public static TEnum ReadValue(IAmetrinReader reader)
    {
        return values[reader.TryReadStringValue().OrThrow()];
    }

    public static Result<TEnum, DeserializationError> TryReadValue(IAmetrinReader reader)
    {
        return reader.TryReadStringValue().Map(ConvertToEnum);

        static Result<TEnum, DeserializationError> ConvertToEnum(string stringValue)
            => values.TryGetValue(stringValue, out var value) ? value : DeserializationError.CreateInvalidValue("", typeof(TEnum).Name, stringValue);
    }
}