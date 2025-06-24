using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

[Command($"{App.Name}-publish", App.Name)]
internal sealed class PublishCommand : CommandBase
{
    public PublishCommand(App.ArgToken[] args) : base(args)
    {
    }

    protected override async ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        var mainFile = parsingResult.GetArgumentValue("MAIN");
        var mainFileName = Path.GetFileNameWithoutExtension(mainFile);
        var logfileName = mainFileName + ".log";
        var pdfName = mainFileName + ".pdf";
        var optimize = parsingResult.GetFlagValue("optimize");
        var clean = parsingResult.GetFlagValue("clean");

        outs.WriteLn($"Publishing LaTeX project...");

        var code = await Compile(mainFile, logfileName, outs);
        if (code != 0)
        {
            return code;
        }

        if (optimize)
        {
            outs.WriteLn();
            outs.WriteLn("Optimizing the PDF using qpdf...");
            var optimizeCode = Optimize(pdfName);
            if (optimizeCode != 0)
            {
                return optimizeCode;
            }
        }

        if (clean)
        {
            outs.WriteLn();
            outs.WriteLn("Cleaning up temporary files...");
            var cleanCode = Clean();
            if (cleanCode != 0)
            {
                return cleanCode;
            }
        }

        outs.WriteLn();
        outs.WriteLn("Publishing completed successfully.", ConsoleFontStyle.Bold, ConsoleColor.Green);

        return 0;
    }

    private static async ValueTask<int> Compile(string mainFile, string logfileName, Out outs)
    {
        using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "latexmk",
            ArgumentList =
            {
                "-lualatex",
                "-silent",
                "-f",
                "-Werror",
                "-file-line-error",
                "-interaction=nonstopmode",
                "-halt-on-error",
                "-logfilewarninglist",
                mainFile
            },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }) ?? throw new InvalidOperationException("Failed to start latexmk process.");

        var checkCommand = new CheckCommand([logfileName, "--pipe-input"], process.StandardOutput.EnumerateAllLines());
        var checkExitCode = await checkCommand.Execute(outs);

        process.WaitForExit();

        return process.ExitCode == 0 && checkExitCode == 0 ? 0 : 1;
    }

    private static int Optimize(string pdfFile)
    {
        using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "qpdf",
            ArgumentList = { pdfFile, "--linearize", "--replace-input" },
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true
        }) ?? throw new InvalidOperationException("Failed to start qpdf process.");

        process.WaitForExit();

        return process.ExitCode;
    }

    private static int Clean()
    {
        using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "latexmk",
            ArgumentList = { "-c" },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }) ?? throw new InvalidOperationException("Failed to start latexmk process.");

        process.WaitForExit();

        return process.ExitCode;
    }

    public override CommandCallConvention GetConvention()
    {
        return new CommandCallConvention(
            name: "publish",
            fullName: $"{App.Name} publish",
            description: "Publish a LaTeX project.",
            aliases: [$"{App.Name} publish [MAIN] [OPTIONS]"],
            flagOptions:
            [
                new CommandCallFlagOption
                {
                    Flag = null,
                    Option = "optimize",
                    Description = "Use qpdf for optimization of the resulting PDF.",
                    HasValue = false,
                    IsMandatory = false,
                },
                new CommandCallFlagOption
                {
                    Flag = null,
                    Option = "clean",
                    Description = "Clean up temporary files after publishing.",
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
                    Name = "MAIN",
                    Description = "Path to the main LaTeX file to publish.",
                    IsMandatory = true,
                },
            ],
            commandFactory: args => new PublishCommand(args));
    }
}
