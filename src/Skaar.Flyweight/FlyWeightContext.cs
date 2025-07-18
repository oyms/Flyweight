using Skaar.Flyweight.Contracts;

namespace Skaar.Flyweight;

public class FlyWeightScope : IDisposable
{
    private readonly List<IPurgable> _purgables = new();
    private static readonly AsyncLocal<Stack<FlyWeightScope>> Parents = new();

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
        var stack = Parents.Value;
        if (stack?.Count > 0)
        {
            stack.Pop();
        }
    }

    public static FlyWeightScope Create()
    {
        var scope = new FlyWeightScope();
        Parents.Value ??= new Stack<FlyWeightScope>();
        Parents.Value.Push(scope);
        return scope;
    }

    internal static FlyWeightScope? Current => Parents.Value?.Count > 0 ? Parents.Value!.Peek() : null;
}