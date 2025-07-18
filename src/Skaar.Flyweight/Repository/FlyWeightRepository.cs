using System.Collections.Concurrent;
using Skaar.Flyweight.Contracts;

namespace Skaar.Flyweight.Repository;

internal class FlyWeightRepository<T, TInner> where TInner : notnull where T : class, IHasInnerValue<TInner>, IPurgable
{
    private static readonly Lock Lock = new();
    private const int CleanupThreshold = 10000;
    private volatile int _itemsSinceCleanup;
    
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
        ConsiderPurge();
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
            ConsiderPurge();
            return innerValue;
        }
    }

    public IEnumerable<T> AllValues => Instances
        .Values
        .Select(i => i.TryGetTarget(out var value) ? value : null)
        .Where(i => i is not null).Cast<T>();

    private void ConsiderPurge()
    {
        if (Interlocked.Increment(ref _itemsSinceCleanup) >= CleanupThreshold)
        {
            Purge();
        }
    }
    
    public void Purge()
    {
        Interlocked.Exchange(ref _itemsSinceCleanup, 0);
        lock (Lock)
        {
            var deadKeys = Instances.Keys.Where(k => Instances[k].TryGetTarget(out _) == false).ToList();
            foreach (var key in deadKeys)
            {
                Instances.TryRemove(key, out _);
            }
        }
    }

    public void Purge(T value)
    {
        Instances.Remove(value.GetInnerValue(), out _);
    }
}