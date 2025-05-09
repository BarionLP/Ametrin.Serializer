namespace Ametrin.Serializer.Generator;

internal static class Helper
{
    internal static bool IsSerializablePropertyType(ITypeSymbol typeSymbol)
    {
        return IsTypeSupportedByWriter(typeSymbol) || typeSymbol.TypeKind is TypeKind.Enum || typeSymbol.HasAttribute(IsGenerateSerializerAttribute);
    }

    internal static bool IsTypeSupportedByWriter(ITypeSymbol type) => type.SpecialType is SpecialType.System_String or SpecialType.System_Int32 or SpecialType.System_Single or SpecialType.System_Boolean or SpecialType.System_DateTime;

    internal static bool IsGenerateSerializerAttribute(INamedTypeSymbol attribute) => attribute is { Name: "GenerateSerializerAttribute", ContainingAssembly.Name: "Ametrin.Serializer" };
    internal static bool IsSerializeAttribute(INamedTypeSymbol attribute) => attribute is { Name: "SerializeAttribute", ContainingAssembly.Name: "Ametrin.Serializer" };
    internal static bool IsSerializationConverter(ITypeSymbol type) => type.AllInterfaces.Any(i => i is { Name: "ISerializationConverter" });
    internal static ITypeSymbol GetMemberType(ISymbol member) => member switch
    {
        IPropertySymbol property => property.Type,
        IFieldSymbol property => property.Type,
        _ => throw new InvalidOperationException($"Tried to deserialize a {member}"),
    };
}
