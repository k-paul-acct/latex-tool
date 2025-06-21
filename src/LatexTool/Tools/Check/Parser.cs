using System.Text.RegularExpressions;

internal sealed partial class Parser
{
    [GeneratedRegex(@"(?:\S.*\.tex)(?:\s|\(|\)|$)")]
    public static partial Regex FilenameRegex();

    [GeneratedRegex(@"(?:Ov|Und)erfull \\[hv]box .*at lines? \d+(?:--\d+)?")]
    public static partial Regex WarningRegex();

    [GeneratedRegex(@"at lines? \d+")]
    public static partial Regex WarningLineNumberRegex();

    private readonly Stack<(int, int)> _filenameIndexStack = [];
    private readonly Stack<WarningEntry> _warningStack = [];
    private readonly string[] _lines;
    private readonly Out _outs;

    private int _parenCount;
    private int _i;

    public Parser(string[] lines, Out outs)
    {
        _lines = lines;
        _outs = outs;
    }

    public LogAnalysisResult Parse()
    {
        var result = LogAnalysisResult.Success;

        for (_i = 0; _i < _lines.Length; ++_i)
        {
            var line = _lines[_i].AsSpan();

            if (!TryGetWarning(line, out var warning))
            {
                ProcessPart(line, 0);
                continue;
            }

            _warningStack.Push(warning);
            result = LogAnalysisResult.Warnings;
        }

        return result;
    }

    private void ProcessPart(ReadOnlySpan<char> part, int offset)
    {
        for (var i = 0; i < part.Length; ++i)
        {
            var c = part[i];

            if (c == '(')
            {
                _parenCount += 1;
                _filenameIndexStack.Push((_i, offset + i + 1));
            }
            else if (c == ')')
            {
                _parenCount -= 1;

                if (!TryGetFilename(offset + i, out var fe))
                {
                    continue;
                }

                while (_warningStack.Count > 0 && _warningStack.Peek().Depth > _parenCount)
                {
                    var we = _warningStack.Pop();
                    var wl = _lines[we.LineIndex].AsSpan();
                    var warning = wl[we.StartIndex..(we.StartIndex + we.Length)];
                    var fl = _lines[fe.LineIndex].AsSpan();
                    var file = fl[fe.StartIndex..(fe.StartIndex + fe.Length)];
                    PrintWarning(file, we.CodeLineNumber, warning);
                }
            }
        }
    }

    private bool TryGetFilename(int end, out FilenameEntry filename)
    {
        var filenameRegex = FilenameRegex();
        var (startLineIndex, startIndex) = _filenameIndexStack.Pop();
        var line = _lines[startLineIndex].AsSpan();

        if (startLineIndex != _i)
        {
            end = line.Length;
        }

        var toFind = line[startIndex..end];

        foreach (var vm in filenameRegex.EnumerateMatches(toFind))
        {
            var trimmed = toFind[vm.Index..(vm.Index + vm.Length)].Trim();
            filename = new FilenameEntry(startLineIndex, vm.Index + startIndex, trimmed.Length);
            return true;
        }

        filename = default;
        return false;
    }

    private bool TryGetWarning(ReadOnlySpan<char> line, out WarningEntry warning)
    {
        var warningRegex = WarningRegex();
        var codeLineNumber = 0;

        foreach (var vm in warningRegex.EnumerateMatches(line))
        {
            var lineNumberRegex = WarningLineNumberRegex();
            var warningMatch = line[vm.Index..(vm.Index + vm.Length)];

            foreach (var lnvm in lineNumberRegex.EnumerateMatches(warningMatch))
            {
                var match = warningMatch[lnvm.Index..(lnvm.Index + lnvm.Length)];
                var spaceIndex = match.LastIndexOf(' ');
                codeLineNumber = int.Parse(match[(spaceIndex + 1)..]);
                break;
            }

            warning = new WarningEntry(_i, vm.Index, vm.Length, codeLineNumber, _parenCount);
            return true;
        }

        warning = default;
        return false;
    }

    private void PrintWarning(ReadOnlySpan<char> file, int lineNumber, ReadOnlySpan<char> warning)
    {
        var name = Path.GetFileName(file);
        _outs.Write(file[..(file.Length - name.Length)]);
        _outs.Write(name, ConsoleFontStyle.Bold);
        _outs.Write($"({lineNumber}): ");
        _outs.Write("warning", ConsoleFontStyle.Bold, ConsoleColor.Yellow);
        _outs.Write(": ");
        _outs.WriteLn(warning);
    }

    private readonly struct FilenameEntry
    {
        public readonly int LineIndex;
        public readonly int StartIndex;
        public readonly int Length;

        public FilenameEntry(int lineIndex, int startIndex, int length)
        {
            LineIndex = lineIndex;
            StartIndex = startIndex;
            Length = length;
        }
    }

    private readonly struct WarningEntry
    {
        public readonly int LineIndex;
        public readonly int StartIndex;
        public readonly int Length;
        public readonly int CodeLineNumber;
        public readonly int Depth;

        public WarningEntry(int lineIndex, int startIndex, int length, int codeLineNumber, int depth)
        {
            LineIndex = lineIndex;
            StartIndex = startIndex;
            Length = length;
            CodeLineNumber = codeLineNumber;
            Depth = depth;
        }
    }
}
