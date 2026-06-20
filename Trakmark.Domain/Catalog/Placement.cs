namespace Trakmark.Domain.Catalog;

/// <summary>
/// A positive-integer finish rank within a competitive heat or section.
/// Placement is separate from <see cref="Performance"/> — every finished result
/// carries a place; only timed or distance disciplines also carry a measured mark.
/// </summary>
public sealed class Placement : IEquatable<Placement>
{
    /// <summary>The finish rank (1 = first place).</summary>
    public int Rank { get; }

    /// <summary>Initializes a new <see cref="Placement"/>.</summary>
    /// <param name="rank">Must be a positive integer.</param>
    public Placement(int rank)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rank);
        Rank = rank;
    }

    /// <inheritdoc/>
    public bool Equals(Placement? other) => other is not null && Rank == other.Rank;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Placement);

    /// <inheritdoc/>
    public override int GetHashCode() => Rank.GetHashCode();

    /// <summary>Returns <see langword="true"/> when both placements are equal.</summary>
    public static bool operator ==(Placement? left, Placement? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the placements differ.</summary>
    public static bool operator !=(Placement? left, Placement? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => Rank.ToString();
}
