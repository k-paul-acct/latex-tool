using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

namespace LatexTool.Lib;

public abstract class CommandBase
{
    private readonly App.IArgToken[] _args;

    public CommandBase(App.IArgToken[] args)
    {
        _args = args;
    }

    public abstract CommandCallConvention GetConvention();
    protected abstract ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult);

    protected virtual void PrintHelp(Out outs, CommandCallConvention convention)
    {
        convention.PrintHelp(outs);
    }

    public ValueTask<int> Execute(Out outs)
    {
        var convention = GetConvention();
        var parsingResult = convention.ParseCliArguments(_args);

        if (!parsingResult.IsValid)
        {
            outs.WriteLn("Errors:");
            foreach (var error in parsingResult.Errors)
            {
                outs.WriteLn($"  {error}");
            }

            outs.WriteLn();
            outs.WriteLn($"For more information, run:");
            outs.WriteLn($"  {convention.FullName} --help");
            return ValueTask.FromResult(1);
        }

        if (parsingResult.ContainsOption("help"))
        {
            PrintHelp(outs, convention);
            return ValueTask.FromResult(0);
        }

        return Execute(outs, parsingResult);
    }
}
