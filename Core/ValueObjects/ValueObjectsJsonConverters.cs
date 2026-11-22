using System.Text.Json;
using Base;
using Core.ValueObjects;

namespace Core;

public static class ValueObjectsJsonConverters
{
    public static void AddValueObjectsJsonConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new TextJsonConverter<Email>(v => new Email(v)));
        options.Converters.Add(new TextJsonConverter<Password>(v => new Password(v)));
    }
}
