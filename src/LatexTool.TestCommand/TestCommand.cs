﻿using LatexTool.Lib;
using LatexTool.Lib.Convention;
using LatexTool.Lib.IO;

namespace LatexTool.TestCommand;

[Command($"{App.Name}-test", App.Name)]
public sealed class TestCommand : CommandBase
{
    public TestCommand(App.ArgToken[] args) : base(args)
    {
    }

    protected override ValueTask<int> Execute(Out outs, CommandCallParsingResult parsingResult)
    {
        outs.WriteLn("This is a test command.");
        return ValueTask.FromResult(0);
    }

    public override CommandCallConvention GetConvention()
    {
        return new CommandCallConvention(
            name: "test",
            fullName: $"{App.Name} test",
            description: "A test command for LatexTool.",
            aliases: [$"{App.Name} test"],
            flagOptions: [],
            commands: [],
            commandIsMandatory: false,
            arguments: [],
            commandFactory: args => new TestCommand(args));
    }
}
