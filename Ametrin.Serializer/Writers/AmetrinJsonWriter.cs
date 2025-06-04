using System;
using System.IO;
using System.Text.Json;

namespace Ametrin.Serializer.Writers;

public sealed class AmetrinJsonWriter(Stream stream, bool leaveOpen = false) : IAmetrinWriter, IDisposable
{
    private readonly Utf8JsonWriter writer = new(stream);

    public void WriteBytesProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<byte> value) => writer.WriteBase64String(properyName, value);
    public void WriteStringProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<char> value) => writer.WriteString(properyName, value);
    public void WriteInt32Property(ReadOnlySpan<char> properyName, int value) => writer.WriteNumber(properyName, value);
    public void WriteSingleProperty(ReadOnlySpan<char> properyName, float value) => writer.WriteNumber(properyName, value);
    public void WriteDoubleProperty(ReadOnlySpan<char> properyName, double value) => writer.WriteNumber(properyName, value);
    public void WriteBooleanProperty(ReadOnlySpan<char> properyName, bool value) => writer.WriteBoolean(properyName, value);
    public void WriteDateTimeProperty(ReadOnlySpan<char> properyName, DateTime value) => writer.WriteString(properyName, value);

    public void WriteObjectProperty<T>(ReadOnlySpan<char> properyName, T value) where T : IAmetrinSerializable<T>
    {
        if (value is null)
        {
            writer.WriteNull(properyName);
            return;
        }

        writer.WritePropertyName(properyName);
        WriteStartObject();
        T.Serialize(value, this);
        WriteEndObject();
    }

    public void WriteStartObject() => writer.WriteStartObject();
    public void WriteEndObject() => writer.WriteEndObject();

    public void Dispose()
    {
        writer.Flush();
        if (!leaveOpen)
        {
            writer.Dispose();
        }
    }
}
