using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Skaar.Flyweight;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class GenerateFlyweightClassAttributeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor InvalidAttributeParams = new(
        id: "FLYWEIGHT001",
        title: "Invalid Flyweight Attribute Parameters",
        messageFormat: $"[{GenerateFlyweightClassAttributeGenerator.AttributeName}] parameter must be valid class- and namespace name",
        category: "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(InvalidAttributeParams);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(startContext =>
        {
            startContext.RegisterSyntaxNodeAction(
                AnalyzeAttribute,
                SyntaxKind.Attribute);
        });
    }

    private void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
    {
        var attributeSyntax = (AttributeSyntax)context.Node;

        var attributeList = attributeSyntax.Parent as AttributeListSyntax;
        if (attributeList == null ||
            !attributeList.Target.IsKind(SyntaxKind.AssemblyKeyword))
            return;

        // Is it our attribute?
        var symbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;
        if (symbol == null)
            return;
        var attrType = symbol.ContainingType;
        if (attrType.ToDisplayString() != $"{FlyWeightClassGeneratorBase.AttributeNamespace}.{GenerateFlyweightClassAttributeGenerator.AttributeName}")
            return;

        // Validate arguments
        if (attributeSyntax.ArgumentList is { Arguments.Count: 2 })
        {
            var nameArg = context.SemanticModel.GetConstantValue(attributeSyntax.ArgumentList.Arguments[0].Expression);
            var nsArg = context.SemanticModel.GetConstantValue(attributeSyntax.ArgumentList.Arguments[1].Expression);

            if (!IsValidTypeName(nameArg.ToString()) || !IsValidTypeName(nsArg.ToString()))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    InvalidAttributeParams,
                    attributeSyntax.GetLocation()));
            }
        }
    }
    private static bool IsValidTypeName(string candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate))
            return false;

        var typeSyntax = SyntaxFactory.ParseTypeName(candidate);
        
        if (typeSyntax.ToFullString().Trim() != candidate.Trim())
            return false;

        return !typeSyntax.ContainsDiagnostics;
    }
}