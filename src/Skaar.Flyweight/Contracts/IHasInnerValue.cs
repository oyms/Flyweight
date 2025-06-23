namespace Skaar.Flyweight.Contracts;

/// <summary>
/// A flyweight wrapper with an inner value.
/// </summary>
/// <typeparam name="TInner">The type of the inner value</typeparam>
public interface IHasInnerValue<out TInner>
{
    TInner GetInnerValue();
}