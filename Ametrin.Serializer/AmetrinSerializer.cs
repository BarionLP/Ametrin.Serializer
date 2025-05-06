using System;

namespace Ametrin.Serializer;

public static class AmetrinSerializer
{

}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class GenerateSerializerAttribute : Attribute;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SerializeAttribute : Attribute;