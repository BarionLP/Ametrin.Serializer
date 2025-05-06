using System;
using System.IO;
using System.Text.Json;

namespace Ametrin.Serializer;

public interface IAmetrinWriter
{
    public void WriteStringProperty(string properyName, string value);
    public void WriteInt32Property(string properyName, int value);
    public void WriteBooleanProperty(string properyName, bool value);
    public void WriteStartObject();
    public void WriteEndObject();
}

public sealed class AmetrinJsonWriter(Stream stream, bool disposeStream = true) : IAmetrinWriter, IDisposable
{
    private readonly Utf8JsonWriter writer = new (stream);

    public void WriteInt32Property(string properyName, int value) => writer.WriteNumber(properyName, value);
    public void WriteStringProperty(string properyName, string value) => writer.WriteString(properyName, value);
    public void WriteBooleanProperty(string properyName, bool value) => writer.WriteBoolean(properyName, value);
    public void WriteStartObject() => writer.WriteStartObject();
    public void WriteEndObject() => writer.WriteEndObject();

    public void Dispose()
    {
        writer.Flush();
        if (disposeStream)
        {
            writer.Dispose();
        }
    }
}
