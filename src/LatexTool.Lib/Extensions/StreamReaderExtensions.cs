namespace System.IO;

public static class StreamReaderExtensions
{
    public static IEnumerable<string> EnumerateAllLines(this StreamReader streamReader)
    {
        string? line;
        while ((line = streamReader.ReadLine()) is not null)
        {
            yield return line;
        }
    }
}
