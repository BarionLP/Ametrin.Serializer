using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Ametrin.Serializer.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SerializerAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor UnsupportedMemberType
        = new(id: "AS001", title: "Unsupported member type", messageFormat: "Unsupported member type! Set a custom converter or generate a serializer", category: "Usage", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidConverter
        = new(id: "AS002", title: "Invalid Converter", messageFormat: "Invalid Converter, all converters have to implement ISerializationConverter", category: "Usage", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);


    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [UnsupportedMemberType, InvalidConverter];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.RegisterSymbolAction(context =>
        {
            var attribute = context.Symbol.GetAttribute(IsSerializeAttribute);
            if (attribute is null) return;
            if (attribute.ConstructorArguments[0].IsNull)
            {
                var type = GetMemberType(context.Symbol);
                if (!IsSerializablePropertyType(type))
                {
                    context.ReportDiagnostic(Diagnostic.Create(UnsupportedMemberType, context.Symbol.Locations[0]));
                }
            }
            else if (!IsSerializationConverter((attribute.ConstructorArguments[0].Value as INamedTypeSymbol)!))
            {
                var syntax = attribute.ApplicationSyntaxReference!.GetSyntax(context.CancellationToken) as AttributeSyntax;
                var location = syntax!.ArgumentList!.Arguments[0].GetLocation();
                context.ReportDiagnostic(Diagnostic.Create(InvalidConverter, location));
            }
        }, SymbolKind.Field, SymbolKind.Property);
    }
}
