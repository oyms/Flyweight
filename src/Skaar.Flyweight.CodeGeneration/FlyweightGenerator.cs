using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Skaar.Flyweight.Templates;

namespace Skaar.Flyweight;

[Generator]
public class FlyweightGenerator : IIncrementalGenerator
{
    public static readonly string ExtendAttributeName = "FlyweightAttribute";
    public static readonly string GenerateAttributeName = "GenerateFlyweightClassAttribute";
    public static readonly string AttributeNamespace = "Skaar.Flyweight";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var templates = new ClassTemplates();
        GenerateAttributeFiles(context, templates);

        GenerateStringBasedClassExtensionFiles(context, templates);
        GenerateStringBasedCreatedClassFiles(context, templates);
        GenerateGenericBasedCreatedClassFiles(context, templates);
    }

    private void GenerateGenericBasedCreatedClassFiles(IncrementalGeneratorInitializationContext context, ClassTemplates templates)
    {
        var markers = context.CompilationProvider.SelectMany((compilation, _) =>
        {
            var markerAttr = compilation.GetTypeByMetadataName($"{AttributeNamespace}.{GenerateAttributeName}`1");
            if (markerAttr == null) return [];
            
            return compilation.Assembly
                .GetAttributes()
                .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass?.ConstructedFrom, markerAttr))
                .Select(attr => new
                {
                    TypeArg = attr.AttributeClass?.TypeArguments.First(),
                    Name = ParseName(attr.ConstructorArguments[0].Value as string)
                })
                .Distinct();
        });
        
        context.RegisterSourceOutput(markers, ((productionContext, args) =>
        {
            var source = templates.TypeBasedClass(args.Name.Name, args.Name.Namespace, "public ", args.TypeArg.ToDisplayString());
            productionContext.AddSource($"{GenerateAttributeName}.{args.TypeArg.Name}.{args.Name.Name}.g.cs", SourceText.From(source, Encoding.UTF8));
        }));
    }

    private void GenerateStringBasedCreatedClassFiles(IncrementalGeneratorInitializationContext context,
        ClassTemplates templates)
    {
        var markers = context.CompilationProvider.SelectMany((compilation, _) =>
        {
            var markerAttr = compilation.GetTypeByMetadataName($"{AttributeNamespace}.{GenerateAttributeName}");
            if (markerAttr == null) return [];
            
            return compilation.Assembly
                .GetAttributes()
                .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, markerAttr))
                .Select(attr => ParseName(attr.ConstructorArguments[0].Value as string))
                .Distinct();
        });

        context.RegisterSourceOutput(markers, ((productionContext, args) =>
        {
            var source = templates.StringBasedClass(args.Name, args.Namespace, "public ");
            productionContext.AddSource($"{args.Namespace}.{args.Name}.g.cs", source);
        }));
    }

    private static void GenerateStringBasedClassExtensionFiles(IncrementalGeneratorInitializationContext context,
        ClassTemplates templates)
    {
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
                .Any(attr => attr.AttributeClass?.ToDisplayString() == $"{AttributeNamespace}.{ExtendAttributeName}")
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
                productionContext.AddSource($"{ns}.{className}.g.cs", templates.StringBasedClass(className, ns, visibility));
            }
        });
    }

    private static void GenerateAttributeFiles(IncrementalGeneratorInitializationContext context, ClassTemplates templates)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{ExtendAttributeName}.g.cs", SourceText.From(
                templates.NonGenericExtendAttribute(ExtendAttributeName, AttributeNamespace), Encoding.UTF8
            ));
        });
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{GenerateAttributeName}.g.cs", SourceText.From(
                templates.NonGenericGenerateAttribute(GenerateAttributeName, AttributeNamespace), Encoding.UTF8
            ));
        });        
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{GenerateAttributeName}.generic.g.cs", SourceText.From(
                templates.Generic1GenerateAttribute(GenerateAttributeName, AttributeNamespace), Encoding.UTF8
            ));
        });
    }
    private (string Name, string Namespace) ParseName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace.");
        }
        var indexOfLastDot= name!.LastIndexOf('.');
        if (indexOfLastDot == -1)
        {
            return (Name: name.Trim(), Namespace: "global");
        }

        if (indexOfLastDot == name.Length - 1)
        {
            return ParseName(name.Substring(0, name.Length - 1));
        }
        var namePart = name.Substring(indexOfLastDot + 1).Trim();
        var ns = name.Substring(0, indexOfLastDot).Trim();
        if (string.IsNullOrEmpty(ns)) ns = "global";
        return (Name: namePart, Namespace: ns);
    }
}