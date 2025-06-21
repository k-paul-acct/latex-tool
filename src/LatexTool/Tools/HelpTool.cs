internal sealed class HelpTool : ITool
{
    public ValueTask Execute(Out outs)
    {
        outs.WriteLn("usage: textool [-v | --version] [-h | --help] <command> [<args>]");
        return ValueTask.CompletedTask;
    }
}
