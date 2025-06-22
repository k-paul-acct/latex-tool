using LatexTool.Lib;
using LatexTool.Lib.IO;

[Command($"{App.Name}-command-list")]
internal sealed class CommandManagementList : CommandBase
{
    public CommandManagementList(App.IArgToken[] args) : base(args)
    {
        if (args.Length != 0)
        {
            throw new ArgumentException("no arguments expected");
        }
    }

    public override ValueTask Execute(Out outs)
    {
        var commandsDir = App.GetCommandsDirectory();
        var files = Directory.GetFiles(commandsDir, "*.dll");

        if (files.Length == 0)
        {
            outs.WriteLn("No commands found.");
            return ValueTask.CompletedTask;
        }

        foreach (var file in files)
        {
            outs.WriteLn(Path.GetFileNameWithoutExtension(file));
        }

        return ValueTask.CompletedTask;
    }
}
