internal static class App
{
    public static AppVersion Version
    {
        get
        {
            var version = typeof(Program).Assembly.GetName().Version;
            return version is null
                ? new AppVersion(1, 0, 0)
                : new AppVersion(version.Major, version.Minor, version.Build);
        }
    }

    public static void UnknownCliArgument(string arg)
    {
        if (arg.StartsWith("--"))
        {
            throw new ArgumentException($"unknown argument '{arg}'");
        }

        if (arg.StartsWith('-'))
        {
            throw new ArgumentException($"unknown option '{arg}'");
        }

        throw new ArgumentException($"unknown command '{arg}'");
    }
}
