using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Ametrin.Serializer.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SerializerAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor NonVoidReturn
        = new(id: "AS001", title: "Unsupported member type", messageFormat: "Unsupported member type! Make sure you have your own serializer registered", category: "Usage", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);


    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [NonVoidReturn];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.RegisterSymbolAction(context =>
        {
            var attribute = context.Symbol.GetAttribute(IsSerializeAttribute);
            if (attribute is not null && attribute.ConstructorArguments[0].IsNull)
            {
                var type = GetMemberType(context.Symbol);
                if (!IsSerializablePropertyType(type))
                {
                    context.ReportDiagnostic(Diagnostic.Create(NonVoidReturn, context.Symbol.Locations[0]));
                }
            }
        }, SymbolKind.Field, SymbolKind.Property);
    }
}
