using LatexTool.Lib.IO;

namespace LatexTool.Lib.Convention;

public sealed class CommandCallConvention
{
    public static CommandCallFlagOption SubCommandHelpOption { get; } = new()
    {
        Flag = null,
        Option = "help",
        Description = "Show help message for given command.",
        HasValue = false,
        IsMandatory = false,
    };

    private static readonly CommandCallFlagOption HelpOption = new()
    {
        Flag = null,
        Option = "help",
        Description = "Show this help message and exit.",
        HasValue = false,
        IsMandatory = false
    };

    public Func<App.IArgToken[], CommandBase> CommandFactory { get; }
    public string Name { get; }
    public string FullName { get; }
    public string Description { get; }
    public string[] Aliases { get; }
    public List<CommandCallFlagOption> FlagOptions { get; }
    public List<CommandCallConvention> Commands { get; }
    public List<CommandCallArgument> Arguments { get; }

    public CommandCallConvention(
        string name,
        string fullName,
        string description,
        string[] aliases,
        List<CommandCallFlagOption> flagOptions,
        List<CommandCallConvention> commands,
        List<CommandCallArgument> arguments,
        Func<App.IArgToken[], CommandBase> commandFactory)
    {
        Name = name;
        FullName = fullName;
        Description = description;
        Aliases = aliases;
        FlagOptions = flagOptions;
        Commands = commands;
        Arguments = arguments;
        CommandFactory = commandFactory;

        ValidateArguments();
    }

    public void ValidateArguments()
    {
        for (var i = 1; i < Arguments.Count; ++i)
        {
            if (Arguments[i].IsMandatory && !Arguments[i - 1].IsMandatory)
            {
                throw new InvalidOperationException(
                    $"Mandatory argument '{Arguments[i].Name}' must not follow" +
                    $"an optional argument '{Arguments[i - 1].Name}'");
            }
        }
    }

    public CommandCallParsingResult ParseCliArguments(App.IArgToken[] args)
    {
        var flagOptions = new List<(CommandCallFlagOption FlagOption, string? Value)>();
        var arguments = new List<(CommandCallArgument, string)>();
        var argumentsIndex = 0;
        var errors = new List<string>();

        using var enumerator = args.AsEnumerable().GetEnumerator();

        while (enumerator.MoveNext())
        {
            var token = enumerator.Current;

            if (token is App.Flag flag)
            {
                var required = FlagOptions.FirstOrDefault(x => x.Flag == flag);
                if (required is null)
                {
                    errors.Add($"Unknown flag '-{flag.Value}'");
                }
                else
                {
                    if (!required.HasValue)
                    {
                        flagOptions.Add((required, null));
                    }
                    else
                    {
                        if (enumerator.MoveNext())
                        {
                            var next = enumerator.Current;
                            if (next is App.Word word)
                            {
                                flagOptions.Add((required, word.Value));
                            }
                            else
                            {
                                errors.Add($"Flag '-{flag.Value}' requires a value");
                            }
                        }
                        else
                        {
                            errors.Add($"Flag '-{flag.Value}' requires a value");
                        }
                    }
                }
            }
            else if (token is App.Option option)
            {
                if (option is { Value: "help" })
                {
                    return new CommandCallParsingResult
                    {
                        FlagOptions = [(HelpOption, null)],
                        Command = null,
                        Arguments = [],
                        Rest = [],
                        Errors = []
                    };
                }

                var required = FlagOptions.FirstOrDefault(x => x.Option == option);
                if (required is null)
                {
                    errors.Add($"Unknown option '--{option.Value}'");
                }
                else
                {
                    if (!required.HasValue)
                    {
                        flagOptions.Add((required, null));
                    }
                    else
                    {
                        if (enumerator.MoveNext())
                        {
                            var next = enumerator.Current;
                            if (next is App.Word word)
                            {
                                flagOptions.Add((required, word.Value));
                            }
                            else
                            {
                                errors.Add($"Option '--{option.Value}' requires a value");
                            }
                        }
                        else
                        {
                            errors.Add($"Option '--{option.Value}' requires a value");
                        }
                    }
                }
            }
            else if (token is App.Word word)
            {
                var commandConvention = Commands.FirstOrDefault(c => c.Name == word.Value);
                if (commandConvention is not null)
                {
                    var rest = enumerator.Rest();
                    var command = commandConvention.CommandFactory(rest);
                    Validate();
                    return new CommandCallParsingResult
                    {
                        FlagOptions = flagOptions,
                        Command = (word.Value, command),
                        Arguments = arguments,
                        Rest = rest,
                        Errors = errors
                    };
                }

                if (argumentsIndex >= Arguments.Count)
                {
                    errors.Add($"Unexpected argument '{word.Value}'");
                }
                else
                {
                    var argument = Arguments[argumentsIndex++];
                    arguments.Add((argument, word.Value));
                }
            }
        }

        Validate();
        return new CommandCallParsingResult
        {
            FlagOptions = flagOptions,
            Command = null,
            Arguments = arguments,
            Rest = [],
            Errors = errors
        };

        void Validate()
        {
            foreach (var flagOption in FlagOptions)
            {
                if (flagOption.IsMandatory && !flagOptions.Any(x => x.FlagOption == flagOption))
                {
                    errors.Add($"Missing required option " +
                               $"{(flagOption.Flag?.Value is char f ? $"'-{f}'" : $"'--{flagOption.Option?.Value}'")}");
                }
            }

            if (argumentsIndex < Arguments.Count(x => x.IsMandatory))
            {
                var missing = Arguments.Skip(argumentsIndex).Where(a => a.IsMandatory);
                foreach (var arg in missing)
                {
                    errors.Add($"Missing required argument '{arg.Name}'");
                }
            }
        }
    }

    public void PrintHelp(Out outs)
    {
        outs.WriteLn("Description:");
        outs.WriteLn("  " + Description);

        outs.WriteLn();
        outs.WriteLn($"Usage:");
        foreach (var alias in Aliases)
        {
            outs.WriteLn("  " + alias);
        }

        if (Arguments.Count > 0)
        {
            outs.WriteLn();
            outs.WriteLn("Arguments:");
            outs.PrintCommandsDescription(Arguments.Select(a => new App.CommandHelpInfo
            {
                Name = a.Name,
                Description = a.Description
            }));
        }

        if (FlagOptions.Count > 0)
        {
            outs.WriteLn();
            outs.WriteLn("Options:");
            outs.PrintOptionsDescription(FlagOptions.Select(fo => new App.OptionHelpInfo
            {
                ShortName = fo.Flag?.Value,
                LongName = fo.Option?.Value,
                Description = fo.Description
            }));
        }

        if (Commands.Count > 0)
        {
            outs.WriteLn();
            outs.WriteLn("Commands:");
            App.PrintCommandsDescription(
                outs,
                Commands.Select(c => new App.CommandHelpInfo
                {
                    Name = c.Name,
                    Description = c.Description
                }));
        }
    }
}
