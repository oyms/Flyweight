using System;
using System.Reflection;

namespace Skaar.Flyweight.Templates;

class ClassTemplates
{
    string ToolName => Assembly.GetExecutingAssembly().GetName().Name;
    Version ToolVersion => Assembly.GetExecutingAssembly().GetName().Version;

    public string NonGenericExtendAttribute(string className, string @namespace)
    {
        return $$"""
             using System;
             using System.CodeDom.Compiler;

             #pragma warning disable CS0436 // Type may be defined multiple times
             namespace {{@namespace}};
             /// <summary>
             /// Classes decorated with this attribute will trigger code generation.
             /// A partial part of the class will be generated in the same namespace.
             /// The part will contain the implementation of the flyweight pattern.
             /// </summary>
             /// <seealso cref="FlyweightBase{T}"/>
             [GeneratedCode("{{ToolName}}", "{{ToolVersion}}")]
             [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
             public class {{className}} : System.Attribute
             {}

             """;
    }
    
    public string NonGenericGenerateAttribute(string className, string @namespace)
    {
        return $$"""
                 using System;
                 using System.CodeDom.Compiler;

                 #pragma warning disable CS0436 // Type may be defined multiple times
                 namespace {{@namespace}};
                 /// <summary>
                 /// This attribute will trigger code generation.
                 /// A partial class will be generated in the same namespace.
                 /// The class will implement the flyweight pattern.
                 /// </summary>
                 /// <seealso cref="FlyweightBase{T}"/>
                 [GeneratedCode("{{ToolName}}", "{{ToolVersion}}")]
                 [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                 public class {{className}} : System.Attribute
                 {
                     /// <param name="fullName">
                     /// The full name of the generated class (namespace and class name).
                     /// The name must be a valid class name
                     /// </param>
                     public {{className}}(string fullName)
                     {
                     }
                 }

                 """;
    }    
    
    public string Generic1ExtendAttribute(string className, string @namespace)
    {
        return $$"""
                 using System;
                 using System.CodeDom.Compiler;

                 #pragma warning disable CS0436 // Type may be defined multiple times
                 namespace {{@namespace}};
                 /// <summary>
                 /// Classes decorated with this attribute will trigger code generation.
                 /// A partial part of the class will be generated in the same namespace.
                 /// The part will contain the implementation of the flyweight pattern.
                 /// </summary>
                 /// <typeparam name="T">The type of the inner value</typeparam>
                 /// <seealso cref="FlyweightBase{T}"/>
                 [GeneratedCode("{{ToolName}}", "{{ToolVersion}}")]
                 [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
                 public class {{className}}<T> : System.Attribute where T : class
                 {}

                 """;
    }
    
    public string Generic1GenerateAttribute(string className, string @namespace)
    {
        return $$"""
                 using System;
                 using System.CodeDom.Compiler;

                 #pragma warning disable CS0436 // Type may be defined multiple times
                 namespace {{@namespace}};
                 /// <summary>
                 /// This attribute will trigger code generation.
                 /// A partial class will be generated in the same namespace.
                 /// The class will implement the flyweight pattern.
                 /// </summary>
                 /// <typeparam name="T">The type of the inner value</typeparam>
                 /// <seealso cref="FlyweightBase{T, TInner}"/>
                 [GeneratedCode("{{ToolName}}", "{{ToolVersion}}")]
                 [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                 public class {{className}}<T> : System.Attribute where T : class
                 {
                     /// <summary>
                     /// Initializes a new instance of the <see cref="{{className}}{T}"/> class.
                     /// </summary>
                     /// <param name="fullName">
                     /// The full name of the generated class (namespace and class name).
                     /// The name must be a valid class name
                     /// </param>
                     public {{className}}(string fullName)
                     {
                     }

                 }

                 """;
    }
    
    public string StringBasedClass(string className, string @namespace, string visibility)
    {
        var source = $$"""
           using System.Diagnostics.CodeAnalysis;
           using System.Text.Json.Serialization;
           using Skaar.Flyweight;
           using Skaar.Flyweight.Contracts;
           using Skaar.Flyweight.Serialization;

           namespace {{@namespace}};
           [System.CodeDom.Compiler.GeneratedCode("{{ToolName}}", "{{ToolVersion}}")] 
           [JsonConverter(typeof(FlyweightJsonConverter<{{className}}>))]
           {{visibility}}partial class {{className}} : 
                FlyweightBase<{{className}}>, IFlyweightFactory<{{className}}, string>
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
        return source;
    }    
    public string TypeBasedClass(string className, string @namespace, string visibility, string innerValueType)
    {
        var source = $$"""
           using System.Diagnostics.CodeAnalysis;
           using System.Text.Json.Serialization;
           using Skaar.Flyweight;
           using Skaar.Flyweight.Contracts;
           using Skaar.Flyweight.Serialization;

           namespace {{@namespace}};
           [System.CodeDom.Compiler.GeneratedCode("{{ToolName}}", "{{ToolVersion}}")] 
           [JsonConverter(typeof(FlyweightJsonConverter<{{className}}, {{innerValueType}}>))]
           {{visibility}}partial class {{className}} : 
                FlyweightBase<{{className}}, {{innerValueType}}>, IFlyweightFactory<{{className}}, {{innerValueType}}>
           {
               private {{className}}({{innerValueType}} key) : base(key)
               {
               }
               
               public static {{className}} Get({{innerValueType}} key)
               {
                   return Get(key, value => new {{className}}(value));
               }
           }
           """;
        return source;
    }
}