internal readonly struct AppVersion
{
    public readonly int Major;
    public readonly int Minor;
    public readonly int Build;

    public AppVersion(int major, int minor, int build)
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
