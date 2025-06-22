using System.Diagnostics.CodeAnalysis;
using Skaar.Flyweight.Contracts;
using Skaar.Flyweight.Repository;

namespace Skaar.Flyweight;

/// <summary>
/// A base class for implementing the Flyweight pattern.
/// The inner value of the flyweight is a string.
/// </summary>
/// <param name="value">The value of the instance.</param>
/// <typeparam name="T">The type of the flyweight class.</typeparam>
public abstract class FlyweightBase<T>(string value) : IComparable<T>,
    IFormattable, IParsable<T>
    where T : FlyweightBase<T>, IFlyweightFactory<T, string>
{
    private static readonly FlyWeightRepository<T> Instances = new();
    private readonly string _value = value;

    /// <inheritdoc cref="IFlyweightFactory{T, TInner}" />
    public static IEnumerable<T> AllValues => Instances.AllValues;
    
    public int CompareTo(T? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null)
        {
            return 1;
        }

        return string.Compare(_value, other._value, StringComparison.Ordinal);
    }
    private int CompareTo(FlyweightBase<T>? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null)
        {
            return 1;
        }

        return string.Compare(_value, other._value, StringComparison.Ordinal);
    }
    
    /// <inheritdoc cref="IFlyweightFactory{T, TInner}" />
    protected static T Get(string key, Func<string,T> create) => Instances.Get(key, create);

    /// <summary>
    /// Returns the inner value of this instance.
    /// </summary>
    public override string ToString() => _value;

    public override bool Equals(object? obj) => ReferenceEquals(this, obj);

    public override int GetHashCode() => _value.GetHashCode(StringComparison.InvariantCulture);
    
    public static bool operator ==(FlyweightBase<T> left, FlyweightBase<T> right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator !=(FlyweightBase<T> left, FlyweightBase<T> right) => !(left == right);
    
    public static bool operator <(FlyweightBase<T> left, FlyweightBase<T> right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(FlyweightBase<T> left, FlyweightBase<T> right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator >(FlyweightBase<T> left, FlyweightBase<T> right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(FlyweightBase<T> left, FlyweightBase<T> right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }

    public virtual string ToString(string? format, IFormatProvider? _ = null) => _value;
    public static T Parse(string s, IFormatProvider? _ = null) => T.Get(s);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result)
    {
        if(s == null)
        {
            result = null;
            return false;
        }
        result = Parse(s);
        return true;
    }
}