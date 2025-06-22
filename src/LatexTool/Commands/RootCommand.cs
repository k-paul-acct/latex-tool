using LatexTool.Lib;
using LatexTool.Lib.IO;

[Command(App.Name)]
internal sealed class RootCommand : CommandBase
{
    public RootCommand(App.IArgToken[] args) : base(args)
    {
    }

    public static CommandBase GetCommand(App.IArgToken[] args)
    {
        using var enumerator = args.AsEnumerable().GetEnumerator();

        while (enumerator.MoveNext())
        {
            var token = enumerator.Current;

            if (token is App.Flag { Value: 'h' } or App.Option { Value: "help" })
            {
                return new HelpCommand([]);
            }
            else if (token is App.Flag { Value: 'v' } or App.Option { Value: "version" })
            {
                return new VersionCommand([]);
            }
            else if (token is App.Word word)
            {
                var rest = enumerator.Rest();
                return word.Value switch
                {
                    "new" => new NewCommand(rest),
                    "check" => new CheckCommand(rest),
                    "command" => CommandManagement.GetCommand(rest),
                    "template" => TemplateCommand.GetCommand(rest),
                    _ => GetCommandDefaultStrategy([token, .. rest], App.Name),
                };
            }

            App.UnknownCliArgument(token);
        }

        return new HelpCommand([]);
    }

    public override ValueTask Execute(Out outs)
    {
        throw new InvalidOperationException();
    }
}
