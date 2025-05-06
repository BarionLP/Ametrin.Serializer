using System.Text.Json;
using Ametrin.Serializer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var value = new GeneratedSerialzer { Name = "Hello" };

using var stream = new MemoryStream();
var writer = new AmetrinJsonWriter(stream, disposeStream: false);

writer.WriteStartObject();
GeneratedSerialzer.Serialize(value, writer);
writer.WriteEndObject();

writer.Dispose();

stream.Seek(0, SeekOrigin.Begin);
var reader = AmetrinJsonReader.Create(stream);

GeneratedSerialzer.Deserialize(reader);



[GenerateSerializer]
public sealed partial class GeneratedSerialzer
{
    [Serialize] public string Name { get; init; }
    [Serialize] public int Age;
    [Serialize] public bool Alive;
}

public sealed class ManualSerializer : IAmetrinSerializable<ManualSerializer>
{
    public string Name { get; init; }
    public int Age;

    public static ManualSerializer Deserialize(IAmetrinReader reader)
    {
        return new()
        {
            Name = reader.ReadStringProperty(nameof(Name)),
            Age = reader.ReadInt32Property(nameof(Age)),
        };
    }

    public static void Serialize(ManualSerializer self, IAmetrinWriter writer)
    {
        writer.WriteStringProperty(nameof(self.Name), self.Name);
        writer.WriteInt32Property(nameof(self.Age), self.Age);
    }
}