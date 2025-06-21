internal sealed class VersionTool : ITool
{
    public ValueTask Execute(Out outs)
    {
        outs.WriteLn(App.GetVersion());
        return ValueTask.CompletedTask;
    }

    public void PrintHelp(Out outs)
    {
        throw new NotSupportedException();
    }
}
