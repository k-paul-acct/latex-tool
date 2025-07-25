using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-command", App.Name)]
internal sealed class CommandManagement : CommandBase
{
    public CommandManagement(App.ArgToken[] args) : base(args)
    {
    }

    protected override ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        if (parsingResult.Command is not null)
        {
            return parsingResult.Command.Value.Item2.Execute(outs);
        }

        return ValueTask.FromResult(0);
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
                new CommandManagementRemove([]).GetConvention(),
            ],
            commandIsMandatory: true,
            arguments: [],
            commandFactory: args => new CommandManagement(args));
    }
}
