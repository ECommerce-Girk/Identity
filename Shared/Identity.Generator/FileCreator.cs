

using System.Text;

public static class FileCreator
{
    public static void Create(string fileName, string content, string target)
    {
        var targetDirectory = Path.Combine(Program.SolutionRoot, "Shared", $"{Program.SolutionName}.Sdk", target);

        var targetFilePath = Path.Combine(targetDirectory, $"{fileName}.cs");
        File.WriteAllText(targetFilePath, content, Encoding.UTF8);

        Console.WriteLine($" -> Generated file: {Path.GetRelativePath(Program.SolutionRoot, targetFilePath)}");
    }
}