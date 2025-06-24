using System.Reflection;
using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-command-list", $"{App.Name}-command")]
internal sealed class CommandManagementList : CommandBase
{
    public CommandManagementList(App.ArgToken[] args) : base(args)
    {
    }

    protected override ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        var commandsDir = App.GetCommandsDirectory();
        var files = Directory.GetFiles(commandsDir, "*.dll");

        if (files.Length == 0)
        {
            outs.WriteLn("No commands found.");
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
            fullName: $"{App.Name} command list",
            description: "List all added custom commands.",
            aliases: [$"{App.Name} command list"],
            flagOptions: [],
            commands: [],
            commandIsMandatory: false,
            arguments: [],
            commandFactory: args => new CommandManagementList(args));
    }
}
