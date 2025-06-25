using System.Collections.Concurrent;
using Skaar.Flyweight.Contracts;

namespace Skaar.Flyweight.Repository;

internal class FlyWeightRepository<T> where T:IHasInnerValue<string>
{
    private static readonly Lock _lock = new();
    private readonly StringRepository _stringRepository = new();
    private static readonly ConcurrentDictionary<string, T> Instances = new();
    public T Get(string key, Func<string,T> create) => Instances.GetOrAdd(_stringRepository.Get(key), create);
    public T Get(Predicate<string> predicate, Func<T> factory)
    {
        lock (_lock)
        {
            var existing = Instances.Keys.FirstOrDefault(x => predicate(x));
            if (existing is not null)
            {
                return Instances[existing];
            }

            var instance = factory.Invoke();
            var key = _stringRepository.Get(instance.GetInnerValue());
            Instances[key] = instance;
            return instance;
        }
    }
    public IEnumerable<T> AllValues => Instances.Values;
}

internal class FlyWeightRepository<T, TInner> where TInner : notnull where T : class, IHasInnerValue<TInner>
{
    private static readonly Lock _lock = new();
    private static readonly ConcurrentDictionary<TInner, WeakReference<T>> Instances = new();
    public T Get(TInner key, Func<TInner,T> create)
    {
        var reference = Instances.GetOrAdd(key, x => new WeakReference<T>(create(x)));
        if (reference.TryGetTarget(out var value))
        {
            return value;
        }

        Instances.Remove(key, out _);
        Instances.GetOrAdd(key, (x) => new WeakReference<T>(create(x)));
        return Get(key, create);
    }
    
    public T Get (Predicate<TInner> predicate, Func<T> factory)
    {
        lock (_lock)
        {
            var existing = Instances.Keys.FirstOrDefault(x => predicate(x));
            if (existing is not null && Instances[existing].TryGetTarget(out var value))
            {
                return value;
            }

            var innerValue = factory.Invoke();
            var key = innerValue.GetInnerValue();
            Instances[key] = new WeakReference<T>(innerValue);
            return innerValue;
        }
    }

    public IEnumerable<T> AllValues => Instances
        .Values
        .Select(i => i.TryGetTarget(out var value) ? value : null)
        .Where(i => i is not null).Cast<T>();

    public void Purge()
    {
        lock (_lock)
        {
            var deadKeys = Instances.Keys.Where(k => Instances[k].TryGetTarget(out var _) == false).ToList();
            foreach (var key in deadKeys)
            {
                Instances.Remove(key, out _);
            }
        }
    }
}