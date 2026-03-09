using System;
using System.IO;
using System.Text;
using Ametrin.Optional;

namespace Ametrin.Serializer.Readers;

public sealed class AmetrinBinaryReader(Stream stream, bool leaveOpen = false) : IAmetrinReader
{
    private readonly BinaryReader reader = new(stream, Encoding.UTF8, leaveOpen);

    public ErrorState<DeserializationError> TryReadPropertyName(ReadOnlySpan<char> name) => default;

    public Result<byte[], DeserializationError> TryReadBytesValue()
    {
        ReadStartArray();
        var length = reader.ReadInt32();
        var bytes = reader.ReadBytes(length);
        ReadEndArray();
        return bytes;
    }

    public Result<string, DeserializationError> TryReadStringValue() => reader.ReadString();
    public Result<int, DeserializationError> TryReadInt32Value() => reader.ReadInt32();
    public Result<Half, DeserializationError> TryReadHalfValue() => reader.ReadHalf();
    public Result<float, DeserializationError> TryReadSingleValue() => reader.ReadSingle();
    public Result<double, DeserializationError> TryReadDoubleValue() => reader.ReadDouble();
    public Result<bool, DeserializationError> TryReadBooleanValue() => reader.ReadBoolean();
    public Result<DateTime, DeserializationError> TryReadDateTimeValue() => new DateTime(reader.ReadInt64());

    public IAmetrinReader ReadStartObject() => this;
    public void ReadEndObject() { }

    public void ReadStartArray() { }
    public void ReadEndArray() { }

    public void Dispose()
    {
        reader.Dispose();
    }
}
