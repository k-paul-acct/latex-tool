using LatexTool.Lib;
using LatexTool.Lib.IO;

var argTokens = App.ParseArgs(args).ToArray();
var outs = new Out();
var command = new RootCommand(argTokens);

try
{
    await command.Execute(outs);
    return 0;
}
catch (Exception ex)
{
    outs.WriteLn($"Error: {ex.Message}");
    return 1;
}
