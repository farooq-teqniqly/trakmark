namespace Trakmark.Domain.ValueObjects;

/// <summary>A non-empty, trimmed name for a school.</summary>
public sealed class SchoolName : IEquatable<SchoolName>
{
    /// <summary>The trimmed name value.</summary>
    public string Value { get; }

    /// <summary>Initializes a <see cref="SchoolName"/>, trimming whitespace.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is empty or whitespace.</exception>
    public SchoolName(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var trimmed = value.Trim();
        if (trimmed.Length == 0)
        {
            throw new ArgumentException("School name must not be empty or whitespace.", nameof(value));
        }

        Value = trimmed;
    }

    /// <inheritdoc/>
    public bool Equals(SchoolName? other) => other is not null && Value == other.Value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as SchoolName);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    /// <inheritdoc/>
    public override string ToString() => Value;

    /// <summary>Returns true when both instances are equal.</summary>
    public static bool operator ==(SchoolName? left, SchoolName? right) => left?.Equals(right) ?? right is null;

    /// <summary>Returns true when both instances are not equal.</summary>
    public static bool operator !=(SchoolName? left, SchoolName? right) => !(left == right);
}
