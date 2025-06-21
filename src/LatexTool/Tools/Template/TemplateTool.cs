internal sealed class TemplateTool : ITool
{
    public TemplateTool(ReadOnlySpan<string> args)
    {
    }

    public ValueTask Execute(Out outs)
    {
        throw new NotImplementedException();
    }

    public void PrintHelp(Out outs)
    {
        throw new NotImplementedException();
    }
} 
