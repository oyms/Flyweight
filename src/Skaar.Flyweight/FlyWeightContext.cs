using Skaar.Flyweight.Contracts;

namespace Skaar.Flyweight;

public class FlyWeightScope : IDisposable
{
    private readonly Lock _lock = new();
    private bool _isDisposed;
    private readonly List<IPurgable> _purgeables = [];
    private static readonly AsyncLocal<Stack<FlyWeightScope>> Parents = new();

    private FlyWeightScope()
    {
    }

    internal void Add(IPurgable purgable)
    {
        lock (_lock)
        {
            if (_isDisposed) throw new InvalidOperationException("Cannot add purgable to a disposed FlyWeightScope.");
            _purgeables.Add(purgable);
        }
    }
    
    public void Dispose()
    {
        lock (_lock)
        {
            foreach (var purgable in _purgeables)
            {
                purgable.Purge();
            }

            _purgeables.Clear();
            var stack = Parents.Value;
            if (stack?.Count > 0)
            {
                stack.Pop();
            }

            _isDisposed = true;
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