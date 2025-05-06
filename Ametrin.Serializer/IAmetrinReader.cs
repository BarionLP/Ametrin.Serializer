using System.IO;
using System.Text.Json;

namespace Ametrin.Serializer;

public interface IAmetrinReader
{
    public string ReadStringProperty(string name);
    public int ReadInt32Property(string name);
    public bool ReadBooleanProperty(string name);
}

public sealed class AmetrinJsonReader(JsonDocument document) : IAmetrinReader
{
    private readonly JsonDocument document = document;

    public static AmetrinJsonReader Create(Stream stream) => new(JsonDocument.Parse(stream));

    public string ReadStringProperty(string name) => document.RootElement.GetProperty(name).GetString()!;
    public int ReadInt32Property(string name) => document.RootElement.GetProperty(name).GetInt32()!;
    public bool ReadBooleanProperty(string name) => document.RootElement.GetProperty(name).GetBoolean();
}
