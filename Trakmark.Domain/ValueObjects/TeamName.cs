namespace Trakmark.Domain.ValueObjects;

/// <summary>A non-empty, trimmed name for a team.</summary>
public sealed class TeamName : IEquatable<TeamName>
{
    /// <summary>The trimmed name value.</summary>
    public string Value { get; }

    /// <summary>Initializes a <see cref="TeamName"/>, trimming whitespace.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is empty or whitespace.</exception>
    public TeamName(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var trimmed = value.Trim();
        if (trimmed.Length == 0)
        {
            throw new ArgumentException(
                "Team name must not be empty or whitespace.",
                nameof(value)
            );
        }

        Value = trimmed;
    }

    /// <inheritdoc/>
    public bool Equals(TeamName? other) =>
        other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as TeamName);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override string ToString() => Value;

    /// <summary>Returns true when both instances are equal.</summary>
    public static bool operator ==(TeamName? left, TeamName? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>Returns true when both instances are not equal.</summary>
    public static bool operator !=(TeamName? left, TeamName? right) => !(left == right);
}
