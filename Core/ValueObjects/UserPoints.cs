using Base;

namespace Core.ValueObjects;

public sealed record UserPoints(int value) : Numeric<int>(
    value,
    min: 0,
    max: 100)
{
    public static implicit operator UserPoints(int value) => new(value);
}