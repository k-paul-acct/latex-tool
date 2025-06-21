internal static class App
{
    public static Version GetVersion()
    {
        var version = typeof(Program).Assembly.GetName().Version;
        return version is null
            ? new Version(1, 0, 0)
            : new Version(version.Major, version.Minor, version.Build);
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

    public readonly struct Version
    {
        public readonly int Major;
        public readonly int Minor;
        public readonly int Build;

        public Version(int major, int minor, int build)
        {
            Major = major;
            Minor = minor;
            Build = build;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}";
        }
    }
}
