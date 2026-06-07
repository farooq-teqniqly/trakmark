namespace Trakmark.Domain.Catalog;

/// <summary>
/// The direction used when comparing performance marks to determine which is better.
/// </summary>
public enum ComparisonDirection
{
    /// <summary>No measurable direction; applies to place-only disciplines.</summary>
    None,

    /// <summary>A lower value is better (e.g., time disciplines).</summary>
    LowerIsBetter,

    /// <summary>A higher value is better (e.g., distance disciplines).</summary>
    HigherIsBetter,
}
