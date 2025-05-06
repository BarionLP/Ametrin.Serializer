using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Ametrin.Serializer;

public static class AmetrinSerializer
{
    public static AmetrinSerializationOptions DefaultOptions { get; } = new AmetrinSerializationOptions
    {

    };

    public static void Serialize<T>(Stream output, T value, AmetrinSerializationOptions? options = null) where T : IAmetrinSerializable<T>
    {
        options ??= DefaultOptions;

        using var encryptionStream = options.Encryption is null ? null : EncryptStream(output, options.Encryption);
        using var compressionStream = options.CompressionLevel is CompressionLevel.NoCompression ? null : CompressStream(encryptionStream ?? output, options.CompressionLevel);

        using var writer = new AmetrinJsonWriter(compressionStream ?? encryptionStream ?? output, leaveOpen: true);
        // using var writer = new AmetrinBinaryWriter(compressionStream ?? encryptionStream ?? output, leaveOpen: true);

        writer.WriteStartObject();
        T.Serialize(value, writer);
        writer.WriteEndObject();
    }

    public static T Deserialize<T>(Stream input, AmetrinSerializationOptions? options = null) where T : IAmetrinSerializable<T>
    {
        options ??= DefaultOptions;

        using var decryptionStream = options.Encryption is null ? null : DecryptStream(input, options.Encryption);
        using var decompressionStream = options.CompressionLevel is CompressionLevel.NoCompression ? null : DecompressStream(decryptionStream ?? input);

        var reader = AmetrinJsonReader.Create(decompressionStream ?? decryptionStream ?? input);
        // using var reader = new AmetrinBinaryReader(decompressionStream ?? decryptionStream ?? input, leaveOpen: true);

        return T.Deserialize(reader);
    }

    public static Stream EncryptStream(Stream stream, AmetrinSerializationOptions.EncryptionOptions options)
    {
        using var derive = new Rfc2898DeriveBytes(options.Password, options.SaltSize, options.Iterations, options.HashAlgorithm);
        stream.Write(derive.Salt);
        using var aes = Aes.Create();
        aes.Key = derive.GetBytes(aes.KeySize / 8);
        aes.IV = derive.GetBytes(aes.BlockSize / 8);
        return new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write, leaveOpen: true);
    }

    public static Stream DecryptStream(Stream stream, AmetrinSerializationOptions.EncryptionOptions options)
    {
        var salt = new byte[options.SaltSize];
        stream.ReadExactly(salt);
        using var derive = new Rfc2898DeriveBytes(options.Password, salt, options.Iterations, options.HashAlgorithm);
        using var aes = Aes.Create();
        aes.Key = derive.GetBytes(aes.KeySize / 8);
        aes.IV = derive.GetBytes(aes.BlockSize / 8);
        return new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read, leaveOpen: true);
    }

    public static Stream CompressStream(Stream stream, CompressionLevel compressionLevel)
    {
        return new ZLibStream(stream, compressionLevel, leaveOpen: true);
    }

    public static Stream DecompressStream(Stream stream)
    {
        return new ZLibStream(stream, CompressionMode.Decompress, leaveOpen: true);
    }


    private static readonly Dictionary<string, Func<IAmetrinReader, object>> knownTypes = [];

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void RegisterSerializer<T>(string name) where T : ITypedAmetrinSerializable<T>
    {
        Debug.Assert(!knownTypes.ContainsKey(name));

        knownTypes[name] = static reader => T.Deserialize(reader);
    }

    public static T DeserializeDynamic<T>(Stream input, AmetrinSerializationOptions? options = null) => (T) DeserializeDynamic(input, options);
    public static object DeserializeDynamic(Stream input, AmetrinSerializationOptions? options = null)
    {
        options ??= DefaultOptions;

        using var decryptionStream = options.Encryption is null ? null : DecryptStream(input, options.Encryption);
        using var decompressionStream = options.CompressionLevel is CompressionLevel.NoCompression ? null : DecompressStream(decryptionStream ?? input);

        var reader = AmetrinJsonReader.Create(decompressionStream ?? decryptionStream ?? input);
        var type = reader.ReadStringProperty("$type");
        if (knownTypes.TryGetValue(type, out var supplier))
        {
            return supplier(reader);
        }

        throw new InvalidOperationException($"Unkown type {type}");
    }
}

[GenerateSerializer]
public sealed partial class AmetrinSerializationOptions
{
    [Serialize(Converter: typeof(EnumSerializer<CompressionLevel>))] public CompressionLevel CompressionLevel { get; init; } = CompressionLevel.NoCompression;
    [Serialize] public EncryptionOptions? Encryption { get; init; } = null;

    [GenerateSerializer]
    public sealed partial class EncryptionOptions
    {
        [Serialize] public required string Password { get; init; }
        [Serialize] public int SaltSize { get; init; } = 12;
        [Serialize] public int Iterations { get; init; } = 10_000;
        [Serialize(Converter: typeof(HashAlgorithmSerializer))] public HashAlgorithmName HashAlgorithm { get; init; } = HashAlgorithmName.SHA3_256;
    }
}

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class GenerateSerializerAttribute(bool SerializeTypeName = false, string? TypeName = null) : Attribute;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SerializeAttribute(Type? Converter = null) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.

public sealed class HashAlgorithmSerializer : ISerializationConverter<HashAlgorithmName>
{
    public static HashAlgorithmName ReadProperty(IAmetrinReader reader, ReadOnlySpan<char> name)
    {
        return new HashAlgorithmName(reader.ReadStringProperty(name));
    }

    public static void WriteProperty(IAmetrinWriter writer, ReadOnlySpan<char> name, HashAlgorithmName value)
    {
        writer.WriteStringProperty(name, value.Name);
    }
}