internal sealed class NewTool : ITool
{
    public NewTool(ReadOnlySpan<string> args)
    {

    }

    public ValueTask Execute(Out outs)
    {
        outs.WriteLn("Creating a new LaTeX project...");
        return ValueTask.CompletedTask;
    }
}
