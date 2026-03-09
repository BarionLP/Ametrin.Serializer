using System;
using Ametrin.Optional;

namespace Ametrin.Serializer;

public interface IAmetrinReader : IDisposable
{
    public ErrorState<DeserializationError> TryReadPropertyName(ReadOnlySpan<char> name);

    public Result<byte[], DeserializationError> TryReadBytesValue();
    public Result<string, DeserializationError> TryReadStringValue();
    public Result<int, DeserializationError> TryReadInt32Value();
    public Result<Half, DeserializationError> TryReadHalfValue();
    public Result<float, DeserializationError> TryReadSingleValue();
    public Result<double, DeserializationError> TryReadDoubleValue();
    public Result<bool, DeserializationError> TryReadBooleanValue();
    public Result<DateTime, DeserializationError> TryReadDateTimeValue();

    public IAmetrinReader ReadStartObject();
    public void ReadEndObject();

    public void ReadStartArray();
    public void ReadEndArray();
}

public static class AmetrinReaderExtensions
{
    extension(IAmetrinReader reader)
    {
        public Result<T, DeserializationError> TryReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T>
            => reader.TryReadProperty(name, reader.TryReadObjectValue<T>).MapError(name, static (e, name) => e with { PropertyName = $"{name}.{e.PropertyName}" });

        public Result<T, DeserializationError> TryReadObjectValue<T>() where T : IAmetrinSerializable<T>
        {
            using var sub = reader.ReadStartObject();
            var result = T.Deserialize(sub);
            reader.ReadEndObject();
            return result;
        }

        public Result<byte[], DeserializationError> TryReadBytesProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadBytesValue);
        public Result<string, DeserializationError> TryReadStringProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadStringValue);
        public Result<int, DeserializationError> TryReadInt32Property(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadInt32Value);
        public Result<Half, DeserializationError> TryReadHalfProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadHalfValue);
        public Result<float, DeserializationError> TryReadSingleProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadSingleValue);
        public Result<double, DeserializationError> TryReadDoubleProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadDoubleValue);
        public Result<bool, DeserializationError> TryReadBooleanProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadBooleanValue);
        public Result<DateTime, DeserializationError> TryReadDateTimeProperty(ReadOnlySpan<char> name) => reader.TryReadPropertyErrorAdjusted(name, reader.TryReadDateTimeValue);

        public void ReadPropertyName(ReadOnlySpan<char> name) => reader.TryReadPropertyName(name).Consume(error: static e => e.Throw());
        public byte[] ReadBytesProperty(ReadOnlySpan<char> name) => reader.TryReadBytesProperty(name).Or(static e => e.Throw<byte[]>());
        public string ReadStringProperty(ReadOnlySpan<char> name) => reader.TryReadStringProperty(name).Or(static e => e.Throw<string>());
        public int ReadInt32Property(ReadOnlySpan<char> name) => reader.TryReadInt32Property(name).Or(static e => e.Throw<int>());
        public Half ReadHalfProperty(ReadOnlySpan<char> name) => reader.TryReadHalfProperty(name).Or(static e => e.Throw<Half>());
        public float ReadSingleProperty(ReadOnlySpan<char> name) => reader.TryReadSingleProperty(name).Or(static e => e.Throw<float>());
        public double ReadDoubleProperty(ReadOnlySpan<char> name) => reader.TryReadDoubleProperty(name).Or(static e => e.Throw<double>());
        public bool ReadBooleanProperty(ReadOnlySpan<char> name) => reader.TryReadBooleanProperty(name).Or(static e => e.Throw<bool>());
        public DateTime ReadDateTimeProperty(ReadOnlySpan<char> name) => reader.TryReadDateTimeProperty(name).Or(static e => e.Throw<DateTime>());
        public T ReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T> => reader.TryReadObjectProperty<T>(name).Or(e => e.Throw<T>());

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
