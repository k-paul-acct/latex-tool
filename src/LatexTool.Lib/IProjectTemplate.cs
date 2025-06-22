namespace LatexTool.Lib;

public interface IProjectTemplate
{
    string Name { get; }
    string Description { get; }
    ValueTask EmitFiles(string? outputDir);
}
