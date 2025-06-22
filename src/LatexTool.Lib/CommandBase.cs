using System.Reflection;
using LatexTool.Lib.IO;

namespace LatexTool.Lib;

public abstract class CommandBase
{
    protected readonly App.IArgToken[] _args;

    public CommandBase(App.IArgToken[] args)
    {
        _args = args;
    }

    public static CommandBase GetCommandDefaultStrategy(App.IArgToken[] args, string parentCommand)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("no arguments provided");
        }

        var token = args[0];
        var commandName = $"{parentCommand}-{token.StringValue}";
        var commandsDir = App.GetCommandsDirectory();
        var commandPath = Path.Combine(commandsDir, commandName + ".dll");

        if (File.Exists(commandPath))
        {
            var assembly = Assembly.LoadFile(commandPath);
            var commandType = assembly
                .GetExportedTypes()
                .FirstOrDefault(t => typeof(CommandBase).IsAssignableFrom(t) &&
                                     !t.IsInterface &&
                                     !t.IsAbstract &&
                                     t.GetCustomAttribute<CommandAttribute>()?.Name == commandName);

            if (commandType is not null)
            {
                var methodInfo = commandType.GetMethod(
                    "GetCommand",
                    BindingFlags.Public | BindingFlags.Static);

                if (methodInfo is not null)
                {
                    var command = methodInfo.Invoke(null, args[1..]) as CommandBase;
                    if (command is not null)
                    {
                        return command;
                    }
                }
            }
        }

        App.UnknownCliArgument(token);
        return null!;
    }

    public abstract ValueTask Execute(Out outs);
}
