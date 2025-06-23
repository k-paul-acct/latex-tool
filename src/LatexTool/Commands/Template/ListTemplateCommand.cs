using System.Reflection;
using LatexTool.Lib;
using LatexTool.Lib.IO;

[Command($"{App.Name}-template-list")]
internal sealed class ListTemplateCommand : CommandBase
{
    public ListTemplateCommand(App.IArgToken[] args) : base(args)
    {
        if (args.Length > 0)
        {
            throw new ArgumentException("no arguments expected");
        }
    }

    public override ValueTask Execute(Out outs)
    {
        var templateDir = TemplateCommand.GetTemplateDirectory();
        var files = Directory.GetFiles(templateDir, "*.dll");

        if (files.Length == 0)
        {
            outs.WriteLn("No templates found.");
            return ValueTask.CompletedTask;
        }

        foreach (var (file, version) in files.Select(x => (x, Assembly.LoadFile(x).GetVersion())))
        {
            outs.WriteLn($"{Path.GetFileNameWithoutExtension(file)} {version}");
        }

        return ValueTask.CompletedTask;
    }
}
