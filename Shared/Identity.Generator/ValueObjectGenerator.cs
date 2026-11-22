
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class ValueObjectGenerator
{
    public static void Generate(string name)
    {
        var valueObject = GetValueObject(name).NormalizeWhitespace();

        var finalCode = $$""""
        #nullable enable
        using Base.ValueObjects;

        namespace {{Program.SolutionName}}.Sdk;

        {{valueObject.ToFullString()}}
        """";

        FileCreator.Create(valueObject.Identifier.ValueText, finalCode, "Sdk");
    }

    public static RecordDeclarationSyntax GetValueObject(string name)
    {
        var valueObjectsPath = Path.Combine(Program.SolutionRoot, "Identity", "Core", "ValueObjects");

        foreach (var file in Directory.GetFiles(valueObjectsPath, "*.cs"))
        {
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file), CSharpParseOptions.Default);
            var root = tree.GetRoot();

            var record = root.DescendantNodes()
                .OfType<RecordDeclarationSyntax>()
                .SingleOrDefault(r => r.Identifier.ValueText == name);

            if (record is not null)
                return record;
        }

        throw new InvalidOperationException($"Value object '{name}' was not found.");
    }
}