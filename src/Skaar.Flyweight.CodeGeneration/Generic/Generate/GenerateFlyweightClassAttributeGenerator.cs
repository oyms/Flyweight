using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Skaar.Flyweight.StringBased;

namespace Skaar.Flyweight.Generic.Generate;

[Generator]
public class GenerateFlyweightClassAttributeGenerator : FlyWeightClassGeneratorBase, IIncrementalGenerator
{
    public static readonly string AttributeName = "GenerateFlyweightClassAttribute";
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{AttributeName}.Generic.g.cs", SourceText.From(
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
                /// <typeparam name="T">The type of the inner value</typeparam>
                /// <seealso cref="FlyweightBase{T, TInner}"/>
                [GeneratedCode("{{ToolName}}", "{{ToolVersion}}")]
                [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                public class {{AttributeName}}<T> : System.Attribute where T : class
                {
                    /// <summary>
                    /// Initializes a new instance of the <see cref="{{AttributeName}}{T}"/> class.
                    /// </summary>
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
    }
}