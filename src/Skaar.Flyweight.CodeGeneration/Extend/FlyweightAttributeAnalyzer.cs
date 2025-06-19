using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Skaar.Flyweight.Extend;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FlyweightAttributeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor InvalidClassModifier = new(
        id: "FLYWEIGHT002",
        title: "Invalid Flyweight Attribute Usage",
        messageFormat: $"Classes decorated with the [{FlyweightAttributeGenerator.AttributeName}] attribute must be public and partial",
        category: "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(InvalidClassModifier);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
    }
    private void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;

        if (!classDecl.AttributeLists.Any())
            return;

        var model = context.SemanticModel;
        var symbol = model.GetDeclaredSymbol(classDecl);
        if (symbol == null)
            return;

        var hasFlyweightAttribute = symbol.GetAttributes()
            .Any(attr => attr.AttributeClass?.ToDisplayString() == $"{FlyWeightClassGeneratorBase.AttributeNamespace}.{FlyweightAttributeGenerator.AttributeName}");
        if (!hasFlyweightAttribute)
            return;

        var isPublicOrInternal = symbol.DeclaredAccessibility == Accessibility.Public ||
                                 symbol.DeclaredAccessibility == Accessibility.Internal;

        var isPartial = classDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

        if (!isPublicOrInternal || !isPartial)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                InvalidClassModifier,
                classDecl.Identifier.GetLocation()
            ));
        }
    }
}