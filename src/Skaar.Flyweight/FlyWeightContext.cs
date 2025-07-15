using Skaar.Flyweight.Contracts;

namespace Skaar.Flyweight;

public class FlyWeightScope : IDisposable
{
    private readonly List<IPurgable> _purgables = new();
    private static readonly AsyncLocal<FlyWeightScope?> CurrentContext = new();
    private static FlyWeightScope? _parent;

    private FlyWeightScope()
    {
    }

    internal void Add(IPurgable purgable)
    {
        _purgables.Add(purgable);
    }
    
    public void Dispose()
    {
        foreach (var purgable in _purgables)
        {
            purgable.Purge();
        }
        _purgables.Clear();
        CurrentContext.Value = _parent;
    }

    public static FlyWeightScope Create()
    {
        _parent = CurrentContext.Value;
        return CurrentContext.Value = new();
    }

    internal static FlyWeightScope? Current => CurrentContext.Value;
}