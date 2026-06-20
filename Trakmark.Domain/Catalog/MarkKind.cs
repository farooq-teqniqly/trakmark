namespace Trakmark.Domain.Catalog;

/// <summary>
/// The kind of measured mark a discipline produces, which implies the comparison direction
/// used when computing season bests and personal bests.
/// </summary>
public sealed class MarkKind : IEquatable<MarkKind>
{
    /// <summary>The display name of this mark kind.</summary>
    public string Name { get; }

    /// <summary>The comparison direction implied by this mark kind.</summary>
    public ComparisonDirection Direction { get; }

    private MarkKind(string name, ComparisonDirection direction)
    {
        Name = name;
        Direction = direction;
    }

    /// <summary>A timed result; lower time wins.</summary>
    public static readonly MarkKind Time = new("Time", ComparisonDirection.LowerIsBetter);

    /// <summary>A distance result; greater distance wins.</summary>
    public static readonly MarkKind Distance = new("Distance", ComparisonDirection.HigherIsBetter);

    /// <summary>A finish-rank result only; no measurable mark and no best.</summary>
    public static readonly MarkKind PlaceOnly = new("PlaceOnly", ComparisonDirection.None);

    /// <inheritdoc/>
    public bool Equals(MarkKind? other) =>
        other is not null && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as MarkKind);

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns <see langword="true"/> when both mark kinds are equal.</summary>
    public static bool operator ==(MarkKind? left, MarkKind? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the mark kinds differ.</summary>
    public static bool operator !=(MarkKind? left, MarkKind? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
