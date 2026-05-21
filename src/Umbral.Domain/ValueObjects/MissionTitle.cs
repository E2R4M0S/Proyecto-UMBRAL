using System;

namespace Umbral.Domain.ValueObjects;

public record MissionTitle
{
    public string Value { get; }

    private MissionTitle(string value)
    {
        Value = value;
    }

    public static MissionTitle Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Mission title cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 150)
            throw new ArgumentException("Mission title must be at most 150 characters.", nameof(value));

        return new MissionTitle(value);
    }

    public static implicit operator string(MissionTitle title) => title.Value;

    public override string ToString() => Value;
}
