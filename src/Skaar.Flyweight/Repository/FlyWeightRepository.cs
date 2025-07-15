using System.Collections.Concurrent;
using Skaar.Flyweight.Contracts;

namespace Skaar.Flyweight.Repository;

internal class FlyWeightRepository<T> where T:IHasInnerValue<string>, IPurgable
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Lock Lock = new();
    private readonly StringRepository _stringRepository = new();
    private static readonly ConcurrentDictionary<string, T> Instances = new();
    public T Get(string key, Func<string,T> create) => Instances.GetOrAdd(_stringRepository.Get(key), k =>
    {
        var instance = create(k);
        FlyWeightScope.Current?.Add(instance);
        return instance;
    });
    public T Get(Predicate<string> predicate, Func<T> factory)
    {
        lock (Lock)
        {
            var existing = Instances.Keys.FirstOrDefault(x => predicate(x));
            if (existing is not null)
            {
                return Instances[existing];
            }

            var instance = factory.Invoke();
            var key = _stringRepository.Get(instance.GetInnerValue());
            Instances[key] = instance;
            FlyWeightScope.Current?.Add(instance);
            return instance;
        }
    }
    public IEnumerable<T> AllValues => Instances.Values;
    
    public void Purge(T value)
    {
        Instances.Remove(value.GetInnerValue(), out var _);
    }
}

internal class FlyWeightRepository<T, TInner> where TInner : notnull where T : class, IHasInnerValue<TInner>, IPurgable
{
    private static readonly Lock Lock = new();
    private static readonly ConcurrentDictionary<TInner, WeakReference<T>> Instances = new();
    public T Get(TInner key, Func<TInner,T> create)
    {
        WeakReference<T> Create(TInner inner)
        {
            var wrapper = create(inner);
            FlyWeightScope.Current?.Add(wrapper);
            return new WeakReference<T>(wrapper);
        }
        
        var reference = Instances.GetOrAdd(key, Create);
        if (reference.TryGetTarget(out var value))
        {
            return value;
        }

        Instances.Remove(key, out _);
        Instances.GetOrAdd(key, Create);
        return Get(key, create);
    }
    
    public T Get (Predicate<TInner> predicate, Func<T> factory)
    {
        lock (Lock)
        {
            var existing = Instances.Keys.FirstOrDefault(x => predicate(x));
            if (existing is not null && Instances[existing].TryGetTarget(out var value))
            {
                return value;
            }

            var innerValue = factory.Invoke();
            var key = innerValue.GetInnerValue();
            Instances[key] = new WeakReference<T>(innerValue);
            FlyWeightScope.Current?.Add(innerValue);
            return innerValue;
        }
    }

    public IEnumerable<T> AllValues => Instances
        .Values
        .Select(i => i.TryGetTarget(out var value) ? value : null)
        .Where(i => i is not null).Cast<T>();

    public void Purge()
    {
        lock (Lock)
        {
            var deadKeys = Instances.Keys.Where(k => Instances[k].TryGetTarget(out var _) == false).ToList();
            foreach (var key in deadKeys)
            {
                Instances.Remove(key, out _);
            }
        }
    }

    public void Purge(T value)
    {
        Instances.Remove(value.GetInnerValue(), out var _);
    }
}