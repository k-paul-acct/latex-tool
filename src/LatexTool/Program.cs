var outs = new Out();

try
{
    var tool = ParseArgs(args);
    await tool.Execute(outs);
}
catch (Exception ex)
{
    outs.WriteLn($"error: {ex.Message}");
    var helpTool = new HelpTool();
    await helpTool.Execute(outs);
}

static ITool ParseArgs(ReadOnlySpan<string> args)
{
    for (var i = 0; i < args.Length; ++i)
    {
        var arg = args[i];

        if (arg == "-v" || arg == "--version")
        {
            return new VersionTool();
        }

        if (arg == "-h" || arg == "--help")
        {
            return new HelpTool();
        }

        if (arg == "new")
        {
            return new NewTool(args[(i + 1)..]);
        }

        if (arg == "check")
        {
            return new CheckTool(args[(i + 1)..]);
        }

        App.UnknownCliArgument(arg);
    }

    return new HelpTool();
}
