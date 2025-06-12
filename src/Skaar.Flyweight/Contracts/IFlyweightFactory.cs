namespace Skaar.Flyweight.Contracts;

public interface IFlyweightFactory<out T>
{
    static abstract IEnumerable<T> AllValues { get; }
    static abstract T Get(string value);
}