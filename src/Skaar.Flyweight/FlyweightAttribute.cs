namespace Skaar.Flyweight;

/// <summary>
/// Classes decorated with this attribute will trigger code generation.
/// A partial part of the class will be generated in the same namespace.
/// The part will contain the implementation of the flyweight pattern.
/// </summary>
/// <remarks>Requires the Skaar.Flyweight.CodeGeneration package.</remarks>
/// <seealso cref="FlyweightBase{T}"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class FlyweightAttribute : Attribute;

/// <summary>
/// Classes decorated with this attribute will trigger code generation.
/// A partial part of the class will be generated in the same namespace.
/// The part will contain the implementation of the flyweight pattern.
/// </summary>
/// <remarks>Requires the Skaar.Flyweight.CodeGeneration package.</remarks>
/// <typeparam name="T">The type of the inner value</typeparam>
/// <seealso cref="FlyweightBase{T, TInner}"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class FlyweightAttribute<T> : Attribute where T : class;