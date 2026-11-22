

using System.Text;

public static class FileCreator
{
    public static void Create(string fileName, string content, string projectName, string? directory = null)
    {
        var targetDirectory = Path.Combine(Program.SolutionRoot, $"Shared/{Program.SolutionName}.{projectName}");
        if(directory != null)
        {
            targetDirectory = Path.Combine(targetDirectory, directory);
            Directory.CreateDirectory(targetDirectory);
        }

        var targetFilePath = Path.Combine(targetDirectory, $"{fileName}.cs");
        File.WriteAllText(targetFilePath, content, Encoding.UTF8);

        Console.WriteLine($" -> Generated file: {Path.GetRelativePath(Program.SolutionRoot, targetFilePath)}");
    }
}