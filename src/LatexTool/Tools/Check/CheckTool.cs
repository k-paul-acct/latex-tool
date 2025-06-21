internal sealed class CheckTool : ITool
{
    private readonly string _filename;
    private readonly bool _pipeInput;

    public CheckTool(ReadOnlySpan<string> args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("no arguments provided");
        }

        var filename = args[0];
        var pipeInput = false;

        for (var i = 1; i < args.Length; ++i)
        {
            var arg = args[i];

            if (arg == "-p" || arg == "--pipe-input")
            {
                pipeInput = true;
                continue;
            }

            App.UnknownCliArgument(arg);
        }

        _filename = filename;
        _pipeInput = pipeInput;
    }

    public ValueTask Execute(Out outs)
    {
        var result = LogAnalysisResult.Success;

        if (_pipeInput)
        {
            var piper = new Piper(In.EnumerateAllLines(), outs);
            result |= piper.PipeInput();
        }

        var parser = new Parser(File.ReadAllLines(_filename), outs);
        result |= parser.Parse();

        if ((result & LogAnalysisResult.Errors) != 0)
        {
            outs.WriteLn();
            outs.WriteLn("Check failed with errors.", ConsoleFontStyle.Bold, ConsoleColor.Red);
        }
        else if ((result & LogAnalysisResult.Warnings) != 0)
        {
            outs.WriteLn();
            outs.WriteLn("Check completed with warnings.", ConsoleFontStyle.Bold, ConsoleColor.Yellow);
        }
        else if ((result & LogAnalysisResult.Success) != 0)
        {
            outs.WriteLn();
            outs.WriteLn("Check completed successfully.", ConsoleFontStyle.Bold, ConsoleColor.Green);
        }

        return ValueTask.CompletedTask;
    }

    public void PrintHelp(Out outs)
    {
        outs.WriteLn("Description:");
        outs.WriteLn("  Checks the output and logs of a LaTeX project build.");
        outs.WriteLn();

        outs.WriteLn("Usage:");
        outs.WriteLn("  textool check FILENAME [OPTIONS]");
        outs.WriteLn();

        outs.WriteLn("Arguments:");
        outs.WriteLn("  FILENAME  The path to the log file to check.");
        outs.WriteLn();

        outs.WriteLn("Options:");
        App.PrintOptionsDescription(outs,
        [
            new App.OptionHelpInfo
            {
                ShortName = 'p',
                LongName = "pipe-input",
                Description = "Pipeline standard input to standard output with important information highlighted."
            }
        ]);
    }
}
