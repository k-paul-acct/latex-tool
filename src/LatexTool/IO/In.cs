internal static class In
{
    public static string[] ReadAllLines()
    {
        string? line;
        List<string> lines = [];

        while ((line = Console.ReadLine()) is not null)
        {
            lines.Add(line);
        }

        return [.. lines];
    }

    public static IEnumerable<string> EnumerateAllLines()
    {
        string? line;

        while ((line = Console.ReadLine()) is not null)
        {
            yield return line;
        }
    }
}
