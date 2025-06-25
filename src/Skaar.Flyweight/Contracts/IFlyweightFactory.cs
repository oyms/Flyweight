namespace Skaar.Flyweight.Contracts;

/// <summary>
/// A type that defines a factory for creating and managing flyweight instances.
/// </summary>
/// <typeparam name="T">The flyweight type</typeparam>
/// <typeparam name="TInner">The inner value type</typeparam>
public interface IFlyweightFactory<out T, TInner>
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
    /// <summary>
    /// Get an instance of the flyweight type based on a predicate and a factory function.
    /// </summary>
    /// <param name="predicate">A method to check if the value exists already.</param>
    /// <param name="factory">A method to create the instance if it not yet exists.</param>
    /// <returns></returns>
    static abstract T Get(Predicate<TInner> predicate, Func<TInner> factory);
}