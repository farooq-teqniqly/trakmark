namespace Trakmark.Domain.ValueObjects;

/// <summary>A non-empty, trimmed name for a meet.</summary>
public sealed class MeetName : IEquatable<MeetName>
{
    /// <summary>The trimmed name value.</summary>
    public string Value { get; }

    /// <summary>Initializes a <see cref="MeetName"/>, trimming whitespace.</summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is empty or whitespace.</exception>
    public MeetName(string value)
    {
        var trimmed = value?.Trim() ?? string.Empty;
        if (trimmed.Length == 0)
        {
            throw new ArgumentException("Meet name must not be empty or whitespace.", nameof(value));
        }

        Value = trimmed;
    }

    /// <inheritdoc/>
    public bool Equals(MeetName? other) => other is not null && Value == other.Value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as MeetName);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    /// <inheritdoc/>
    public override string ToString() => Value;
}
