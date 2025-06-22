using LatexTool.Lib;
using LatexTool.Lib.IO;

[Command($"{App.Name}-template-add")]
internal sealed class AddTemplateCommand : CommandBase
{
    private readonly string _templateName;
    private readonly string _templatePath;

    public AddTemplateCommand(App.IArgToken[] args) : base(args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("not enough arguments provided");
        }

        _templateName = args[0].StringValue;
        _templatePath = args[1].StringValue;
    }

    public override ValueTask Execute(Out outs)
    {
        var templateDir = TemplateCommand.GetTemplateDirectory();
        var templatePath = Path.Combine(templateDir, _templateName + ".dll");
        File.Copy(_templatePath, templatePath, overwrite: true);
        outs.WriteLn($"Template '{_templateName}' has been successfully added.");
        return ValueTask.CompletedTask;
    }
}
