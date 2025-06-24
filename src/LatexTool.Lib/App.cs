using System.Reflection;
using LatexTool.Lib.IO;

namespace LatexTool.Lib;

public static class App
{
    public const string Name = "textool";

    public static Version GetVersion()
    {
        var version = (Assembly.GetEntryAssembly() ?? typeof(App).Assembly).GetName().Version;
        return version is null
            ? new Version(1, 0, 0)
            : new Version(version.Major, version.Minor, version.Build);
    }

    public static Version GetVersion(this Assembly assembly)
    {
        var version = assembly.GetName().Version;
        return version is null
            ? new Version(1, 0, 0)
            : new Version(version.Major, version.Minor, version.Build);
    }

    public static string GetCommandsDirectory()
    {
        var templateDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Name,
            "commands");

        Directory.CreateDirectory(templateDir);
        return templateDir;
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
                _ => "    ",
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

    public static IEnumerable<ArgToken> ParseArgs(string[] args)
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

    public static ArgToken[] Rest(this IEnumerator<ArgToken> tokens)
    {
        var list = new List<ArgToken>();

        while (tokens.MoveNext())
        {
            list.Add(tokens.Current);
        }

        return [.. list];
    }

    public abstract record ArgToken
    {
        public abstract string StringValue { get; }

        public static implicit operator ArgToken(string value)
        {
            if (value.StartsWith("--") && value.Length > 2)
            {
                return new Option { Value = value[2..] };
            }
            else if (value.StartsWith('-') && value.Length > 1)
            {
                return new Flag { Value = value[1] };
            }
            else
            {
                return new Word { Value = value };
            }
        }
    }

    public sealed record Word : ArgToken
    {
        public required string Value { get; init; }
        public override string StringValue => Value;

        public static implicit operator Word(string value)
        {
            return new Word { Value = value };
        }
    }

    public sealed record Option : ArgToken
    {
        public required string Value { get; init; }
        public override string StringValue => Value;

        public static implicit operator Option(string value)
        {
            return new Option { Value = value };
        }
    }

    public sealed record Flag : ArgToken
    {
        public required char Value { get; init; }
        public override string StringValue => char.ToString(Value);

        public static implicit operator Flag(char value)
        {
            return new Flag { Value = value };
        }
    }
}
