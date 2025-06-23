using System.Reflection;
using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command(App.Name, null)]
internal sealed class RootCommand : CommandBase
{
    public RootCommand(App.IArgToken[] args) : base(args)
    {
    }

    protected override ValueTask Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        if (parsingResult.ContainsOption("version"))
        {
            outs.WriteLn(App.GetVersion());
            return ValueTask.CompletedTask;
        }

        if (parsingResult.Command is null)
        {
            outs.WriteLn("Run 'textool --help' for more information.");
            return ValueTask.CompletedTask;
        }

        return parsingResult.Command.Value.Item2.Execute(outs);
    }

    protected override void PrintHelp(Out outs, CommandCallConvention convention)
    {
        convention.PrintHelp(outs);
        outs.WriteLn();
        outs.WriteLn("Run 'textool COMMAND --help' for more information on a command.");
    }

    public override CommandCallConvention GetConvention()
    {
        var commandsDir = App.GetCommandsDirectory();
        var files = Directory.GetFiles(commandsDir, "*.dll");
        var conventions = new List<CommandCallConvention>();

        foreach (var file in files)
        {
            var assembly = Assembly.LoadFile(file);
            var commandType = assembly
                .GetExportedTypes()
                .FirstOrDefault(t => typeof(CommandBase).IsAssignableFrom(t) &&
                                     !t.IsInterface &&
                                     !t.IsAbstract &&
                                     t.GetCustomAttribute<CommandAttribute>()?.ParentCommand == App.Name);

            if (commandType is not null)
            {
                App.IArgToken[] emptyArgs = [];
                var instance = Activator.CreateInstance(commandType, [emptyArgs]);
                var methodInfo = commandType.GetMethod("GetConvention", BindingFlags.Public | BindingFlags.Instance);
                if (methodInfo is not null && instance is not null)
                {
                    var convention = methodInfo.Invoke(instance, null) as CommandCallConvention;
                    if (convention is not null)
                    {
                        conventions.Add(convention);
                    }
                }
            }
        }

        return new CommandCallConvention(
            name: App.Name,
            fullName: App.Name,
            description: "A tool for easier management of LaTeX projects.",
            aliases: [$"{App.Name} [OPTIONS] [COMMAND] [ARGS]"],
            flagOptions:
            [
                new CommandCallFlagOption
                {
                    Flag = null,
                    Option = "help",
                    Description = "Print this help message.",
                    HasValue = false,
                    IsMandatory = false,
                },
                new CommandCallFlagOption
                {
                    Flag = 'v',
                    Option = "version",
                    Description = "Print the version of the tool.",
                    HasValue = false,
                    IsMandatory = false,
                },
            ],
            commands:
            [
                new NewCommand([]).GetConvention(),
                new CheckCommand([]).GetConvention(),
                new TemplateCommand([]).GetConvention(),
                new CommandManagement([]).GetConvention(),
                ..conventions,
            ],
            arguments: [],
            commandFactory: args => new RootCommand(args));
    }
}
