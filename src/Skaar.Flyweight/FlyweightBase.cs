using Skaar.Flyweight.Contracts;
using Skaar.Flyweight.Repository;

namespace Skaar.Flyweight;

public abstract class FlyweightBase<T>(string value) : IComparable<T>
    where T : FlyweightBase<T>, IFlyweightFactory<T>
{
    private static readonly FlyWeightRepository<T> Instances = new();
    private readonly string _value = value;
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
    protected static T Get(string key, Func<string,T> create) => Instances.Get(key, create);

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
}