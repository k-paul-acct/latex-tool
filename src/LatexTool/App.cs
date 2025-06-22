internal static class App
{
    public static Version GetVersion()
    {
        var version = typeof(Program).Assembly.GetName().Version;
        return version is null
            ? new Version(1, 0, 0)
            : new Version(version.Major, version.Minor, version.Build);
    }

    public static void UnknownCliArgument(IArgsToken arg)
    {
        if (arg is Option o)
        {
            throw new ArgumentException($"Unknown option '--{o.Value}'");
        }

        if (arg is Flag f)
        {
            throw new ArgumentException($"Unknown flag '-{f.Value}'");
        }

        if (arg is Word w)
        {
            throw new ArgumentException($"Unknown command '{w.Value}'");
        }
    }

    public static void PrintCommandsDescription(this Out outs, IEnumerable<CommandHelpInfo> commands)
    {
        var commandEntries = commands.Select(x => ($"  {x.Name}  ", x.Description)).ToList();
        PrintNameDescription(outs, commandEntries);
    }

    public static void PrintOptionsDescription(this Out outs, IEnumerable<OptionHelpInfo> options)
    {
        var optionEntries = options
            .Select(x => ((x.ShortName, x.LongName) switch
            {
                (not null, not null) => $"  -{x.ShortName.Value}, --{x.LongName}  ",
                (null, not null) => $"      --{x.LongName}  ",
                (not null, null) => $"  -{x.ShortName.Value}  ",
                _ => throw new ArgumentException("Option must have at least one name"),
            }, x.Description))
            .ToList();

        PrintNameDescription(outs, optionEntries);
    }

    private static void PrintNameDescription(Out outs, List<(string, string)> nameDescriptionPairs)
    {
        var widest = nameDescriptionPairs.MaxBy(x => x.Item1.Length).Item1?.Length ?? 0;
        var padded = nameDescriptionPairs.Select(x => (x.Item1.PadRight(widest), x.Item2));
        var descriptionWidth = Math.Max(outs.TerminalWidth - widest, 5);
        var spaces = new string(' ', widest);

        foreach (var (name, description) in padded)
        {
            outs.Write(name);

            var isFirst = true;
            foreach (var line in description.AsSpan().WrapText(descriptionWidth))
            {
                if (!isFirst)
                {
                    outs.Write(spaces);
                }

                outs.WriteLn(line);
                isFirst = false;
            }
        }
    }

    public readonly struct Version
    {
        public readonly int Major;
        public readonly int Minor;
        public readonly int Build;

        public Version(int major, int minor, int build)
        {
            Major = major;
            Minor = minor;
            Build = build;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}";
        }
    }

    public sealed class CommandHelpInfo
    {
        public required string Name { get; init; }
        public required string Description { get; init; }
    }

    public sealed class OptionHelpInfo
    {
        public required char? ShortName { get; init; }
        public required string? LongName { get; init; }
        public required string Description { get; init; }
    }

    public static IEnumerable<IArgsToken> ParseArgs(string[] args)
    {
        for (var i = 0; i < args.Length; ++i)
        {
            var arg = args[i];

            if (arg.StartsWith("--") && arg.Length > 2)
            {
                yield return new Option { Value = arg[2..] };
            }
            else if (arg.StartsWith('-') && arg.Length > 1)
            {
                for (var j = 1; j < arg.Length; ++j)
                {
                    yield return new Flag { Value = arg[j] };
                }
            }
            else
            {
                yield return new Word { Value = arg };
            }
        }
    }

    public static IArgsToken[] Rest(this IEnumerator<IArgsToken> tokens)
    {
        var list = new List<IArgsToken>();

        while (tokens.MoveNext())
        {
            list.Add(tokens.Current);
        }

        return [.. list];   
    }

    public interface IArgsToken
    {
        string StringValue { get; }
    }

    public sealed class Word : IArgsToken
    {
        public required string Value { get; init; }

        public string StringValue => Value;
    }

    public sealed class Option : IArgsToken
    {
        public required string Value { get; init; }

        public string StringValue => Value;
    }

    public sealed class Flag : IArgsToken
    {
        public required char Value { get; init; }

        public string StringValue => char.ToString(Value);
    }
}
