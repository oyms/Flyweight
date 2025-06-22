using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Skaar.Flyweight.StringBased.Extend;

[Generator]
public class FlyweightAttributeGenerator : FlyWeightClassGeneratorBase, IIncrementalGenerator
{
    public static string AttributeName = "FlyweightAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("FlyweightAttribute.g.cs", SourceText.From(
                $$"""
                  using System;
                  using System.CodeDom.Compiler;

                  #pragma warning disable CS0436 // Type may be defined multiple times
                  namespace {{AttributeNamespace}};
                  /// <summary>
                  /// Classes decorated with this attribute will trigger code generation.
                  /// A partial part of the class will be generated in the same namespace.
                  /// The part will contain the implementation of the flyweight pattern.
                  /// </summary>
                  /// <seealso cref="FlyweightBase{T}"/>
                  [GeneratedCode("{{ToolName}}", "{{ToolVersion}}")]
                  [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
                  public class {{AttributeName}} : System.Attribute
                  {}

                  """, Encoding.UTF8
            ));
        });

        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                (node, _) => node is ClassDeclarationSyntax cds && cds.AttributeLists.Any(),
                (ctx, _) =>
                {
                    var classSyntax = (ClassDeclarationSyntax)ctx.Node;
                    var symbol = ctx.SemanticModel.GetDeclaredSymbol(classSyntax);
                    return symbol;
                })
            .Where(s => s is not null && s.GetAttributes()
                .Any(attr => attr.AttributeClass?.ToDisplayString() == $"{AttributeNamespace}.{AttributeName}")
            )
            .Collect();

        context.RegisterSourceOutput(classDeclarations, (productionContext, classSymbols) =>
        {
            foreach (var classSymbol in classSymbols)
            {
                var className = classSymbol.Name;
                var ns = classSymbol.ContainingNamespace.ToDisplayString();
                var visibility = classSymbol.DeclaredAccessibility switch
                {
                    Accessibility.Public => "public ",
                    Accessibility.Internal => "internal ",
                    _ => string.Empty
                };
                productionContext.AddSource($"{ns}.{className}.g.cs", GetClassSource(className, ns, visibility));
            }
        });
    }
}