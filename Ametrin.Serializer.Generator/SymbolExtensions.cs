namespace Ametrin.Serializer.Generator;

public static class SymbolExtensions
{
    public static bool HasAttribute(this ISymbol symbol, Func<INamedTypeSymbol, bool> condition)
        => symbol.GetAttributes().Any(attributeData => attributeData.AttributeClass is not null && condition(attributeData.AttributeClass));
    public static AttributeData? GetAttribute(this ISymbol symbol, Func<INamedTypeSymbol, bool> condition)
        => symbol.GetAttributes().FirstOrDefault(attributeData => attributeData.AttributeClass is not null && condition(attributeData.AttributeClass));
}
