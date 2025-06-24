using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-command-remove", $"{App.Name}-command")]
internal sealed class CommandManagementRemove : CommandBase
{
    public CommandManagementRemove(App.ArgToken[] args) : base(args)
    {
    }

    protected override ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        var name = parsingResult.GetArgumentValue("NAME");

        var commandsDir = App.GetCommandsDirectory();
        var commandPath = Path.Combine(commandsDir, name + ".dll");

        File.Delete(commandPath);
        outs.WriteLn($"Command '{name}' has been successfully removed.");
        return ValueTask.FromResult(0);
    }

    public override CommandCallConvention GetConvention()
    {
        return new CommandCallConvention(
            name: "remove",
            fullName: $"{App.Name} command remove",
            description: "Removes a command from the tool.",
            aliases: [$"{App.Name} command remove [NAME]"],
            flagOptions: [],
            commands: [],
            commandIsMandatory: false,
            arguments:
            [
                new CommandCallArgument
                {
                    Name = "NAME",
                    Description = "The name of the command to remove.",
                    IsMandatory = true,
                },
            ],
            commandFactory: args => new CommandManagementRemove(args));
    }
}
