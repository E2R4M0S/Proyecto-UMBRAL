using System;

namespace Umbral.Domain.ValueObjects;

public record TeamName
{
    public string Value { get; }

    private TeamName(string value)
    {
        Value = value;
    }

    public static TeamName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Team name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("Team name must be at most 100 characters.", nameof(value));

        return new TeamName(value);
    }

    public static implicit operator string(TeamName name) => name.Value;

    public override string ToString() => Value;
}
