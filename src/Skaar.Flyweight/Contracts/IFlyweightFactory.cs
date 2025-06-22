namespace Skaar.Flyweight.Contracts;

/// <summary>
/// A type that defines a factory for creating and managing flyweight instances.
/// </summary>
/// <typeparam name="T">The flyweight type</typeparam>
/// <typeparam name="TInner">The inner value type</typeparam>
public interface IFlyweightFactory<out T, in TInner>
{
    /// <summary>
    /// Get all the registered values of the flyweight type.
    /// </summary>
    static abstract IEnumerable<T> AllValues { get; }
    /// <summary>
    /// Get an instance of the flyweight type based on an inner value.
    /// </summary>
    /// <param name="value">The inner value.</param>
    /// <returns>An instance of <typeparamref name="T"/>.</returns>
    static abstract T Get(TInner value);
}