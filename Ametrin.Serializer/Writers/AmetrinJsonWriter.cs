using System;
using System.IO;
using System.Text.Json;

namespace Ametrin.Serializer.Writers;

public sealed class AmetrinJsonWriter(Stream stream, bool leaveOpen = false) : IAmetrinWriter, IDisposable
{
    private readonly Utf8JsonWriter writer = new(stream);

    public void WritePropertyName(ReadOnlySpan<char> name) => writer.WritePropertyName(name);
    
    public void WriteBytesValue(ReadOnlySpan<byte> value) => writer.WriteBase64StringValue(value);
    public void WriteStringValue(ReadOnlySpan<char> value) => writer.WriteStringValue(value);
    public void WriteInt32Value(int value) => writer.WriteNumberValue(value);
    public void WriteHalfValue(Half value) => writer.WriteNumberValue((float)value);
    public void WriteSingleValue(float value) => writer.WriteNumberValue(value);
    public void WriteDoubleValue(double value) => writer.WriteNumberValue(value);
    public void WriteBooleanValue(bool value) => writer.WriteBooleanValue(value);
    public void WriteDateTimeValue(DateTime value) => writer.WriteStringValue(value);

    public void WriteStartObject() => writer.WriteStartObject();
    public void WriteEndObject() => writer.WriteEndObject();

    public void WriteStartArray(int length) => writer.WriteStartArray();
    public void WriteEndArray() => writer.WriteEndArray();

    public void Dispose()
    {
        writer.Flush();
        if (!leaveOpen)
        {
            writer.Dispose();
        }
    }
}
