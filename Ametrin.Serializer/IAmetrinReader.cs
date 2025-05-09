using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Ametrin.Serializer;

public interface IAmetrinReader
{
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

public sealed class AmetrinJsonReader(JsonElement document) : IAmetrinReader
{
    private readonly JsonElement document = document;

    public static AmetrinJsonReader Create(Stream stream) => new(JsonDocument.Parse(stream).RootElement);

    public byte[] ReadBytesProperty(ReadOnlySpan<char> name) => document.GetProperty(name).GetBytesFromBase64();
    public string ReadStringProperty(ReadOnlySpan<char> name) => document.GetProperty(name).GetString()!;
    public int ReadInt32Property(ReadOnlySpan<char> name) => document.GetProperty(name).GetInt32();
    public float ReadSingleProperty(ReadOnlySpan<char> name) => document.GetProperty(name).GetSingle();
    public double ReadDoubleProperty(ReadOnlySpan<char> name) => document.GetProperty(name).GetDouble();
    public bool ReadBooleanProperty(ReadOnlySpan<char> name) => document.GetProperty(name).GetBoolean();
    public DateTime ReadDateTimeProperty(ReadOnlySpan<char> name) => document.GetProperty(name).GetDateTime();

    public T ReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T>
    {
        var reader = new AmetrinJsonReader(document.GetProperty(name));
        return T.Deserialize(reader);
    }

    public void ReadStartObject() { }
    public void ReadEndObject() { }
}

public sealed class AmetrinBinaryReader(Stream stream, bool leaveOpen = false) : IAmetrinReader, IDisposable
{
    private readonly BinaryReader reader = new(stream, Encoding.UTF8, leaveOpen);

    public static AmetrinJsonReader Create(Stream stream) => new(JsonDocument.Parse(stream).RootElement);

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
