using System.Reflection;
using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-new", App.Name)]
internal sealed class NewCommand : CommandBase
{
    public NewCommand(App.ArgToken[] args) : base(args)
    {
    }

    protected override async ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        var template = parsingResult.GetArgumentValue("TEMPLATE");
        var output = parsingResult.GetOptionValue("output");

        var templatesDir = TemplateCommand.GetTemplateDirectory();
        var templatePath = Path.Combine(templatesDir, template + ".dll");

        if (!File.Exists(templatePath))
        {
            throw new InvalidOperationException($"Template '{template}' not found.");
        }

        outs.WriteLn($"Creating a new '{template}' LaTeX project...");

        var assembly = Assembly.LoadFile(templatePath);
        var templateType = assembly
            .GetExportedTypes()
            .FirstOrDefault(t => typeof(IProjectTemplate).IsAssignableFrom(t) &&
                                 !t.IsInterface &&
                                 !t.IsAbstract) ??
            throw new InvalidOperationException($"Error while creating a project.");

        var projectTemplate = (IProjectTemplate?)Activator.CreateInstance(templateType) ??
                              throw new InvalidOperationException($"Error while creating a project.");

        await projectTemplate.EmitFiles(output);

        outs.WriteLn($"Project created successfully.");
        return 0;
    }

    public override CommandCallConvention GetConvention()
    {
        return new CommandCallConvention(
            name: "new",
            fullName: $"{App.Name} new",
            description: "Create a new LaTeX project.",
            aliases: [$"{App.Name} new [TEMPLATE] [OPTIONS]"],
            flagOptions:
            [
                new CommandCallFlagOption
                {
                    Flag = 'o',
                    Option = "output",
                    Description = "Output directory for the new project.",
                    HasValue = true,
                    IsMandatory = false,
                },
            ],
            commands: [],
            commandIsMandatory: false,
            arguments:
            [
                new CommandCallArgument
                {
                    Name = "TEMPLATE",
                    Description = "The template to use for the new project.",
                    IsMandatory = true,
                },
            ],
            commandFactory: args => new NewCommand(args));
    }
}
