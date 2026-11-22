using Base;

namespace Identity.Sdk;

public sealed record Password(string value) : Text(value, maxLength: 53, minLength: 8)
{
    public override string ToString() => base.ToString();
}