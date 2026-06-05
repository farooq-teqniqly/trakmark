namespace Trakmark.Domain.Catalog;

/// <summary>
/// A positive-integer finish rank within a competitive heat or section.
/// Placement is separate from <see cref="Performance"/> — every finished result
/// carries a place; only timed or distance disciplines also carry a measured mark.
/// </summary>
public sealed record Placement
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
    public override string ToString() => Rank.ToString();
}
