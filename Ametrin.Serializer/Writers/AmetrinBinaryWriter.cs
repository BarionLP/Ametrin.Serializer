using System;
using System.IO;
using System.Text;

namespace Ametrin.Serializer.Writers;

public sealed class AmetrinBinaryWriter(Stream stream, bool leaveOpen = false) : IAmetrinWriter, IDisposable
{
    private readonly BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen);

    public void WriteBytesProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<byte> value)
    {
        writer.Write(value.Length);
        writer.Write(value);
    }

    public void WriteStringProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<char> value)
    {
        writer.Write7BitEncodedInt(value.Length);
        writer.Write(value);
    }

    public void WriteInt32Property(ReadOnlySpan<char> properyName, int value) => writer.Write(value);
    public void WriteSingleProperty(ReadOnlySpan<char> properyName, float value) => writer.Write(value);
    public void WriteDoubleProperty(ReadOnlySpan<char> properyName, double value) => writer.Write(value);
    public void WriteBooleanProperty(ReadOnlySpan<char> properyName, bool value) => writer.Write(value);
    public void WriteDateTimeProperty(ReadOnlySpan<char> properyName, DateTime value) => writer.Write(value.Ticks);

    public void WriteObjectProperty<T>(ReadOnlySpan<char> properyName, T value) where T : IAmetrinSerializable<T>
    {
        WriteStartObject();
        T.Serialize(value, this);
        WriteEndObject();
    }

    public void WriteStartObject() { }
    public void WriteEndObject() { }

    public void Dispose()
    {
        writer.Flush();
        writer.Dispose();
    }
}
