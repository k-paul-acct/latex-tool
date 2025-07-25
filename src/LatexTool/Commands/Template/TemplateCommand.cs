using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-template", App.Name)]
internal sealed class TemplateCommand : CommandBase
{
    public TemplateCommand(App.ArgToken[] args) : base(args)
    {
    }

    public static string GetTemplateDirectory()
    {
        var templateDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            App.Name,
            "templates");
        Directory.CreateDirectory(templateDir);
        return templateDir;
    }

    protected override ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        if (parsingResult.Command is not null)
        {
            return parsingResult.Command.Value.Item2.Execute(outs);
        }

        return ValueTask.FromResult(0);
    }

    public override CommandCallConvention GetConvention()
    {
        return new CommandCallConvention(
            name: "template",
            fullName: $"{App.Name} template",
            description: "Manage LaTex templates.",
            aliases: [$"{App.Name} template [COMMAND] [OPTIONS]"],
            flagOptions:
            [
                CommandCallConvention.SubCommandHelpOption,
            ],
            commands:
            [
                new AddTemplateCommand([]).GetConvention(),
                new ListTemplateCommand([]).GetConvention(),
            ],
            commandIsMandatory: true,
            arguments: [],
            commandFactory: args => new TemplateCommand(args));
    }
}
