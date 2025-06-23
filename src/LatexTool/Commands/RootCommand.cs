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

    private static IEnumerable<CommandCallConvention> GetCommandsFromAssemblies()
    {
        var commandsDir = App.GetCommandsDirectory();
        var files = Directory.GetFiles(commandsDir, "*.dll");

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
                        yield return convention;
                    }
                }
            }
        }
    }

    public override CommandCallConvention GetConvention()
    {
        var conventions = GetCommandsFromAssemblies();
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
            commands: new TwoPartEnumerable<CommandCallConvention>(
                [
                    new NewCommand([]).GetConvention(),
                    new CheckCommand([]).GetConvention(),
                    new TemplateCommand([]).GetConvention(),
                    new CommandManagement([]).GetConvention(),
                ],
                conventions),
            commandIsMandatory: false,
            arguments: [],
            commandFactory: args => new RootCommand(args));
    }

    private sealed class TwoPartEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _first;
        private readonly IEnumerable<T> _second;

        public TwoPartEnumerable(IEnumerable<T> first, IEnumerable<T> second)
        {
            _first = first;
            _second = second;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _first)
            {
                yield return item;
            }

            foreach (var item in _second)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
