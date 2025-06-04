using System.IO.Compression;
using Ametrin.Optional;
using Ametrin.Serializer;
using Ametrin.Serializer.Sample;
using BenchmarkDotNet.Running;

// var value = new GeneratedSerialzer { Name = "Hello" };
var value = new BenchmarkData();

var options = new AmetrinSerializationOptions
{
    CompressionLevel = CompressionLevel.NoCompression,
    Encryption = new()
    {
        Password = "ametrin"
    }
};

using var stream = new MemoryStream();

AmetrinSerializer.Serialize(stream, value, options);

stream.Seek(0, SeekOrigin.Begin);

using var peakingStream = new StreamReader(AmetrinSerializer.DecryptStream(stream, options.Encryption));
Console.WriteLine(peakingStream.ReadToEnd());

stream.Seek(0, SeekOrigin.Begin);
var newValue = AmetrinSerializer.Deserialize<BenchmarkData>(stream, options);

// BenchmarkRunner.Run<Benchmarks>();


[GenerateSerializer()]
public sealed partial class GeneratedSerializer
{
    [Serialize] public string Name { get; init; }
    [Serialize] public int Age;
    [Serialize] public bool Alive;
    [Serialize] public DateTime Created { get; } = DateTime.Now;
}