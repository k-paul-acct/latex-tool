internal static class App
{
    public static Version GetVersion()
    {
        var version = typeof(Program).Assembly.GetName().Version;
        return version is null
            ? new Version(1, 0, 0)
            : new Version(version.Major, version.Minor, version.Build);
    }

    public static void UnknownCliArgument(string arg)
    {
        if (arg.StartsWith("--"))
        {
            throw new ArgumentException($"unknown argument '{arg}'");
        }

        if (arg.StartsWith('-'))
        {
            throw new ArgumentException($"unknown option '{arg}'");
        }

        throw new ArgumentException($"unknown command '{arg}'");
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
}
