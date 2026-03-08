using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Ametrin.Optional;

namespace Ametrin.Serializer.Readers;

public sealed class AmetrinJsonReader(JsonElement element) : IAmetrinReader
{
    private readonly JsonElement element = element;
    private JsonElement? currentProperty = null;

    public static AmetrinJsonReader Create(Stream stream) => new(JsonDocument.Parse(stream).RootElement);

    public ErrorState<DeserializationError> TryReadPropertyName(ReadOnlySpan<char> name)
    {
        if (element.TryGetProperty(name, out var prop))
        {
            currentProperty = prop;
            return default;
        }
        return DeserializationError.CreatePropertyNotFound(name.ToString());
    }

    public Result<string, DeserializationError> TryReadStringValue() => TryGet<string>("String", static (property, [MaybeNullWhen(false)] out value) => Option.Success(property).Require(static p => p.ValueKind is JsonValueKind.String).Map(static p => p.GetString()).Branch(out value));
    public Result<byte[], DeserializationError> TryReadBytesValue() => TryGet<byte[]>("Byte[]", static (property, [MaybeNullWhen(false)] out value) => property.TryGetBytesFromBase64(out value));
    public Result<int, DeserializationError> TryReadInt32Value() => TryGet<int>("Int32", static (property, out value) => property.TryGetInt32(out value));
    public Result<Half, DeserializationError> TryReadHalfValue() => TryGet<float>("Half", static (property, out value) => property.TryGetSingle(out value)).Map(static f => (Half)f);
    public Result<float, DeserializationError> TryReadSingleValue() => TryGet<float>("Single", static (property, out value) => property.TryGetSingle(out value));
    public Result<double, DeserializationError> TryReadDoubleValue() => TryGet<double>("Double", static (property, out value) => property.TryGetDouble(out value));
    public Result<bool, DeserializationError> TryReadBooleanValue() => GetCurrentProperty().ValueKind switch { JsonValueKind.True => true, JsonValueKind.False => false, _ => DeserializationError.CreateInvalidPropertyType("<todo>", "Boolean") };
    public Result<DateTime, DeserializationError> TryReadDateTimeValue() => TryGet<DateTime>("DateTime", static (property, out value) => property.TryGetDateTime(out value));

    private delegate bool TryGetDelegate<T>(JsonElement element, [MaybeNullWhen(false)] out T result);
    private Result<T, DeserializationError> TryGet<T>(string type, TryGetDelegate<T> getter)
    {
        var current = GetCurrentProperty();
        if (getter(current, out var result))
        {
            return result;
        }
        return DeserializationError.CreateInvalidPropertyType("<todo>", type);
    }


    private JsonElement GetCurrentProperty()
    {
        if (currentProperty is { } current)
        {
            currentProperty = null;
            return current;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public IAmetrinReader ReadStartObject() => new AmetrinJsonReader(GetCurrentProperty());
    public void ReadEndObject() { }

    public void ReadStartArray() { }
    public void ReadEndArray() { }
}
