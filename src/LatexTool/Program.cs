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

static ITool ParseArgs(string[] args)
{
    using var enumerator = App.ParseArgs(args).GetEnumerator();

    while (enumerator.MoveNext())
    {
        var token = enumerator.Current;

        if (token is App.Flag { Value: 'h' } or App.Option { Value: "help" })
        {
            return new HelpTool();
        }
        else if (token is App.Flag { Value: 'v' } or App.Option { Value: "version" })
        {
            return new VersionTool();
        }
        else if (token is App.Word word)
        {
            var rest = enumerator.Rest();
            switch (word.Value)
            {
                case "new":
                    return new NewTool(rest);
                case "check":
                    return new CheckTool(rest);
                case "template":
                    return new TemplateTool(rest);
                default:
                    break;
            }
        }

        App.UnknownCliArgument(token);
    }

    return new HelpTool();
}
