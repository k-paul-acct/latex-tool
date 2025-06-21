internal sealed class Piper
{
    private readonly IEnumerable<string> _lines;
    private readonly Out _outs;

    public Piper(IEnumerable<string> lines, Out outs)
    {
        _lines = lines;
        _outs = outs;
    }

    public LogAnalysisResult PipeInput()
    {
        var result = LogAnalysisResult.Success;

        foreach (var line in _lines)
        {
            var lineSpan = line.AsSpan();

            foreach (var token in lineSpan.Split(
                ["errors", "error", "warnings", "warning"],
                StringComparison.OrdinalIgnoreCase,
                includeSeparators: true))
            {
                if (token.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                {
                    _outs.Write(token, ConsoleFontStyle.Bold, ConsoleColor.Red);
                    result |= LogAnalysisResult.Errors;
                }
                else if (token.StartsWith("warning", StringComparison.OrdinalIgnoreCase))
                {
                    _outs.Write(token, ConsoleFontStyle.Bold, ConsoleColor.Yellow);
                    result |= LogAnalysisResult.Warnings;
                }
                else
                {
                    _outs.Write(token);
                }
            }

            _outs.WriteLn();
        }

        return result;
    }
}
