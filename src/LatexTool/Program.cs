var outs = new Out();

try
{
    var tool = ParseArgs(args, out var helpOnly);

    if (helpOnly)
    {
        tool.PrintHelp(outs);
    }
    else
    {
        await tool.Execute(outs);
    }
}
catch (Exception ex)
{
    outs.WriteLn($"error: {ex.Message}");
    var helpTool = new HelpTool();
    await helpTool.Execute(outs);
}

static ITool ParseArgs(ReadOnlySpan<string> args, out bool helpOnly)
{
    helpOnly = false;

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

        var toolArgs = args[(i + 1)..];

        if (toolArgs.Contains("-h") || toolArgs.Contains("--help"))
        {
            helpOnly = true;
        }

        if (arg == "new")
        {
            return new NewTool(toolArgs);
        }

        if (arg == "check")
        {
            return new CheckTool(toolArgs);
        }

        if (arg == "template")
        {
            return new TemplateTool(toolArgs);
        }

        App.UnknownCliArgument(arg);
    }

    return new HelpTool();
}
