namespace Trakmark.Domain.Catalog;

/// <summary>
/// A measured performance mark — the closed base class for <see cref="TimeMark"/>
/// and <see cref="DistanceMark"/>. Placement (finish rank) is separate and is not
/// modeled as a performance variant.
/// </summary>
public abstract class Performance
{
    /// <summary>Determines whether this mark is better than <paramref name="other"/>
    /// according to the discipline's comparison direction.</summary>
    /// <param name="other">The mark to compare against.</param>
    /// <returns><see langword="true"/> if this mark is strictly better.</returns>
    public abstract bool IsBetterThan(Performance other);
}