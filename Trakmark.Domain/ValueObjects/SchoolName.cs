namespace Trakmark.Domain.ValueObjects;

/// <summary>A non-empty, trimmed name for a school.</summary>
public sealed record SchoolName
{
    /// <summary>The trimmed name value.</summary>
    public string Value { get; }

    /// <summary>Initializes a <see cref="SchoolName"/>, trimming whitespace.</summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is empty or whitespace.</exception>
    public SchoolName(string value)
    {
        var trimmed = value?.Trim() ?? string.Empty;
        if (trimmed.Length == 0)
            throw new ArgumentException("School name must not be empty or whitespace.", nameof(value));
        Value = trimmed;
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
