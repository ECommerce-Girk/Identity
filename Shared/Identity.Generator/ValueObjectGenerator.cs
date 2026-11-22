using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class ValueObjectGenerator
{
    public static void Generate()
    {
        var valueObjectsPath = Path.Combine(Program.SolutionRoot, "Core", "ValueObjects");

        foreach (var file in Directory.GetFiles(valueObjectsPath, "*.cs", SearchOption.AllDirectories))
        {
            var tree = CSharpSyntaxTree.ParseText(
                File.ReadAllText(file),
                CSharpParseOptions.Default
            );

            var root = tree.GetRoot();

            var records = root.DescendantNodes().OfType<RecordDeclarationSyntax>();

            foreach (var record in records)
            {
                GenerateValueObject(record);
            }
        }
    }

    private static void GenerateValueObject(RecordDeclarationSyntax record)
    {
        var valueObject = record.NormalizeWhitespace();

        var finalCode = $$"""
        using Base;

        namespace {{Program.SolutionName}}.Sdk;

        {{valueObject.ToFullString()}}
        """;

        FileCreator.Create(
            record.Identifier.ValueText,
            finalCode,
            "ValueObjects"
        );
    }
}