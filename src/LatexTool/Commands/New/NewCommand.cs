using System.Reflection;
using LatexTool.Lib;
using LatexTool.Lib.IO;

[Command($"{App.Name}-new")]
internal sealed class NewCommand : CommandBase
{
    private readonly string _template;
    private readonly string? _output;

    public NewCommand(App.IArgToken[] args) : base(args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("no arguments provided");
        }

        var template = args[0].StringValue;
        string? output = null;

        for (var i = 1; i < args.Length; ++i)
        {
            var arg = args[i];

            if (arg is App.Flag { Value: 'o' } or App.Option { Value: "output" })
            {
                output = ++i < args.Length
                    ? args[i].StringValue
                    : throw new ArgumentException("no output directory provided");
                continue;
            }

            App.UnknownCliArgument(arg);
        }

        _template = template;
        _output = output;
    }

    public override async ValueTask Execute(Out outs)
    {
        var templatesDir = TemplateCommand.GetTemplateDirectory();
        var templatePath = Path.Combine(templatesDir, _template + ".dll");

        if (!File.Exists(templatePath))
        {
            throw new InvalidOperationException($"Template '{_template}' not found.");
        }

        outs.WriteLn($"Creating a new '{_template}' LaTeX project...");

        var assembly = Assembly.LoadFile(templatePath);
        var templateType = assembly
            .GetExportedTypes()
            .FirstOrDefault(t => typeof(IProjectTemplate).IsAssignableFrom(t) &&
                                 !t.IsInterface &&
                                 !t.IsAbstract);

        if (templateType is null)
        {
            throw new InvalidOperationException($"Error while creating a project.");
        }

        var template = (IProjectTemplate?)Activator.CreateInstance(templateType);

        if (template is null)
        {
            throw new InvalidOperationException($"Error while creating a project.");
        }

        await template.EmitFiles(_output);

        outs.WriteLn($"Project created successfully.");
    }
}
