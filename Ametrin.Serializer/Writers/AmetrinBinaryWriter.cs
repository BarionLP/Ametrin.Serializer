using System;
using System.IO;
using System.Text;

namespace Ametrin.Serializer.Writers;

public sealed class AmetrinBinaryWriter(Stream stream, bool leaveOpen = false) : IAmetrinWriter, IDisposable
{
    private readonly BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen);

    public void WritePropertyName(ReadOnlySpan<char> propertyName) { }
    
    public void WriteBytesValue(ReadOnlySpan<byte> value)
    {
        WriteStartArray(value.Length);
        writer.Write(value);
        WriteEndArray();
    }

    public void WriteStringValue(ReadOnlySpan<char> value)
    {
        WriteStartArray(value.Length);
        writer.Write(value);
        WriteEndArray();
    }

    public void WriteInt32Value(int value) => writer.Write(value);
    public void WriteHalfValue(Half value) => writer.Write(value);
    public void WriteSingleValue(float value) => writer.Write(value);
    public void WriteDoubleValue(double value) => writer.Write(value);
    public void WriteBooleanValue(bool value) => writer.Write(value);
    public void WriteDateTimeValue(DateTime value) => writer.Write(value.Ticks);

    public void WriteStartObject() { }
    public void WriteEndObject() { }

    public void WriteStartArray(int length)
    {
        WriteInt32Value(length);
    }
    public void WriteEndArray() { }

    public void Dispose()
    {
        writer.Flush();
        writer.Dispose();
    }

}
