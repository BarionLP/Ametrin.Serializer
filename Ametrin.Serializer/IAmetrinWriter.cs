using System;
using System.Collections.Frozen;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Ametrin.Serializer;

public interface IAmetrinWriter
{
    public void WriteBytesProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<byte> value);
    public void WriteStringProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<char> value);
    public void WriteInt32Property(ReadOnlySpan<char> properyName, int value);
    public void WriteSingleProperty(ReadOnlySpan<char> properyName, float value);
    public void WriteBooleanProperty(ReadOnlySpan<char> properyName, bool value);
    public void WriteDateTimeProperty(ReadOnlySpan<char> properyName, DateTime value);
    public void WriteObjectProperty<T>(ReadOnlySpan<char> properyName, T value) where T : IAmetrinSerializable<T>;
    public void WriteStartObject();
    public void WriteEndObject();
}

public sealed class AmetrinJsonWriter(Stream stream, bool leaveOpen = false) : IAmetrinWriter, IDisposable
{
    private readonly Utf8JsonWriter writer = new(stream);

    public void WriteBytesProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<byte> value) => writer.WriteBase64String(properyName, value);
    public void WriteStringProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<char> value) => writer.WriteString(properyName, value);
    public void WriteInt32Property(ReadOnlySpan<char> properyName, int value) => writer.WriteNumber(properyName, value);
    public void WriteSingleProperty(ReadOnlySpan<char> properyName, float value) => writer.WriteNumber(properyName, value);
    public void WriteBooleanProperty(ReadOnlySpan<char> properyName, bool value) => writer.WriteBoolean(properyName, value);
    public void WriteDateTimeProperty(ReadOnlySpan<char> properyName, DateTime value) => writer.WriteString(properyName, value);

    public void WriteObjectProperty<T>(ReadOnlySpan<char> properyName, T value) where T : IAmetrinSerializable<T>
    {
        if (value is null)
        {
            writer.WriteNull(properyName);
            return;
        }

        writer.WritePropertyName(properyName);
        WriteStartObject();
        T.Serialize(value, this);
        WriteEndObject();
    }

    public void WriteStartObject() => writer.WriteStartObject();
    public void WriteEndObject() => writer.WriteEndObject();

    public void Dispose()
    {
        writer.Flush();
        if (!leaveOpen)
        {
            writer.Dispose();
        }
    }
}

public sealed class AmetrinBinaryWriter(Stream stream, bool leaveOpen = false) : IAmetrinWriter, IDisposable
{
    private readonly BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen);

    public void WriteBytesProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<byte> value)
    {
        writer.Write(value.Length);
        writer.Write(value);
    }

    public void WriteStringProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<char> value)
    {
        writer.Write7BitEncodedInt(value.Length);
        writer.Write(value);
    }

    public void WriteInt32Property(ReadOnlySpan<char> properyName, int value) => writer.Write(value);
    public void WriteSingleProperty(ReadOnlySpan<char> properyName, float value) => writer.Write(value);
    public void WriteBooleanProperty(ReadOnlySpan<char> properyName, bool value) => writer.Write(value);
    public void WriteDateTimeProperty(ReadOnlySpan<char> properyName, DateTime value) => writer.Write(value.Ticks);

    public void WriteObjectProperty<T>(ReadOnlySpan<char> properyName, T value) where T : IAmetrinSerializable<T>
    {
        WriteStartObject();
        T.Serialize(value, this);
        WriteEndObject();
    }

    public void WriteStartObject() { }
    public void WriteEndObject() { }

    public void Dispose()
    {
        writer.Flush();
        writer.Dispose();
    }
}

public class EnumSerializer<TEnum> : ISerializationConverter<TEnum> where TEnum : struct, Enum
{
    private static readonly FrozenDictionary<string, TEnum> values = Enum.GetValues<TEnum>().ToFrozenDictionary(static v => v.ToString(), StringComparer.OrdinalIgnoreCase);

    public static void WriteProperty(IAmetrinWriter writer, ReadOnlySpan<char> name, TEnum value)
    {
        writer.WriteStringProperty(name, value.ToString());
    }

    public static TEnum ReadProperty(IAmetrinReader reader, ReadOnlySpan<char> name)
    {
        return values[reader.ReadStringProperty(name)];
    }
}