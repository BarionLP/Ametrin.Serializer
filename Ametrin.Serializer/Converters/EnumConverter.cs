using System;
using System.Collections.Frozen;
using Ametrin.Optional;

namespace Ametrin.Serializer.Converters;

public sealed class EnumConverter<TEnum> : ISerializationConverter<TEnum> where TEnum : struct, Enum
{
    private static readonly FrozenDictionary<string, TEnum> values = Enum.GetValues<TEnum>().ToFrozenDictionary(static v => v.ToString(), StringComparer.OrdinalIgnoreCase);

    public static void WriteProperty(IAmetrinWriter writer, ReadOnlySpan<char> name, TEnum value)
    {
        writer.WriteStringProperty(name, value.ToString());
    }

    public static TEnum ReadProperty(IAmetrinReader reader, ReadOnlySpan<char> name)
    {
        return values[reader.ReadStringProperty(name)];
    }

    public static Result<TEnum, DeserializationError> TryReadProperty(IAmetrinReader reader, ReadOnlySpan<char> name)
    {
        return reader.TryReadStringProperty(name).Map(name, ConvertToEnum);

        static Result<TEnum, DeserializationError> ConvertToEnum(string stringValue, ReadOnlySpan<char> name)
            => values.TryGetValue(stringValue, out var value) ? value : DeserializationError.CreateInvalidPropertyType(name.ToString(), typeof(TEnum).Name);
    }
}