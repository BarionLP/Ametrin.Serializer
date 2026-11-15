using System.IO.Compression;
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
using var writer = AmetrinSerializer.CreateJsonSerializer(stream, options);

BenchmarkData.WriteValue(writer, value);

stream.Seek(0, SeekOrigin.Begin);

using var peakingStream = new StreamReader(AmetrinSerializer.DecryptStream(stream, options.Encryption));
Console.WriteLine(peakingStream.ReadToEnd());

stream.Seek(0, SeekOrigin.Begin);
var newValue = AmetrinSerializer.TryDeserialize<BenchmarkData>(stream, options).OrThrow();

BenchmarkRunner.Run<Benchmarks>();

[GenerateSerializer]
public sealed partial class GeneratedSerializer
{
    [Serialize] public string Name { get; init; }
    [Serialize] public int Age;
    [Serialize] public bool Alive;
    [Serialize] public DateTime Created { get; } = DateTime.Now;
}

[GenerateSerializer(AllProperties: true)]
public partial record struct TestRecord(Guid ID, string Name, GeneratedSerializer Nested);