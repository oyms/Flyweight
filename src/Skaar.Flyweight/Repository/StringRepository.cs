namespace Skaar.Flyweight.Repository;

internal class StringRepository
{
    private static readonly HashSet<string> Strings = new();

    public string Get(string value)
    {
        if (Strings.TryGetValue(value, out var existing))
        {
            return existing;
        }
        Strings.Add(value);
        return value;
    }
}