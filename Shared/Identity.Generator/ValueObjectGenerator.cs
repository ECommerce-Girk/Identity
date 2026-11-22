
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class ValueObjectGenerator
{
    public static void Generate()
    {
        var valueObjectsPath = Path.Combine(
            Program.SolutionRoot,
            "Core",
            "ValueObjects"
        );

        var records = new List<RecordDeclarationSyntax>();

        foreach (var file in Directory.GetFiles(
            valueObjectsPath,
            "*.cs",
            SearchOption.AllDirectories))
        {
            var tree = CSharpSyntaxTree.ParseText(
                File.ReadAllText(file),
                CSharpParseOptions.Default
            );

            var root = tree.GetRoot();

            records.AddRange(
                root.DescendantNodes()
                    .OfType<RecordDeclarationSyntax>()
            );
        }

        foreach (var record in records)
        {
            GenerateValueObject(record);
        }

        SyncJsonConverters(records);
        SyncDatabaseConverters(records);
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

    private static void SyncJsonConverters(
        IEnumerable<RecordDeclarationSyntax> records)
    {
        var converterPath = Path.Combine(
            Program.SolutionRoot,
            "Core",
            "ValueObjects",
            "ValueObjectsJsonConverters.cs"
        );

        var source = File.ReadAllText(converterPath);

        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetRoot();

        var method = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(m => m.Identifier.ValueText ==
                "AddValueObjectsJsonConverters");

        if (method.Body is null)
            throw new InvalidOperationException(
                "AddValueObjectsJsonConverters must have a method body."
            );

        var existingConverters = method.Body
            .DescendantNodes()
            .OfType<ObjectCreationExpressionSyntax>()
            .Select(creation => GetConverterType(creation.Type))
            .Where(name => name is not null)
            .ToHashSet();

        var newStatements = new List<StatementSyntax>();

        foreach (var record in records)
        {
            var name = record.Identifier.ValueText;

            if (existingConverters.Contains(name))
                continue;

            var numericType = GetNumericType(record);

            var converter = numericType is not null
                ? $"NumericJsonConverter<{name}, {numericType}>"
                : $"TextJsonConverter<{name}>";

            var statement = SyntaxFactory.ParseStatement(
                $"options.Converters.Add(new {converter}(v => new {name}(v)));"
            );

            newStatements.Add(statement);
            existingConverters.Add(name);
        }

        if (newStatements.Count == 0)
        {
            Console.WriteLine(" -> JSON converters are already synchronized.");
            return;
        }

        var updatedMethod = method.WithBody(
            method.Body.AddStatements(newStatements.ToArray())
        );

        var updatedRoot = root.ReplaceNode(method, updatedMethod)
            .NormalizeWhitespace();

        File.WriteAllText(
            converterPath,
            updatedRoot.ToFullString(),
            Encoding.UTF8
        );

        Console.WriteLine(
            $" -> Added {newStatements.Count} JSON converter(s)."
        );
    }

    private static string? GetNumericType(RecordDeclarationSyntax record)
    {
        return record.BaseList?.Types
            .Select(t => t.Type)
            .OfType<GenericNameSyntax>()
            .FirstOrDefault(t => t.Identifier.ValueText == "Numeric")
            ?.TypeArgumentList.Arguments.FirstOrDefault()
            ?.ToString();
    }

    private static string? GetConverterType(TypeSyntax type)
    {
        return type.DescendantNodesAndSelf()
            .OfType<GenericNameSyntax>()
            .FirstOrDefault(g =>
                g.Identifier.ValueText is "TextJsonConverter"
                    or "NumericJsonConverter")
            ?.TypeArgumentList.Arguments.FirstOrDefault()
            ?.ToString();
    }

    private static void SyncDatabaseConverters(
        IEnumerable<RecordDeclarationSyntax> records)
    {
        var converterPath = Path.Combine(
            Program.SolutionRoot,
            "Infra",
            "Database",
            "ValueObjectsConverters.cs"
        );

        var source = File.ReadAllText(converterPath);

        var root = CSharpSyntaxTree.ParseText(source).GetRoot();

        var namespaceNode = root.DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .Single(n => n.Name.ToString() == "Infra.Database");

        var method = namespaceNode.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(m => m.Identifier.ValueText == "Configure");

        if (method.Body is null)
            throw new InvalidOperationException(
                "Configure must have a method body."
            );

        var existingClasses = namespaceNode.Members
            .OfType<ClassDeclarationSyntax>()
            .Select(c => c.Identifier.ValueText)
            .ToHashSet();

        var existingRegistrations = method.Body
            .DescendantNodes()
            .OfType<GenericNameSyntax>()
            .Where(g => g.Identifier.ValueText is "Text" or "Numeric")
            .Select(g => g.TypeArgumentList.Arguments.FirstOrDefault()?.ToString())
            .Where(name => name is not null)
            .ToHashSet();

        var newClasses = new List<MemberDeclarationSyntax>();
        var newRegistrations = new List<StatementSyntax>();

        foreach (var record in records)
        {
            var name = record.Identifier.ValueText;
            var converterName = $"{name}Converter";

            var numericType = GetNumericType(record);

            // Generate converter class if missing.
            if (existingClasses.Add(converterName))
            {
                var classCode = numericType is not null
                    ? $"""
                    internal sealed class {converterName}() : NumericConverter<{name}, {numericType}>(v => new {name}(v));

                    """
                    : $"""
                    internal sealed class {converterName}() : TextConverter<{name}>(v => new {name}(v));
                    
                    """;

                var declaration = SyntaxFactory.ParseMemberDeclaration(classCode);

                if (declaration is not null)
                    newClasses.Add(declaration);
            }

            // Generate EF configuration if missing.
            if (!existingRegistrations.Add(name))
                continue;

            var registration = numericType is not null
                ? $"ValueObjectConverter.Numeric<{name}, {numericType}, {converterName}>(configurationBuilder);"
                : $"ValueObjectConverter.Text<{name}, {converterName}>(configurationBuilder);";

            newRegistrations.Add(
                SyntaxFactory.ParseStatement(registration)
            );
        }

        if (newClasses.Count == 0 && newRegistrations.Count == 0)
        {
            Console.WriteLine(" -> Database converters are already synchronized.");
            return;
        }

        var updatedMethod = method.WithBody(
            method.Body.AddStatements(newRegistrations.ToArray())
        );

        var updatedNamespace = namespaceNode.ReplaceNode(method, updatedMethod);

        updatedNamespace = updatedNamespace.AddMembers(
            newClasses.ToArray()
        );

        var updatedRoot = root.ReplaceNode(namespaceNode, updatedNamespace)
            .NormalizeWhitespace();

        File.WriteAllText(
            converterPath,
            updatedRoot.ToFullString(),
            Encoding.UTF8
        );

        Console.WriteLine(
            $" -> Added {newClasses.Count} database converter class(es) " +
            $"and {newRegistrations.Count} registration(s)."
        );
    }

}
