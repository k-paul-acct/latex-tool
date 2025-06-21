internal sealed class VersionTool : ITool
{
    public ValueTask Execute(Out outs)
    {
        outs.WriteLn(App.Version);
        return ValueTask.CompletedTask;
    }
}
