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
        var length = reader.Read7BitEncodedInt();
        return reader.ReadBytes(length);
    }

    public Result<string, DeserializationError> TryReadStringValue()
    {
        var length = reader.Read7BitEncodedInt();
        return new string(reader.ReadChars(length));
    }

    public Result<short, DeserializationError> TryReadInt16Value() => reader.ReadInt16();
    public Result<ushort, DeserializationError> TryReadUInt16Value() => reader.ReadUInt16();
    public Result<int, DeserializationError> TryReadInt32Value() => reader.ReadInt32();
    public Result<uint, DeserializationError> TryReadUInt32Value() => reader.ReadUInt32();
    public Result<long, DeserializationError> TryReadInt64Value() => reader.ReadInt64();
    public Result<ulong, DeserializationError> TryReadUInt64Value() => reader.ReadUInt64();
    public Result<Half, DeserializationError> TryReadHalfValue() => reader.ReadHalf();
    public Result<float, DeserializationError> TryReadSingleValue() => reader.ReadSingle();
    public Result<double, DeserializationError> TryReadDoubleValue() => reader.ReadDouble();
    public Result<bool, DeserializationError> TryReadBooleanValue() => reader.ReadBoolean();
    public Result<DateTime, DeserializationError> TryReadDateTimeValue() => new DateTime(reader.ReadInt64());

    public IAmetrinReader ReadStartObject() => new AmetrinBinaryReader(reader.BaseStream, leaveOpen: true);
    public void ReadEndObject() { }

    public IAmetrinReader ReadStartArray(out int itemCount)
    {
        itemCount = reader.Read7BitEncodedInt();
        return new AmetrinBinaryReader(reader.BaseStream, leaveOpen: true);
    }
    public void ReadEndArray() { }

    public void Dispose()
    {
        reader.Dispose();
    }
}
