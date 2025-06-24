using System.Reflection;
using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-template-list", $"{App.Name}-template")]
internal sealed class ListTemplateCommand : CommandBase
{
    public ListTemplateCommand(App.ArgToken[] args) : base(args)
    {
    }

    protected override ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        var templateDir = TemplateCommand.GetTemplateDirectory();
        var files = Directory.GetFiles(templateDir, "*.dll");

        if (files.Length == 0)
        {
            outs.WriteLn("No templates found.");
            return ValueTask.FromResult(0);
        }

        foreach (var (file, version) in files.Select(x => (x, Assembly.LoadFile(x).GetVersion())))
        {
            outs.WriteLn($"{Path.GetFileNameWithoutExtension(file)} {version}");
        }

        return ValueTask.FromResult(0);
    }

    public override CommandCallConvention GetConvention()
    {
        return new CommandCallConvention(
            name: "list",
            fullName: $"{App.Name} template list",
            description: "List all templates.",
            aliases: [$"{App.Name} template list"],
            flagOptions: [],
            commands: [],
            commandIsMandatory: false,
            arguments: [],
            commandFactory: args => new ListTemplateCommand(args));
    }
}
