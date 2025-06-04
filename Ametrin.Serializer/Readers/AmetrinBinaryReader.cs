using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Ametrin.Optional;

namespace Ametrin.Serializer.Readers;

public sealed class AmetrinBinaryReader(Stream stream, bool leaveOpen = false) : IAmetrinReader, IDisposable
{
    private readonly BinaryReader reader = new(stream, Encoding.UTF8, leaveOpen);

    public Result<byte[], DeserializationError> TryReadBytesProperty(ReadOnlySpan<char> name)
    {
        var length = reader.ReadInt32();
        return reader.ReadBytes(length);
    }

    public Result<string, DeserializationError> TryReadStringProperty(ReadOnlySpan<char> name) => reader.ReadString();
    public Result<int, DeserializationError> TryReadInt32Property(ReadOnlySpan<char> name) => reader.ReadInt32();
    public Result<float, DeserializationError> TryReadSingleProperty(ReadOnlySpan<char> name) => reader.ReadSingle();
    public Result<double, DeserializationError> TryReadDoubleProperty(ReadOnlySpan<char> name) => reader.ReadDouble();
    public Result<bool, DeserializationError> TryReadBooleanProperty(ReadOnlySpan<char> name) => reader.ReadBoolean();
    public Result<DateTime, DeserializationError> TryReadDateTimeProperty(ReadOnlySpan<char> name) => new DateTime(reader.ReadInt64());

    public Result<T, DeserializationError> TryReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T> => T.TryDeserialize(this);
    
    public byte[] ReadBytesProperty(ReadOnlySpan<char> name)
    {
        var length = reader.ReadInt32();
        return reader.ReadBytes(length);
    }

    public string ReadStringProperty(ReadOnlySpan<char> name) => reader.ReadString();
    public int ReadInt32Property(ReadOnlySpan<char> name) => reader.ReadInt32();
    public float ReadSingleProperty(ReadOnlySpan<char> name) => reader.ReadSingle();
    public double ReadDoubleProperty(ReadOnlySpan<char> name) => reader.ReadDouble();
    public bool ReadBooleanProperty(ReadOnlySpan<char> name) => reader.ReadBoolean();
    public DateTime ReadDateTimeProperty(ReadOnlySpan<char> name) => new(reader.ReadInt64());

    public T ReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T> => T.Deserialize(this);

    public void ReadStartObject() { }
    public void ReadEndObject() { }

    public void Dispose()
    {
        reader.Dispose();
    }
}
