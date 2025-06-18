using System;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Skaar.Flyweight;

public abstract class FlyWeightClassGeneratorBase
{
    public static readonly string AttributeNamespace = "Skaar.Flyweight";
    protected string ToolName => Assembly.GetExecutingAssembly().GetName().Name;
    protected Version ToolVersion => Assembly.GetExecutingAssembly().GetName().Version;

    protected SourceText GetClassSource(string className, string @namespace)
    {
        var source = $$"""
                       using System.Text.Json.Serialization;
                       using Skaar.Flyweight;
                       using Skaar.Flyweight.Contracts;
                       using Skaar.Flyweight.Serialization;

                       namespace {{@namespace}};
                       [System.CodeDom.Compiler.GeneratedCode("{{ToolName}}", "{{ToolVersion}}")] 
                       [JsonConverter(typeof(FlyweightJsonConverter<{{className}}>))]
                       public partial class {{className}}: FlyweightBase<{{className}}>, IFlyweightFactory<{{className}}>
                       {
                           private {{className}}(string key) : base(key)
                           {
                           }
                           
                           public static {{className}} Get(string key)
                           {
                               return Get(key, value => new {{className}}(value));
                           }
                       }
                       """;
        return SourceText.From(source, Encoding.UTF8);
    }
}