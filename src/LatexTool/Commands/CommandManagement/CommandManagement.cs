using LatexTool.Lib;
using LatexTool.Lib.IO;

[Command($"{App.Name}-command")]
internal sealed class CommandManagement : CommandBase
{
    public CommandManagement(App.IArgToken[] args) : base(args)
    {
    }

    public static CommandBase GetCommand(App.IArgToken[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("no arguments provided");
        }

        var command = args[0].StringValue switch
        {
            "add" => new CommandManagementAdd(args[1..]),
            "list" => new CommandManagementList(args[1..]),
            _ => (CommandBase?)null,
        };

        if (command is null)
        {
            App.UnknownCliArgument(args[0]);
            return null!;
        }

        return command;
    }

    public override ValueTask Execute(Out outs)
    {
        throw new InvalidOperationException();
    }
}
