using System.IO.Compression;
using System.Security.Cryptography;
using Ametrin.Serializer;

var value = new GeneratedSerialzer { Name = "Hello" };

var options = new AmetrinSerializationOptions
{
    CompressionLevel = CompressionLevel.NoCompression,
    Encryption = new()
    {
        Password = "ametrin"
    }
};

using var stream = new MemoryStream();

AmetrinSerializer.Serialize(stream, options, options);

stream.Seek(0, SeekOrigin.Begin);

using var peakingStream = new StreamReader(AmetrinSerializer.DecryptStream(stream, options.Encryption));
Console.WriteLine(peakingStream.ReadToEnd());

stream.Seek(0, SeekOrigin.Begin);
var newValue = AmetrinSerializer.Deserialize<AmetrinSerializationOptions>(stream, options);
Console.WriteLine(newValue.Encryption!.Password);


[GenerateSerializer()]
public sealed partial class GeneratedSerialzer
{
    [Serialize] public string Name { get; init; }
    [Serialize] public int Age;
    [Serialize] public bool Alive;
    [Serialize] public DateTime Created { get; } = DateTime.Now;
}