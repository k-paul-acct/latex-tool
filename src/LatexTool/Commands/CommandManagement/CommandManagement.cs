using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-command", App.Name)]
internal sealed class CommandManagement : CommandBase
{
    public CommandManagement(App.IArgToken[] args) : base(args)
    {
    }

    protected override ValueTask Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        if (parsingResult.Command is not null)
        {
            return parsingResult.Command.Value.Item2.Execute(outs);
        }

        return ValueTask.CompletedTask;
    }

    public override CommandCallConvention GetConvention()
    {
        return new CommandCallConvention(
            name: "command",
            fullName: $"{App.Name} command",
            description: "Additional commands management.",
            aliases: [$"{App.Name} command [COMMAND] [OPTIONS]"],
            flagOptions:
            [
                CommandCallConvention.SubCommandHelpOption,
            ],
            commands:
            [
                new CommandManagementAdd([]).GetConvention(),
                new CommandManagementList([]).GetConvention(),
            ],
            arguments: [],
            commandFactory: args => new CommandManagement(args));
    }
}
