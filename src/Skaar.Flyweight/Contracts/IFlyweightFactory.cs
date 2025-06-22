namespace Skaar.Flyweight.Contracts;

/// <summary>
/// A type that defines a factory for creating and managing flyweight instances.
/// </summary>
/// <typeparam name="T">The flyweight type</typeparam>
public interface IFlyweightFactory<out T>
{
    /// <summary>
    /// Get all the registered values of the flyweight type.
    /// </summary>
    static abstract IEnumerable<T> AllValues { get; }
    /// <summary>
    /// Get an instance of the flyweight type based on a string value.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>An instance of <typeparamref name="T"/>.</returns>
    static abstract T Get(string value);
}