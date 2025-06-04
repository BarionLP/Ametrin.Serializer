using System;
using System.IO;
using System.Text.Json;
using Ametrin.Optional;

namespace Ametrin.Serializer.Readers;

public sealed class AmetrinJsonReader(JsonElement element) : IAmetrinReader
{
    private readonly JsonElement element = element;

    public static AmetrinJsonReader Create(Stream stream) => new(JsonDocument.Parse(stream).RootElement);

    public Result<byte[], DeserializationError> TryReadBytesProperty(ReadOnlySpan<char> name) => element.TryGetProperty(name, out var property) ? property.TryGetBytesFromBase64(out var value) ? value : DeserializationError.CreateInvalidPropertyType(name.ToString(), "Byte[]") : DeserializationError.CreatePropertyNotFound(name.ToString());
    public Result<string, DeserializationError> TryReadStringProperty(ReadOnlySpan<char> name) => element.TryGetProperty(name, out var property) ? property.ValueKind is JsonValueKind.String ? property.GetString()! : DeserializationError.CreateInvalidPropertyType(name.ToString(), "String") : DeserializationError.CreatePropertyNotFound(name.ToString());
    public Result<int, DeserializationError> TryReadInt32Property(ReadOnlySpan<char> name) => element.TryGetProperty(name, out var property) ? property.TryGetInt32(out var value) ? value : DeserializationError.CreateInvalidPropertyType(name.ToString(), "Int32") : DeserializationError.CreatePropertyNotFound(name.ToString());
    public Result<float, DeserializationError> TryReadSingleProperty(ReadOnlySpan<char> name) => element.TryGetProperty(name, out var property) ? property.TryGetSingle(out var value) ? value : DeserializationError.CreateInvalidPropertyType(name.ToString(), "Single") : DeserializationError.CreatePropertyNotFound(name.ToString());
    public Result<double, DeserializationError> TryReadDoubleProperty(ReadOnlySpan<char> name) => element.TryGetProperty(name, out var property) ? property.TryGetDouble(out var value) ? value : DeserializationError.CreateInvalidPropertyType(name.ToString(), "Double") : DeserializationError.CreatePropertyNotFound(name.ToString());
    public Result<bool, DeserializationError> TryReadBooleanProperty(ReadOnlySpan<char> name) => element.TryGetProperty(name, out var property) ? property.ValueKind switch { JsonValueKind.True => true, JsonValueKind.False => false, _ => DeserializationError.CreateInvalidPropertyType(name.ToString(), "Boolean") } : DeserializationError.CreatePropertyNotFound(name.ToString());
    public Result<DateTime, DeserializationError> TryReadDateTimeProperty(ReadOnlySpan<char> name) => element.TryGetProperty(name, out var property) ? property.TryGetDateTime(out var value) ? value : DeserializationError.CreateInvalidPropertyType(name.ToString(), "DateTime") : DeserializationError.CreatePropertyNotFound(name.ToString());

    public Result<T, DeserializationError> TryReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T>
    {
        if (!element.TryGetProperty(name, out var property))
        {
            return DeserializationError.CreatePropertyNotFound(name.ToString());
        }
        var reader = new AmetrinJsonReader(property);
        var result = T.TryDeserialize(reader);
        if (OptionsMarshall.TryGetError(result, out var e))
        {
            return new DeserializationError(e.Type, $"{name}.{e.PropertyName}", e.ExpectedType);
        }
        return result;
    }

    public byte[] ReadBytesProperty(ReadOnlySpan<char> name) => element.GetProperty(name).GetBytesFromBase64();
    public string ReadStringProperty(ReadOnlySpan<char> name) => element.GetProperty(name).GetString()!;
    public int ReadInt32Property(ReadOnlySpan<char> name) => element.GetProperty(name).GetInt32();
    public float ReadSingleProperty(ReadOnlySpan<char> name) => element.GetProperty(name).GetSingle();
    public double ReadDoubleProperty(ReadOnlySpan<char> name) => element.GetProperty(name).GetDouble();
    public bool ReadBooleanProperty(ReadOnlySpan<char> name) => element.GetProperty(name).GetBoolean();
    public DateTime ReadDateTimeProperty(ReadOnlySpan<char> name) => element.GetProperty(name).GetDateTime();

    public T ReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T>
    {
        var reader = new AmetrinJsonReader(element.GetProperty(name));
        return T.Deserialize(reader);
    }

    public void ReadStartObject() { }
    public void ReadEndObject() { }
}
