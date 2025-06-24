using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-command-add", $"{App.Name}-command")]
internal sealed class CommandManagementAdd : CommandBase
{
    public CommandManagementAdd(App.IArgToken[] args) : base(args)
    {
    }

    protected override ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        var name = parsingResult.GetArgumentValue("NAME");
        var dllPath = parsingResult.GetArgumentValue("DLL");

        var commandsDir = App.GetCommandsDirectory();
        var commandPath = Path.Combine(commandsDir, name + ".dll");

        File.Copy(dllPath, commandPath, overwrite: true);
        outs.WriteLn($"Command '{name}' has been successfully added.");
        return ValueTask.FromResult(0);
    }

    public override CommandCallConvention GetConvention()
    {
        return new CommandCallConvention(
            name: "add",
            fullName: $"{App.Name} command add",
            description: "Add a new command to the tool.",
            aliases: [$"{App.Name} command add [NAME] [DLL]"],
            flagOptions: [],
            commands: [],
            commandIsMandatory: false,
            arguments:
            [
                new CommandCallArgument
                {
                    Name = "NAME",
                    Description = "The name of the command to add.",
                    IsMandatory = true,
                },
                new CommandCallArgument
                {
                    Name = "DLL",
                    Description = "The path to the DLL file containing the command implementation.",
                    IsMandatory = true,
                },
            ],
            commandFactory: args => new CommandManagementAdd(args));
    }
}
