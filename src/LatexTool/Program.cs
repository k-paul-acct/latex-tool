using LatexTool.Lib;
using LatexTool.Lib.IO;

var argTokens = App.ParseArgs(args).ToArray();
var command = new RootCommand(argTokens);
var outs = new Out();

try
{
    return await command.Execute(outs);
}
catch (Exception ex)
{
    outs.WriteLn($"Error: {ex.Message}");
    return 1;
}
