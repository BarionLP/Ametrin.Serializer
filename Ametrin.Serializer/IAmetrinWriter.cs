using System;
using Ametrin.Optional;

namespace Ametrin.Serializer;

public interface IAmetrinWriter : IDisposable
{
    public void WritePropertyName(ReadOnlySpan<char> value);

    public void WriteBytesValue(ReadOnlySpan<byte> value);
    public void WriteStringValue(ReadOnlySpan<char> value);
    public void WriteInt16Value(short value);
    public void WriteUInt16Value(ushort value);
    public void WriteInt32Value(int value);
    public void WriteUInt32Value(uint value);
    public void WriteInt64Value(long value);
    public void WriteUInt64Value(ulong value);
    public void WriteHalfValue(Half value);
    public void WriteSingleValue(float value);
    public void WriteDoubleValue(double value);
    public void WriteBooleanValue(bool value);
    public void WriteDateTimeValue(DateTime value);

    public IAmetrinWriter WriteStartObject();
    public void WriteEndObject();

    public IAmetrinWriter WriteStartArray(int itemCount);
    public void WriteEndArray();
}

public static class AmetrinWriterExtensions
{
    extension(IAmetrinWriter writer)
    {
        public void WriteBytesProperty(ReadOnlySpan<char> propertyName, ReadOnlySpan<byte> value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteBytesValue(value);
        }

        public void WriteStringProperty(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteStringValue(value);
        }

        public void WriteInt16Property(ReadOnlySpan<char> propertyName, short value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteInt16Value(value);
        }

        public void WriteUInt16Property(ReadOnlySpan<char> propertyName, ushort value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteUInt16Value(value);
        }

        public void WriteInt32Property(ReadOnlySpan<char> propertyName, int value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteInt32Value(value);
        }

        public void WriteUInt32Property(ReadOnlySpan<char> propertyName, uint value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteUInt32Value(value);
        }

        public void WriteInt64Property(ReadOnlySpan<char> propertyName, long value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteInt64Value(value);
        }

        public void WriteUInt64Property(ReadOnlySpan<char> propertyName, ulong value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteUInt64Value(value);
        }

        public void WriteHalfProperty(ReadOnlySpan<char> propertyName, Half value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteHalfValue(value);
        }

        public void WriteSingleProperty(ReadOnlySpan<char> propertyName, float value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteSingleValue(value);
        }

        public void WriteDoubleProperty(ReadOnlySpan<char> propertyName, double value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteDoubleValue(value);
        }

        public void WriteBooleanProperty(ReadOnlySpan<char> propertyName, bool value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteBooleanValue(value);
        }

        public void WriteDateTimeProperty(ReadOnlySpan<char> propertyName, DateTime value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteDateTimeValue(value);
        }

        public void WriteProperty<T>(ReadOnlySpan<char> propertyName, T value) where T : ISerializationConverter<T>
        {
            writer.WritePropertyName(propertyName);
            T.WriteValue(writer, value);
        }

        public void WriteArrayValue<T>(ReadOnlySpan<T> values) where T : ISerializationConverter<T>
        {
            writer.WriteArrayValue(values, T.WriteValue);
        }

        public void WriteArrayValue<T>(ReadOnlySpan<T> values, Action<IAmetrinWriter, T> write)
        {
            writer.WriteStartArray(values.Length);
            foreach (var value in values)
            {
                write(writer, value);
            }
            writer.WriteEndArray();
        }

        // public void WriteArrayValue<T>(ReadOnlySpan<T> values, Func<T, IAmetrinWriter, ErrorState> write)
        // {
        //     writer.WriteStartArray(values.Length);
        //     foreach (var value in values)
        //     {
        //         write(value, writer);
        //     }
        //     writer.WriteEndArray();
        // }
    }
}