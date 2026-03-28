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
        writer.Write7BitEncodedInt(value.Length);
        writer.Write(value);
        WriteEndArray();
    }

    public void WriteStringValue(ReadOnlySpan<char> value)
    {
        writer.Write7BitEncodedInt(value.Length);
        writer.Write(value);
        WriteEndArray();
    }

    public void WriteInt16Value(short value) => writer.Write(value);
    public void WriteUInt16Value(ushort value) => writer.Write(value);
    public void WriteInt32Value(int value) => writer.Write(value);
    public void WriteUInt32Value(uint value) => writer.Write(value);
    public void WriteInt64Value(long value) => writer.Write(value);
    public void WriteUInt64Value(ulong value) => writer.Write(value);
    public void WriteHalfValue(Half value) => writer.Write(value);
    public void WriteSingleValue(float value) => writer.Write(value);
    public void WriteDoubleValue(double value) => writer.Write(value);
    public void WriteBooleanValue(bool value) => writer.Write(value);
    public void WriteDateTimeValue(DateTime value) => writer.Write(value.Ticks);

    public IAmetrinWriter WriteStartObject() => new AmetrinBinaryWriter(writer.BaseStream, leaveOpen: true);
    public void WriteEndObject() { }

    public IAmetrinWriter WriteStartArray(int length)
    {
        writer.Write7BitEncodedInt(length);
        return new AmetrinBinaryWriter(writer.BaseStream, leaveOpen: true);
    }
    
    public void WriteEndArray() { }

    public void Dispose()
    {
        writer.Flush();
        writer.Dispose();
    }
}
