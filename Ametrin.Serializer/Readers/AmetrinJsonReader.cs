using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Ametrin.Optional;

namespace Ametrin.Serializer.Readers;

public abstract class AmetrinJsonReader : IAmetrinReader
{
    public static AmetrinJsonObjectReader Create(Stream stream) => new(JsonDocument.Parse(stream).RootElement);

    public abstract ErrorState<DeserializationError> TryReadPropertyName(ReadOnlySpan<char> name);

    public Result<string, DeserializationError> TryReadStringValue() => TryGet<string>("String", static (property, [MaybeNullWhen(false)] out value) => Option.Success(property).Require(static p => p.ValueKind is JsonValueKind.String).Map(static p => p.GetString()).Branch(out value));
    public Result<byte[], DeserializationError> TryReadBytesValue() => TryGet<byte[]>("Byte[]", static (property, [MaybeNullWhen(false)] out value) => property.TryGetBytesFromBase64(out value));
    public Result<short, DeserializationError> TryReadInt16Value() => TryGet<short>("Int16", static (property, out value) => property.TryGetInt16(out value));
    public Result<ushort, DeserializationError> TryReadUInt16Value() => TryGet<ushort>("UInt16", static (property, out value) => property.TryGetUInt16(out value));
    public Result<int, DeserializationError> TryReadInt32Value() => TryGet<int>("Int32", static (property, out value) => property.TryGetInt32(out value));
    public Result<uint, DeserializationError> TryReadUInt32Value() => TryGet<uint>("UInt32", static (property, out value) => property.TryGetUInt32(out value));
    public Result<long, DeserializationError> TryReadInt64Value() => TryGet<long>("Int64", static (property, out value) => property.TryGetInt64(out value));
    public Result<ulong, DeserializationError> TryReadUInt64Value() => TryGet<ulong>("UInt64", static (property, out value) => property.TryGetUInt64(out value));
    public Result<Half, DeserializationError> TryReadHalfValue() => TryGet<float>("Half", static (property, out value) => property.TryGetSingle(out value)).Map(static f => (Half)f);
    public Result<float, DeserializationError> TryReadSingleValue() => TryGet<float>("Single", static (property, out value) => property.TryGetSingle(out value));
    public Result<double, DeserializationError> TryReadDoubleValue() => TryGet<double>("Double", static (property, out value) => property.TryGetDouble(out value));
    public Result<bool, DeserializationError> TryReadBooleanValue() => ConsumeCurrentElement().ValueKind switch { JsonValueKind.True => true, JsonValueKind.False => false, _ => DeserializationError.CreateInvalidPropertyType("<todo>", "Boolean") };
    public Result<DateTime, DeserializationError> TryReadDateTimeValue() => TryGet<DateTime>("DateTime", static (property, out value) => property.TryGetDateTime(out value));

    private delegate bool TryGetDelegate<T>(JsonElement element, [MaybeNullWhen(false)] out T result);
    private Result<T, DeserializationError> TryGet<T>(string type, TryGetDelegate<T> getter)
    {
        var current = ConsumeCurrentElement();
        if (getter(current, out var result))
        {
            return result;
        }
        return DeserializationError.CreateInvalidPropertyType("<todo>", type);
    }


    protected abstract JsonElement ConsumeCurrentElement();

    public IAmetrinReader ReadStartObject() => new AmetrinJsonObjectReader(ConsumeCurrentElement());
    public void ReadEndObject() { }

    public IAmetrinReader ReadStartArray(out int itemCount)
    {
        var current = ConsumeCurrentElement();
        itemCount = current.GetArrayLength();
        return new AmetrinJsonArrayReader(current);
    }
    public void ReadEndArray() { }

    public void Dispose() { }
}

public sealed class AmetrinJsonObjectReader(JsonElement objectElement) : AmetrinJsonReader
{
    private readonly JsonElement objectElement = objectElement.ValueKind is JsonValueKind.Object ? objectElement : throw new ArgumentException(nameof(objectElement));
    private JsonElement? currentElement = null;


    public override ErrorState<DeserializationError> TryReadPropertyName(ReadOnlySpan<char> name)
    {
        if (currentElement.HasValue) throw new InvalidOperationException();

        if (objectElement.TryGetProperty(name, out var prop))
        {
            currentElement = prop;
            return default;
        }
        return DeserializationError.CreatePropertyNotFound(name.ToString());
    }

    protected override JsonElement ConsumeCurrentElement()
    {
        if (currentElement is { } current)
        {
            currentElement = null;
            return current;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}

public sealed class AmetrinJsonArrayReader(JsonElement arrayElement) : AmetrinJsonReader
{
    private JsonElement.ArrayEnumerator array = arrayElement.ValueKind is JsonValueKind.Array ? arrayElement.EnumerateArray() : throw new ArgumentException(nameof(arrayElement));

    public override ErrorState<DeserializationError> TryReadPropertyName(ReadOnlySpan<char> name)
    {
        throw new InvalidOperationException();
    }

    protected override JsonElement ConsumeCurrentElement()
    {
        if (array.MoveNext())
        {
            return array.Current;
        }
        throw new IndexOutOfRangeException();
    }
}