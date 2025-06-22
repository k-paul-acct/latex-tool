namespace LatexTool.Lib.IO;

public readonly struct Out
{
    private readonly bool _isOutputRedirected;

    public readonly int TerminalWidth;

    public Out()
    {
        _isOutputRedirected = Console.IsOutputRedirected;
        TerminalWidth = _isOutputRedirected ? 80 : Console.WindowWidth;
    }

    public void Write(
        ReadOnlySpan<char> text,
        ConsoleFontStyle fontStyle = ConsoleFontStyle.Regular,
        ConsoleColor? color = null)
    {
        if (_isOutputRedirected)
        {
            Console.Out.Write(text);
            return;
        }

        if (fontStyle == ConsoleFontStyle.Bold)
        {
            Console.Out.Write("\x1b[1m");
        }

        if (color is not null)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color.Value;
            Console.Out.Write(text);
            Console.ForegroundColor = oldColor;
        }
        else
        {
            Console.Out.Write(text);
        }

        if (fontStyle == ConsoleFontStyle.Bold)
        {
            Console.Out.Write("\x1b[0m");
        }
    }

    public void WriteLn()
    {
        Console.Out.WriteLine();
    }

    public void WriteLn(
        ReadOnlySpan<char> text,
        ConsoleFontStyle fontStyle = ConsoleFontStyle.Regular,
        ConsoleColor? color = null)
    {
        Write(text, fontStyle, color);
        WriteLn();
    }

    public void Write(
        string? text,
        ConsoleFontStyle fontStyle = ConsoleFontStyle.Regular,
        ConsoleColor? color = null)
    {
        Write(text.AsSpan(), fontStyle, color);
    }

    public void WriteLn(
        string? text,
        ConsoleFontStyle fontStyle = ConsoleFontStyle.Regular,
        ConsoleColor? color = null)
    {
        Write(text.AsSpan(), fontStyle, color);
        WriteLn();
    }

    public void Write<T>(
        T value,
        ConsoleFontStyle fontStyle = ConsoleFontStyle.Regular,
        ConsoleColor? color = null)
    {
        var text = value?.ToString() ?? "";
        Write(text.AsSpan(), fontStyle, color);
    }

    public void WriteLn<T>(
        T value,
        ConsoleFontStyle fontStyle = ConsoleFontStyle.Regular,
        ConsoleColor? color = null)
    {
        var text = value?.ToString() ?? "";
        Write(text.AsSpan(), fontStyle, color);
        WriteLn();
    }
}
