using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Skaar.Flyweight.StringBased.Generate;

[Generator]
public class GenerateFlyweightClassAttributeGenerator : FlyWeightClassGeneratorBase, IIncrementalGenerator
{
    public static string AttributeName = "GenerateFlyweightClassAttribute";
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{AttributeName}.g.cs", SourceText.From(
                $$"""
                using System;
                using System.CodeDom.Compiler;

                #pragma warning disable CS0436 // Type may be defined multiple times
                namespace {{AttributeNamespace}};
                /// <summary>
                /// This attribute will trigger code generation.
                /// A partial class will be generated in the same namespace.
                /// The class will implement the flyweight pattern.
                /// </summary>
                /// <seealso cref="FlyweightBase{T}"/>
                [GeneratedCode("{{ToolName}}", "{{ToolVersion}}")]
                [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                public class {{AttributeName}} : System.Attribute
                {
                    /// <param name="fullName">
                    /// The full name of the generated class (namespace and class name).
                    /// The name must be a valid class name
                    /// </param>
                    public {{AttributeName}}(string fullName)
                    {
                    }
                }
                
                """, Encoding.UTF8
                ));
        });
        
        var markers = context.CompilationProvider.SelectMany((compilation, _) =>
        {
            var markerAttr = compilation.GetTypeByMetadataName($"{AttributeNamespace}.{AttributeName}");
            if (markerAttr == null) return [];
            
            return compilation.Assembly
                .GetAttributes()
                .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, markerAttr))
                .Select(attr => ParseName(attr.ConstructorArguments[0].Value as string))
                .Distinct();
        });
        
        context.RegisterSourceOutput(markers, ((productionContext, args) =>
        {
            var source = GetClassSource(args.Name, args.Namespace, "public ");
            productionContext.AddSource($"{args.Namespace}.{args.Name}.g.cs", source);
        }));
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