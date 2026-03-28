using System;
using Ametrin.Optional;

namespace Ametrin.Serializer;

public interface IAmetrinReader : IDisposable
{
    public ErrorState<DeserializationError> TryReadPropertyName(ReadOnlySpan<char> name);

    public Result<byte[], DeserializationError> TryReadBytesValue();
    public Result<string, DeserializationError> TryReadStringValue();
    public Result<short, DeserializationError> TryReadInt16Value();
    public Result<ushort, DeserializationError> TryReadUInt16Value();
    public Result<int, DeserializationError> TryReadInt32Value();
    public Result<uint, DeserializationError> TryReadUInt32Value();
    public Result<long, DeserializationError> TryReadInt64Value();
    public Result<ulong, DeserializationError> TryReadUInt64Value();
    public Result<Half, DeserializationError> TryReadHalfValue();
    public Result<float, DeserializationError> TryReadSingleValue();
    public Result<double, DeserializationError> TryReadDoubleValue();
    public Result<bool, DeserializationError> TryReadBooleanValue();
    public Result<DateTime, DeserializationError> TryReadDateTimeValue();

    public IAmetrinReader ReadStartObject();
    public void ReadEndObject();

    public IAmetrinReader ReadStartArray(out int itemCount);
    public void ReadEndArray();
}

public static class AmetrinReaderExtensions
{
    extension(IAmetrinReader reader)
    {
        public Result<T, DeserializationError> TryReadObjectProperty<T>(ReadOnlySpan<char> name) where T : ISerializationConverter<T>
            => reader.TryReadProperty(name, reader.TryReadObjectValue<T>).MapError(name, static (e, name) => e with { PropertyName = $"{name}.{e.PropertyName}" });

        public Result<T, DeserializationError> TryReadObjectValue<T>() where T : ISerializationConverter<T> 
            => reader.TryReadObjectValue(T.TryReadValue);

        public T ReadObjectValue<T>(Func<IAmetrinReader, Result<T, DeserializationError>> factory) => reader.TryReadObjectValue(factory).Or(static e => e.Throw<T>());
        public Result<T, DeserializationError> TryReadObjectValue<T>(Func<IAmetrinReader, Result<T, DeserializationError>> factory)
        {
            using var objectReader = reader.ReadStartObject();
            var result = factory(objectReader);
            reader.ReadEndObject();
            return result;
        }

        public Result<T[], DeserializationError> TryReadArrayValue<T>(Func<IAmetrinReader, Result<T, DeserializationError>> read)
        {
            using var arrayReader = reader.ReadStartArray(out var length);
            var items = new T[length];
            for (var i = 0; i < length; i++)
            {
                if (read(arrayReader).Branch(out var result, out var error))
                {
                    items[i] = result;
                }
                else
                {
                    return error;
                }
            }
            reader.ReadEndArray();
            return items;
        }

        public Result<byte[], DeserializationError> TryReadBytesProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadBytesValue);
        public Result<string, DeserializationError> TryReadStringProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadStringValue);
        public Result<short, DeserializationError> TryReadInt16Property(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadInt16Value);
        public Result<ushort, DeserializationError> TryReadUInt16Property(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadUInt16Value);
        public Result<int, DeserializationError> TryReadInt32Property(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadInt32Value);
        public Result<uint, DeserializationError> TryReadUInt32Property(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadUInt32Value);
        public Result<long, DeserializationError> TryReadInt64Property(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadInt64Value);
        public Result<ulong, DeserializationError> TryReadUInt64Property(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadUInt64Value);
        public Result<Half, DeserializationError> TryReadHalfProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadHalfValue);
        public Result<float, DeserializationError> TryReadSingleProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadSingleValue);
        public Result<double, DeserializationError> TryReadDoubleProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadDoubleValue);
        public Result<bool, DeserializationError> TryReadBooleanProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadBooleanValue);
        public Result<DateTime, DeserializationError> TryReadDateTimeProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadDateTimeValue);

        public void ReadPropertyName(ReadOnlySpan<char> name) => reader.TryReadPropertyName(name).Consume(error: static e => e.Throw());
        public byte[] ReadBytesProperty(ReadOnlySpan<char> name) => reader.TryReadBytesProperty(name).Or(static e => e.Throw<byte[]>());
        public string ReadStringProperty(ReadOnlySpan<char> name) => reader.TryReadStringProperty(name).Or(static e => e.Throw<string>());
        public short ReadInt16Property(ReadOnlySpan<char> name) => reader.TryReadInt16Property(name).Or(static e => e.Throw<short>());
        public ushort ReadUInt16Property(ReadOnlySpan<char> name) => reader.TryReadUInt16Property(name).Or(static e => e.Throw<ushort>());
        public int ReadInt32Property(ReadOnlySpan<char> name) => reader.TryReadInt32Property(name).Or(static e => e.Throw<int>());
        public uint ReadUInt32Property(ReadOnlySpan<char> name) => reader.TryReadUInt32Property(name).Or(static e => e.Throw<uint>());
        public long ReadInt64Property(ReadOnlySpan<char> name) => reader.TryReadInt64Property(name).Or(static e => e.Throw<long>());
        public ulong ReadUInt64Property(ReadOnlySpan<char> name) => reader.TryReadUInt64Property(name).Or(static e => e.Throw<ulong>());
        public Half ReadHalfProperty(ReadOnlySpan<char> name) => reader.TryReadHalfProperty(name).Or(static e => e.Throw<Half>());
        public float ReadSingleProperty(ReadOnlySpan<char> name) => reader.TryReadSingleProperty(name).Or(static e => e.Throw<float>());
        public double ReadDoubleProperty(ReadOnlySpan<char> name) => reader.TryReadDoubleProperty(name).Or(static e => e.Throw<double>());
        public bool ReadBooleanProperty(ReadOnlySpan<char> name) => reader.TryReadBooleanProperty(name).Or(static e => e.Throw<bool>());
        public DateTime ReadDateTimeProperty(ReadOnlySpan<char> name) => reader.TryReadDateTimeProperty(name).Or(static e => e.Throw<DateTime>());
        public T ReadObjectProperty<T>(ReadOnlySpan<char> name) where T : ISerializationConverter<T> => reader.TryReadObjectProperty<T>(name).Or(e => e.Throw<T>());

        private Result<T, DeserializationError> TryReadPropertyErrorAdjusted<T>(ReadOnlySpan<char> name, Func<Result<T, DeserializationError>> getter)
        {
            return reader.TryReadPropertyName(name).Branch(out var error) ? getter().MapError(name, static (e, name) => e with { PropertyName = name.ToString() }) : error;
        }
        private Result<T, DeserializationError> TryReadProperty<T>(ReadOnlySpan<char> name, Func<Result<T, DeserializationError>> getter)
        {
            return reader.TryReadPropertyName(name).Branch(out var error) ? getter() : error;
        }
    }

}
