using System;
using System.Reflection;

namespace Skaar.Flyweight.Templates;

class ClassTemplates
{
    string ToolName => Assembly.GetExecutingAssembly().GetName().Name;
    Version ToolVersion => Assembly.GetExecutingAssembly().GetName().Version;

    public string StringBasedClass(string className, string @namespace, string visibility)
    {
        var source = $$"""
           using System.Diagnostics.CodeAnalysis;
           using System.Text.Json.Serialization;
           using Skaar.Flyweight;
           using Skaar.Flyweight.Contracts;
           using Skaar.Flyweight.Serialization;

           #nullable enable
           
           namespace {{@namespace}};
           [System.CodeDom.Compiler.GeneratedCode("{{ToolName}}", "{{ToolVersion}}")] 
           [JsonConverter(typeof(FlyweightJsonConverter<{{className}}>))]
           {{visibility}}partial class {{className}} : 
                FlyweightBase<{{className}}>, IFlyweightFactory<{{className}}, string>
           {
               private {{className}}(string key) : base(key)
               {
               }
               
               public static {{className}} Get(string key) => GetOrCreate(key, value => new {{className}}(value));
               public static {{className}} Get(System.Predicate<string> predicate, System.Func<string> factory) => GetOrCreate(predicate, () => new {{className}}(factory()));
           }
           """;
        return source;
    }    
    public string TypeBasedClass(string className, string @namespace, string visibility, string innerValueType, InterfaceImplementations interfaces)
    {
        var source = $$"""
           using System.Diagnostics.CodeAnalysis;
           using System.Text.Json.Serialization;
           using Skaar.Flyweight;
           using Skaar.Flyweight.Contracts;
           using Skaar.Flyweight.Serialization;
           
           #nullable enable

           namespace {{@namespace}};
           [System.CodeDom.Compiler.GeneratedCode("{{ToolName}}", "{{ToolVersion}}")] 
           [JsonConverter(typeof(FlyweightJsonConverter<{{className}}, {{innerValueType}}>))]
           {{visibility}}partial class {{className}} : 
                FlyweightBase<{{className}}, {{innerValueType}}>, IFlyweightFactory<{{className}}, {{innerValueType}}> {{interfaces.InterfaceList()}}
           {
               private {{className}}({{innerValueType}} key) : base(key)
               {
               }
           {{interfaces.Implementations()}}
               
               public static {{className}} Get({{innerValueType}} key) => GetOrCreate(key, value => new {{className}}(value));
               public static {{className}} Get(System.Predicate<{{innerValueType}}> predicate, System.Func<{{innerValueType}}> factory) => GetOrCreate(predicate, () => new {{className}}(factory()));   
           }
           """;
        return source;
    }
}