using System.Collections.Concurrent;

namespace Skaar.Flyweight.Repository;

internal class FlyWeightRepository<T>
{
    private readonly StringRepository _stringRepository = new();
    private static readonly ConcurrentDictionary<string, T> Instances = new();
    public T Get(string key, Func<string,T> create) => Instances.GetOrAdd(_stringRepository.Get(key), create);

    public IEnumerable<T> AllValues => Instances.Values;
}

internal class FlyWeightRepository<T, TInner> where TInner : notnull
{
    private static readonly ConcurrentDictionary<TInner, T> Instances = new();
    public T Get(TInner key, Func<TInner,T> create) => Instances.GetOrAdd(key, create);
    public IEnumerable<T> AllValues => Instances.Values;
    public void Purge(TInner instance) => Instances.Remove(instance, out _);
}