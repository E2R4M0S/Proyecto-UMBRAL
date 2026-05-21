namespace Umbral.Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));

        if (!value.Contains('@'))
            throw new ArgumentException("Email must contain the '@' character.", nameof(value));

        var parts = value.Split('@');
        if (parts.Length != 2)
            throw new ArgumentException("Email must contain exactly one '@' character.", nameof(value));

        var localPart = parts[0];
        var domain = parts[1];

        if (string.IsNullOrWhiteSpace(localPart))
            throw new ArgumentException("Email must have a local part before '@'.", nameof(value));

        if (string.IsNullOrWhiteSpace(domain) || !domain.Contains('.'))
            throw new ArgumentException("Email must have a valid domain.", nameof(value));

        return new Email(value.Trim().ToLowerInvariant());
    }

    public override string ToString() => Value;
}
