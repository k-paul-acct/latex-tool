namespace LatexTool.Lib.Convention;

public sealed class CommandCallArgument
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool IsMandatory { get; init; }
}
