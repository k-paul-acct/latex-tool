using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-template-add", $"{App.Name}-template")]
internal sealed class AddTemplateCommand : CommandBase
{
    public AddTemplateCommand(App.IArgToken[] args) : base(args)
    {
    }

    protected override ValueTask Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        var name = parsingResult.GetArgumentValue("NAME");
        var dllPath = parsingResult.GetArgumentValue("DLL");

        var templatesDir = TemplateCommand.GetTemplateDirectory();
        var templatePath = Path.Combine(templatesDir, name + ".dll");

        File.Copy(dllPath, templatePath, overwrite: true);
        outs.WriteLn($"Template '{name}' has been successfully added.");
        return ValueTask.CompletedTask;
    }

    public override CommandCallConvention GetConvention()
    {
        return new CommandCallConvention(
            name: "add",
            fullName: $"{App.Name} template add",
            description: "Add a new LaTeX template.",
            aliases: [$"{App.Name} template add [NAME] [DLL]"],
            flagOptions: [],
            commands: [],
            arguments:
            [
                new CommandCallArgument
                {
                    Name = "NAME",
                    Description = "The name of the template to add.",
                    IsMandatory = true,
                },
                new CommandCallArgument
                {
                    Name = "DLL",
                    Description = "The path to the DLL file containing the template implementation.",
                    IsMandatory = true,
                },
            ],
            commandFactory: args => new AddTemplateCommand(args));
    }
}
