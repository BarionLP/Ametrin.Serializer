using System;
using Ametrin.Optional;

namespace Ametrin.Serializer;

public interface IAmetrinReader
{
    public Result<byte[], DeserializationError> TryReadBytesProperty(ReadOnlySpan<char> name);
    public Result<string, DeserializationError> TryReadStringProperty(ReadOnlySpan<char> name);
    public Result<int, DeserializationError> TryReadInt32Property(ReadOnlySpan<char> name);
    public Result<float, DeserializationError> TryReadSingleProperty(ReadOnlySpan<char> name);
    public Result<double, DeserializationError> TryReadDoubleProperty(ReadOnlySpan<char> name);
    public Result<bool, DeserializationError> TryReadBooleanProperty(ReadOnlySpan<char> name);
    public Result<DateTime, DeserializationError> TryReadDateTimeProperty(ReadOnlySpan<char> name);
    public Result<T, DeserializationError> TryReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T>;

    public byte[] ReadBytesProperty(ReadOnlySpan<char> name);
    public string ReadStringProperty(ReadOnlySpan<char> name);
    public int ReadInt32Property(ReadOnlySpan<char> name);
    public float ReadSingleProperty(ReadOnlySpan<char> name);
    public double ReadDoubleProperty(ReadOnlySpan<char> name);
    public bool ReadBooleanProperty(ReadOnlySpan<char> name);
    public DateTime ReadDateTimeProperty(ReadOnlySpan<char> name);
    public T ReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T>;

    public void ReadStartObject();
    public void ReadEndObject();
}
