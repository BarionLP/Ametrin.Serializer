using System;

namespace Ametrin.Serializer;

public interface IAmetrinWriter
{
    public void WriteBytesProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<byte> value);
    public void WriteStringProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<char> value);
    public void WriteInt32Property(ReadOnlySpan<char> properyName, int value);
    public void WriteSingleProperty(ReadOnlySpan<char> properyName, float value);
    public void WriteDoubleProperty(ReadOnlySpan<char> properyName, double value);
    public void WriteBooleanProperty(ReadOnlySpan<char> properyName, bool value);
    public void WriteDateTimeProperty(ReadOnlySpan<char> properyName, DateTime value);
    public void WriteObjectProperty<T>(ReadOnlySpan<char> properyName, T value) where T : IAmetrinSerializable<T>;
    public void WriteStartObject();
    public void WriteEndObject();
}
