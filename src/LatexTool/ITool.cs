internal interface ITool
{
    ValueTask Execute(Out outs);
    void PrintHelp(Out outs);
}
