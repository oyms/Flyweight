namespace Skaar.Flyweight.Contracts;

public interface IPurgable
{
    /// <summary>
    /// Purge the flyweight instances.
    /// </summary>
    void Purge();
}