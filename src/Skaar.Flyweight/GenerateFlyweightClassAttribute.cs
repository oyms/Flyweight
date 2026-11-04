namespace Skaar.Flyweight;

/// <summary>
/// This attribute will trigger code generation.
/// A partial class will be generated in the same namespace.
/// The class will implement the flyweight pattern.
/// </summary>
/// <remarks>Requires the Skaar.Flyweight.CodeGeneration package.</remarks>
/// <param name="fullName">
/// The full name of the generated class (namespace and class name).
/// The name must be a valid class name
/// </param>
/// <seealso cref="FlyweightBase{T}"/>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class GenerateFlyweightClassAttribute(string fullName) : Attribute
{
    /// <summary>
    /// The full name of the generated class (namespace and class name).
    /// </summary>
    public string FullName { get; } = fullName;
}

/// <summary>
/// This attribute will trigger code generation.
/// A partial class will be generated in the same namespace.
/// The class will implement the flyweight pattern.
/// </summary>
/// <remarks>Requires the Skaar.Flyweight.CodeGeneration package.</remarks>
/// <typeparam name="T">The type of the inner value</typeparam>
/// <param name="fullName">
/// The full name of the generated class (namespace and class name).
/// The name must be a valid class name
/// </param>
/// <seealso cref="FlyweightBase{T, TInner}"/>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class GenerateFlyweightClassAttribute<T>(string fullName) : Attribute where T : class
{
    /// <summary>
    /// The full name of the generated class (namespace and class name).
    /// </summary>
    public string FullName { get; } = fullName;
}
