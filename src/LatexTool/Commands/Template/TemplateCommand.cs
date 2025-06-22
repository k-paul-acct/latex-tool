using LatexTool.Lib;
using LatexTool.Lib.IO;

[Command($"{App.Name}-template")]
internal sealed class TemplateCommand : CommandBase
{
    public TemplateCommand(App.IArgToken[] args) : base(args)
    {
    }

    public override ValueTask Execute(Out outs)
    {
        throw new InvalidOperationException();
    }

    public static CommandBase GetCommand(App.IArgToken[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("no arguments provided");
        }

        var command = args[0].StringValue;
        var rest = args[1..];

        if (command == "add")
        {
            return new AddTemplateCommand(rest);
        }

        if (command == "list")
        {
            return new ListTemplateCommand(rest);
        }

        App.UnknownCliArgument(args[0]);
        return null!;
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
}
