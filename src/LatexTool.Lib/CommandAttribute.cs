namespace LatexTool.Lib;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CommandAttribute : Attribute
{
    public string Name { get; }
    public string? ParentCommand { get; }

    public CommandAttribute(string name, string? parentCommand)
    {
        Name = name;
        ParentCommand = parentCommand;
    }
}
