namespace Trakmark.Domain.Catalog;

/// <summary>
/// A closed set of hurdle heights used as a setup parameter for hurdle-run disciplines.
/// Hurdle height is part of discipline identity: 110m @ 39" and 110m @ 33" are distinct disciplines.
/// </summary>
public sealed class HurdleHeight : IEquatable<HurdleHeight>
{
    /// <summary>The display name of this hurdle height.</summary>
    public string Name { get; }

    private HurdleHeight(string name) => Name = name;

    /// <summary>39-inch hurdle height (standard men's HS/college).</summary>
    public static readonly HurdleHeight Inches39 = new("39\"");

    /// <summary>36-inch hurdle height (standard women's HS/college).</summary>
    public static readonly HurdleHeight Inches36 = new("36\"");

    /// <summary>33-inch hurdle height (youth/middle school).</summary>
    public static readonly HurdleHeight Inches33 = new("33\"");

    /// <summary>30-inch hurdle height (youth/elementary).</summary>
    public static readonly HurdleHeight Inches30 = new("30\"");

    /// <inheritdoc/>
    public bool Equals(HurdleHeight? other) =>
        other is not null && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as HurdleHeight);

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns <see langword="true"/> when both hurdle heights are equal.</summary>
    public static bool operator ==(HurdleHeight? left, HurdleHeight? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the hurdle heights differ.</summary>
    public static bool operator !=(HurdleHeight? left, HurdleHeight? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
