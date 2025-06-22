using Skaar.Flyweight.Contracts;
using Skaar.Flyweight.Repository;

namespace Skaar.Flyweight;

/// <summary>
/// A base class for implementing the Flyweight pattern.
/// The inner value of the flyweight is of type <typeparamref name="TInner"/>.
/// </summary>
/// <param name="value">The value of the instance.</param>
/// <typeparam name="T">The type of the flyweight class.</typeparam>
/// <typeparam name="TInner">The type of inner value.</typeparam>
/// <remarks>The type of <typeparamref name="TInner"/> should be equatable with itself.</remarks>
public abstract class FlyweightBase<T, TInner>(TInner value)
    where T : FlyweightBase<T, TInner> , IFlyweightFactory<T, TInner>
    where TInner : class
{
    private static readonly FlyWeightRepository<T, TInner> Instances = new();
    private readonly TInner _value = value;
    
    /// <inheritdoc cref="IFlyweightFactory{T, TInner}" />
    public static IEnumerable<T> AllValues => Instances.AllValues;
    
    /// <inheritdoc cref="IFlyweightFactory{T, TInner}" />
    protected static T Get(TInner key, Func<TInner,T> create) => Instances.Get(key, create);
    
    public override string ToString() => _value?.ToString() ?? string.Empty;
    
    public override bool Equals(object? obj) => ReferenceEquals(this, obj);
    
    public override int GetHashCode() => _value?.GetHashCode() ?? 0;

    public static implicit operator TInner(FlyweightBase<T, TInner> flyweight) => flyweight._value;
    
    public static bool operator ==(FlyweightBase<T, TInner> left, FlyweightBase<T, TInner> right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator !=(FlyweightBase<T, TInner> left, FlyweightBase<T, TInner> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// This will remove this instance from the inner repository.
    /// This will cause the instance to be garbage collected if there are no other references to it.
    /// When <see cref="Get"/> is called with the same key again, a new instance will be created.
    /// </summary>
    public void Detach() => Instances.Purge(_value);
}