using System.Text.RegularExpressions;

namespace Umbral.Domain.ValueObjects;

public record Alias
{
    public string Value { get; }

    private Alias(string value)
    {
        Value = value;
    }

    public static Alias Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Alias cannot be empty.", nameof(value));

        if (value.Length < 3)
            throw new ArgumentException("Alias must be at least 3 characters.", nameof(value));

        if (value.Length > 20)
            throw new ArgumentException("Alias must be at most 20 characters.", nameof(value));

        if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
            throw new ArgumentException("Alias must contain only letters, numbers, and underscores.", nameof(value));

        return new Alias(value.ToLowerInvariant());
    }

    public override string ToString() => Value;

    public static implicit operator string(Alias alias) => alias.Value;
}
