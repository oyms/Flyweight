using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Skaar.Flyweight;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FlyweightAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor InvalidClassModifier = new(
        id: "FLYWEIGHT002",
        title: "Invalid Flyweight Attribute Usage",
        messageFormat: $"Classes decorated with the [{Generator.ExtendAttributeName}] attribute must be public or internal, and partial",
        category: "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    private static readonly DiagnosticDescriptor InvalidAttributeParams = new(
        id: "FLYWEIGHT001",
        title: "Invalid Flyweight Attribute Parameters",
        messageFormat: $"[{Generator.GenerateAttributeName}] parameter must be valid class- and namespace name",
        category: "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(InvalidClassModifier, InvalidAttributeParams);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(AnalyzeDecoratedClass, SyntaxKind.ClassDeclaration);
        context.RegisterCompilationStartAction(startContext =>
        {
            startContext.RegisterSyntaxNodeAction(
                AnalyzeAttribute,
                SyntaxKind.Attribute);
        });
    }
    
    private void AnalyzeDecoratedClass(SyntaxNodeAnalysisContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;

        if (!classDecl.AttributeLists.Any())
            return;

        var model = context.SemanticModel;
        var symbol = model.GetDeclaredSymbol(classDecl);
        if (symbol == null)
            return;

        var hasFlyweightAttribute = symbol.GetAttributes()
            .Any(attr => attr.AttributeClass?.ToDisplayString() == $"{Generator.AttributeNamespace}.{Generator.ExtendAttributeName}");
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
        if (attrType.ToDisplayString() != $"{Generator.AttributeNamespace}.{Generator.GenerateAttributeName}")
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