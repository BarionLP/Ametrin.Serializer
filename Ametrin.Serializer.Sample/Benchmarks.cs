using System.IO.Compression;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace Ametrin.Serializer.Sample;

[MemoryDiagnoser(true)]
public class Benchmarks
{
    private AmetrinSerializationOptions OptionsNone = new();

    private AmetrinSerializationOptions OptionsCompression = new()
    {
        CompressionLevel = CompressionLevel.Optimal,
    };

    private AmetrinSerializationOptions OptionsEncrption = new()
    {
        Encryption = new()
        {
            Password = "Am3tr!n"
        },
    };

    private AmetrinSerializationOptions OptionsBoth = new()
    {
        CompressionLevel = CompressionLevel.Optimal,
        Encryption = new()
        {
            Password = "Am3tr!n"
        },
    };

    private readonly BenchmarkData data = new()
    {

    };

    private readonly MemoryStream stream = new();

    [IterationSetup]
    public void IterationSetup()
    {
        stream.Seek(0, SeekOrigin.Begin);
    }

    [Benchmark(Baseline = true)]
    public void System_Text_Json()
    {
        JsonSerializer.Serialize(stream, data);
    }

    [Benchmark]
    public void Ametrin_None()
    {
        AmetrinSerializer.Serialize(stream, data);
    }

    [Benchmark]
    public void Ametrin_Compressed()
    {
        AmetrinSerializer.Serialize(stream, data, OptionsCompression);
    }

    [Benchmark]
    public void Ametrin_Encypted()
    {
        AmetrinSerializer.Serialize(stream, data, OptionsEncrption);
    }

    [Benchmark]
    public void Ametrin_Both()
    {
        AmetrinSerializer.Serialize(stream, data, OptionsBoth);
    }
}

[GenerateSerializer(AllProperties: true)]
internal sealed partial class BenchmarkData
{
    private float pi { get; } = float.Pi;
    private double pi2 { get; } = double.Pi;
    private string lorem { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private int value { get; } = int.MaxValue;
    private int value2 { get; } = int.MinValue;
    public Test t1 { get; } = Test.c;
    [Serialize(Converter: typeof(EnumSerializer<Test>))] public Test t2 = Test.v;
    private string lorem1 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem2 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem3 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem4 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem5 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem6 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem7 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem8 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem9 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem10 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem11 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem12 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem13 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem14 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem15 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem16 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem17 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem18 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem19 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    public enum Test { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z }
}
