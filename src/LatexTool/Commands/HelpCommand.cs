using LatexTool.Lib;
using LatexTool.Lib.IO;

[Command($"{App.Name}-help")]
internal sealed class HelpCommand : CommandBase
{
    public HelpCommand(App.IArgToken[] args) : base(args)
    {
    }

    public override ValueTask Execute(Out outs)
    {
        outs.WriteLn("Usage: textool [OPTIONS] COMMAND [ARGS]");
        outs.WriteLn();

        outs.WriteLn("A tool for easier management of LaTeX projects.");
        outs.WriteLn();

        outs.WriteLn("Commands:");
        App.PrintCommandsDescription(outs,
        [
            new App.CommandHelpInfo
            {
                Name = "new",
                Description = "Create a new LaTeX project."
            },
            new App.CommandHelpInfo
            {
                Name = "check",
                Description = "Check the output and logs of a LaTeX project build."
            },
            new App.CommandHelpInfo
            {
                Name = "template",
                Description = "Manage LaTex templates."
            },
        ]);
        outs.WriteLn();

        outs.WriteLn("Global options:");
        App.PrintOptionsDescription(outs,
        [
            new App.OptionHelpInfo
            {
                ShortName = null,
                LongName = "help",
                Description = "Print this help message."
            },
            new App.OptionHelpInfo
            {
                ShortName = 'v',
                LongName = "version",
                Description = "Print the version of the tool."
            }
        ]);
        outs.WriteLn();

        outs.WriteLn("Run 'textool COMMAND --help' for more information on a command.");
        return ValueTask.CompletedTask;
    }
}
