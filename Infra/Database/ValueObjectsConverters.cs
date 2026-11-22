using Base.Database;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database;
public static class ValueObjectsConverters
{
    public static void Configure(ModelConfigurationBuilder configurationBuilder)
    {
        ValueObjectConverter.Text<Password, PasswordConverter>(configurationBuilder);
        ValueObjectConverter.Numeric<UserPoints, int, UserPointsConverter>(configurationBuilder);
        ValueObjectConverter.Text<Email, EmailConverter>(configurationBuilder);
    }
}

internal sealed class PasswordConverter() : TextConverter<Password>(v => new Password(v));
internal sealed class UserPointsConverter() : NumericConverter<UserPoints, int>(v => new UserPoints(v));
internal sealed class EmailConverter() : TextConverter<Email>(v => new Email(v));