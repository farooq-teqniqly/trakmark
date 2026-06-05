namespace Trakmark.Domain.ValueObjects;

/// <summary>A non-empty, trimmed name for a team.</summary>
public sealed class TeamName : IEquatable<TeamName>
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
    public bool Equals(TeamName? other) =>
        other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as TeamName);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    /// <summary>Returns <see langword="true"/> when both operands are equal.</summary>
    public static bool operator ==(TeamName? left, TeamName? right) =>
        left is null ? right is null : left.Equals(right);

    /// <summary>Returns <see langword="true"/> when both operands are not equal.</summary>
    public static bool operator !=(TeamName? left, TeamName? right) =>
        !(left == right);

    /// <inheritdoc/>
    public override string ToString() => Value;
}
