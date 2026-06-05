namespace Trakmark.Domain.ValueObjects;

/// <summary>A non-empty, trimmed name for a school.</summary>
public sealed class SchoolName : IEquatable<SchoolName>
{
    /// <summary>The trimmed name value.</summary>
    public string Value { get; }

    /// <summary>Initializes a <see cref="SchoolName"/>, trimming whitespace.</summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is empty or whitespace.</exception>
    public SchoolName(string value)
    {
        var trimmed = value?.Trim() ?? string.Empty;
        if (trimmed.Length == 0)
        {
            throw new ArgumentException("School name must not be empty or whitespace.", nameof(value));
        }

        Value = trimmed;
    }

    /// <inheritdoc/>
    public bool Equals(SchoolName? other) =>
        other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as SchoolName);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    /// <summary>Returns <see langword="true"/> when both operands are equal.</summary>
    public static bool operator ==(SchoolName? left, SchoolName? right) =>
        left is null ? right is null : left.Equals(right);

    /// <summary>Returns <see langword="true"/> when both operands are not equal.</summary>
    public static bool operator !=(SchoolName? left, SchoolName? right) =>
        !(left == right);

    /// <inheritdoc/>
    public override string ToString() => Value;
}
