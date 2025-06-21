internal sealed class ToolException : Exception
{
    public ToolException(string toolName, string message) : base(message)
    {
        ToolName = toolName;
    }

    public string ToolName { get; }
}
