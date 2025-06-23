namespace LatexTool.Lib.Convention;

public sealed class CommandCallParsingResult
{
    public required List<(CommandCallFlagOption FlagOption, string? Value)> FlagOptions { get; init; }
    public required (string, CommandBase)? Command { get; init; }
    public required List<(CommandCallArgument, string)> Arguments { get; init; }
    public required List<string> Errors { get; init; }
    public required App.IArgToken[] Rest { get; init; }
    public bool IsValid => Errors.Count == 0;
    public bool IsHelpRequested => FlagOptions.Any(fo => fo.FlagOption.Option == "help");

    public bool ContainsOption(App.Option option)
    {
        return FlagOptions.Any(fo => fo.FlagOption.Option == option);
    }

    public string GetArgumentValue(string name)
    {
        return Arguments.First(a => a.Item1.Name == name).Item2;
    }

    public string? GetOptionValue(App.Option option)
    {
        return FlagOptions.FirstOrDefault(fo => fo.FlagOption.Option == option).Value;
    }
}
