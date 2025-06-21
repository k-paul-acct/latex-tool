internal sealed class NewTool : ITool
{
    private readonly string _template;
    private readonly string? _output;

    public NewTool(ReadOnlySpan<string> args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("no arguments provided");
        }

        var template = args[0];
        string? output = null;

        for (var i = 1; i < args.Length; ++i)
        {
            var arg = args[i];

            if (arg == "-o" || arg == "--output")
            {
                output = ++i < args.Length
                    ? args[i]
                    : throw new ArgumentException("no output directory provided");
                continue;
            }

            App.UnknownCliArgument(arg);
        }

        _template = template;
        _output = output;
    }

    public ValueTask Execute(Out outs)
    {
        outs.WriteLn($"Creating a new '{_template}' LaTeX project...");
        return ValueTask.CompletedTask;
    }
}

internal interface IProjectTemplate
{
    string Name { get; }
    string Description { get; }
    ValueTask EmitFiles(string? outputDir);
}

internal sealed class LabWorkTemplate : IProjectTemplate
{
    public string Name => "Lab Work";
    public string Description => "A template for lab work projects";

    public ValueTask EmitFiles(string? outputDir)
    {
        // Logic to create files for the lab work template
        return ValueTask.CompletedTask;
    }
}
