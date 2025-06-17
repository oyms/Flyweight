using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Skaar.Flyweight;

[Generator]
public class ClassGenerator: IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var toolName = Assembly.GetExecutingAssembly().GetName().Name;
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("GenerateFlyweightClassAttribute.g.cs", SourceText.From(
                $$"""
                using System;
                using System.CodeDom.Compiler;

                #pragma warning disable CS0436 // Type may be defined multiple times
                namespace Skaar.Flyweight;
                [GeneratedCode("{{toolName}}", "{{version}}")]
                [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                public class GenerateFlyweightClassAttribute : System.Attribute
                {
                    public GenerateFlyweightClassAttribute(string fullName)
                    {
                    }
                }
                
                """, Encoding.UTF8
                ));
        });
        
        var markers = context.CompilationProvider.SelectMany((compilation, _) =>
        {
            var markerAttr = compilation.GetTypeByMetadataName("Skaar.Flyweight.GenerateFlyweightClassAttribute");
            if (markerAttr == null) return [];
            
            return compilation.Assembly
                .GetAttributes()
                .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, markerAttr))
                .Select(attr =>
                {
                    return ParseName(attr.ConstructorArguments[0].Value as string);
                });
        });
        
        context.RegisterSourceOutput(markers, ((productionContext, args) =>
        {
            var source = $$"""
                         using System.Text.Json.Serialization;
                         using Skaar.Flyweight;
                         using Skaar.Flyweight.Contracts;
                         using Skaar.Flyweight.Serialization;
                         
                         namespace {{args.Namespace}};
                         [System.CodeDom.Compiler.GeneratedCode("{{toolName}}", "{{version}}")] 
                         [JsonConverter(typeof(FlyweightJsonConverter<{{args.Name}}>))]
                         public partial class {{args.Name}}: FlyweightBase<{{args.Name}}>, IFlyweightFactory<{{args.Name}}>
                         {
                             private {{args.Name}}(string key) : base(key)
                             {
                             }
                             
                             public static {{args.Name}} Get(string key)
                             {
                                 return Get(key, value => new {{args.Name}}(value));
                             }
                         }
                         """;
            productionContext.AddSource($"{args.Namespace}.{args.Name}.g.cs", SourceText.From(source, Encoding.UTF8));
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