using System.Reflection;
using LatexTool.Lib;

namespace LatexTool.Template.LabWork;

public sealed class LabWorkTemplate : IProjectTemplate
{
    public string Name => "Lab Work";
    public string Description => "A template for lab work projects.";

    public ValueTask EmitFiles(string? outputDir)
    {
        const string @namespace = "LatexTool.Template.LabWork.Files.";

        var dir = Directory.GetCurrentDirectory();
        var templateDir = Path.Combine(dir, outputDir ?? "");
        var assembly = Assembly.GetExecutingAssembly();

        Directory.CreateDirectory(templateDir);

        foreach (var resource in assembly.GetManifestResourceNames())
        {
            if (!resource.StartsWith(@namespace))
            {
                continue;
            }

            var relativePath = resource[@namespace.Length..];
            var fileName = relativePath
                .Replace('.', Path.DirectorySeparatorChar)
                .Replace("____", ".")
                .Replace('^', '.');

            var outputPath = Path.Combine(templateDir, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            using var stream = assembly.GetManifestResourceStream(resource)!;
            using var file = File.Create(outputPath);

            stream.CopyTo(file);
        }

        return ValueTask.CompletedTask;
    }
}
