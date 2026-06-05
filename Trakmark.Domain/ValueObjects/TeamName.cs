namespace Trakmark.Domain.ValueObjects;

/// <summary>A non-empty, trimmed name for a team.</summary>
public sealed record TeamName
{
    /// <summary>The trimmed name value.</summary>
    public string Value { get; }

    /// <summary>Initializes a <see cref="TeamName"/>, trimming whitespace.</summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is empty or whitespace.</exception>
    public TeamName(string value)
    {
        var trimmed = value?.Trim() ?? string.Empty;
        if (trimmed.Length == 0)
        {
            throw new ArgumentException("Team name must not be empty or whitespace.", nameof(value));
        }

        Value = trimmed;
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
