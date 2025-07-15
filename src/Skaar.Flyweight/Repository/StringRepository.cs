namespace Skaar.Flyweight.Repository;

internal class StringRepository
{
    private static readonly HashSet<string> Strings = new();
    private static readonly object Lock = new();

    public string Get(string value)
    {
        lock (Lock)
        {
            if (Strings.TryGetValue(value, out var existing))
            {
                return existing;
            }

            Strings.Add(value);
            return value;
        }
    }
}