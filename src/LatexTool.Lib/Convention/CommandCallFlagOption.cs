namespace LatexTool.Lib.Convention;

public sealed class CommandCallFlagOption
{
    public required App.Flag? Flag { get; init; }
    public required App.Option? Option { get; init; }
    public required string Description { get; init; }
    public required bool HasValue { get; init; }
    public required bool IsMandatory { get; init; }
}
