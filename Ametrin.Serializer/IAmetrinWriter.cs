using System;
using Ametrin.Optional;

namespace Ametrin.Serializer;

public interface IAmetrinWriter
{
    public void WritePropertyName(ReadOnlySpan<char> value);

    public void WriteBytesValue(ReadOnlySpan<byte> value);
    public void WriteStringValue(ReadOnlySpan<char> value);
    public void WriteInt32Value(int value);
    public void WriteHalfValue(Half value);
    public void WriteSingleValue(float value);
    public void WriteDoubleValue(double value);
    public void WriteBooleanValue(bool value);
    public void WriteDateTimeValue(DateTime value);

    public void WriteStartObject();
    public void WriteEndObject();

    public void WriteStartArray(int length);
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

        public void WriteInt32Property(ReadOnlySpan<char> propertyName, int value)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteInt32Value(value);
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

        public void WriteObjectValue<T>(T value) where T : IAmetrinSerializable<T>
        {
            writer.WriteStartObject();
            T.Serialize(value, writer);
            writer.WriteEndObject();
        }

        public void WriteObjectProperty<T>(ReadOnlySpan<char> propertyName, T value) where T : IAmetrinSerializable<T>
        {
            writer.WritePropertyName(propertyName);
            writer.WriteObjectValue(value);
        }

        public void WriteArrayValue<T>(ReadOnlySpan<T> values) where T : IAmetrinSerializable<T>
        {
            writer.WriteArrayValue(values, static (v, writer) => writer.WriteObjectValue(v));
        }

        public void WriteArrayValue<T>(ReadOnlySpan<T> values, Action<T, IAmetrinWriter> write)
        {
            writer.WriteStartArray(values.Length);
            foreach (var value in values)
            {
                write(value, writer);
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