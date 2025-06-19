using System.Collections.Concurrent;

namespace Skaar.Flyweight.Repository;

internal class FlyWeightRepository<T>
{
    private readonly StringRepository _stringRepository = new();
    private static readonly ConcurrentDictionary<string, T> Instances = new();
    public T Get(string key, Func<string,T> create) => Instances.GetOrAdd(_stringRepository.Get(key), create);

    public IEnumerable<T> AllValues => Instances.Values;
}