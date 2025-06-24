using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-check", App.Name)]
internal sealed class CheckCommand : CommandBase
{
    private readonly IEnumerable<string>? _inputLines;

    public CheckCommand(App.ArgToken[] args) : base(args)
    {
    }

    public CheckCommand(App.ArgToken[] args, IEnumerable<string> inputLines) : base(args)
    {
        _inputLines = inputLines;
    }

    protected override ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        var filename = parsingResult.GetArgumentValue("FILENAME");
        var pipeInput = parsingResult.ContainsOption("pipe-input");

        var result = LogAnalysisResult.Success;

        if (pipeInput)
        {
            var piper = new Piper(_inputLines ?? In.EnumerateAllLines(), outs);
            result |= piper.PipeInput();
        }

        var parser = new Parser(File.ReadAllLines(filename), outs);
        result |= parser.Parse();

        if ((result & LogAnalysisResult.Errors) != 0)
        {
            outs.WriteLn("Check failed with errors.", ConsoleFontStyle.Bold, ConsoleColor.Red);
        }
        else if ((result & LogAnalysisResult.Warnings) != 0)
        {
            outs.WriteLn("Check completed with warnings.", ConsoleFontStyle.Bold, ConsoleColor.Yellow);
        }
        else if ((result & LogAnalysisResult.Success) != 0)
        {
            outs.WriteLn("Check completed successfully.", ConsoleFontStyle.Bold, ConsoleColor.Green);
        }

        return ValueTask.FromResult(0);
    }

    public override CommandCallConvention GetConvention()
    {
        return new CommandCallConvention(
            name: "check",
            fullName: $"{App.Name} check",
            description: "Checks the output and logs of a LaTeX project build.",
            aliases: ["textool check [FILENAME] [OPTIONS]"],
            flagOptions:
            [
                new CommandCallFlagOption
                {
                    Flag = 'p',
                    Option = "pipe-input",
                    Description = "Pipeline standard input to standard output with important information highlighted.",
                    HasValue = false,
                    IsMandatory = false,
                },
            ],
            commands: [],
            commandIsMandatory: false,
            arguments:
            [
                new CommandCallArgument
                {
                    Name = "FILENAME",
                    Description = "The path to the log file to check.",
                    IsMandatory = true,
                }
            ],
            commandFactory: args => new CheckCommand(args));
    }
}
