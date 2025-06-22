using LatexTool.Lib;
using LatexTool.Lib.IO;

[Command($"{App.Name}-version")]
internal sealed class VersionCommand : CommandBase
{
    public VersionCommand(App.IArgToken[] args) : base(args)
    {
    }

    public override ValueTask Execute(Out outs)
    {
        outs.WriteLn(App.GetVersion());
        return ValueTask.CompletedTask;
    }
}
