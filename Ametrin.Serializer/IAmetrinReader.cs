using System;
using Ametrin.Optional;

namespace Ametrin.Serializer;

public interface IAmetrinReader
{
    public Result<byte[], DeserializationError> TryReadBytesProperty(ReadOnlySpan<char> name);
    public Result<string, DeserializationError> TryReadStringProperty(ReadOnlySpan<char> name);
    public Result<int, DeserializationError> TryReadInt32Property(ReadOnlySpan<char> name);
    public Result<Half, DeserializationError> TryReadHalfProperty(ReadOnlySpan<char> name);
    public Result<float, DeserializationError> TryReadSingleProperty(ReadOnlySpan<char> name);
    public Result<double, DeserializationError> TryReadDoubleProperty(ReadOnlySpan<char> name);
    public Result<bool, DeserializationError> TryReadBooleanProperty(ReadOnlySpan<char> name);
    public Result<DateTime, DeserializationError> TryReadDateTimeProperty(ReadOnlySpan<char> name);
    public Result<T, DeserializationError> TryReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T>;

    public byte[] ReadBytesProperty(ReadOnlySpan<char> name) => TryReadBytesProperty(name).Or(static e => e.Throw<byte[]>());
    public string ReadStringProperty(ReadOnlySpan<char> name) => TryReadStringProperty(name).Or(static e => e.Throw<string>());
    public int ReadInt32Property(ReadOnlySpan<char> name) => TryReadInt32Property(name).Or(static e => e.Throw<int>());
    public Half ReadHalfProperty(ReadOnlySpan<char> name) => TryReadHalfProperty(name).Or(static e => e.Throw<Half>());
    public float ReadSingleProperty(ReadOnlySpan<char> name) => TryReadSingleProperty(name).Or(static e => e.Throw<float>());
    public double ReadDoubleProperty(ReadOnlySpan<char> name) => TryReadDoubleProperty(name).Or(static e => e.Throw<double>());
    public bool ReadBooleanProperty(ReadOnlySpan<char> name) => TryReadBooleanProperty(name).Or(static e => e.Throw<bool>());
    public DateTime ReadDateTimeProperty(ReadOnlySpan<char> name) => TryReadDateTimeProperty(name).Or(static e => e.Throw<DateTime>());
    public T ReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T> => TryReadObjectProperty<T>(name).Or(e => e.Throw<T>());

    public void ReadStartObject();
    public void ReadEndObject();
}
