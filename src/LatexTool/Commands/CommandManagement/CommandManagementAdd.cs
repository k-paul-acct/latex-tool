using LatexTool.Lib;
using LatexTool.Lib.IO;

[Command($"{App.Name}-command-add")]
internal sealed class CommandManagementAdd : CommandBase
{
    private readonly string _name;
    private readonly string _dllPath;

    public CommandManagementAdd(App.IArgToken[] args) : base(args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("Command name and DLL path are required.");
        }

        _name = args[0].StringValue;
        _dllPath = args[1].StringValue;
    }

    public override ValueTask Execute(Out outs)
    {
        var commandDir = App.GetCommandsDirectory();
        var parts = _name.Split('-');
        var fullPath = parts.Length > 1 ? Path.Combine([commandDir, .. parts[..^1]]) : commandDir;
        var commandFile = Path.Combine(fullPath, $"{parts[^1]}.dll");

        Directory.CreateDirectory(fullPath);

        File.Copy(_dllPath, commandFile, true);
        outs.WriteLn($"Command '{_name}' has been successfully added.");
        return ValueTask.CompletedTask;
    }
}
