namespace Trakmark.Domain.Catalog;

/// <summary>
/// The competitive division within which a result was recorded (Varsity, JV, or Open).
/// Tier is contextual metadata and does not segment personal bests or season bests — a mark
/// set in any tier is eligible as a best.
/// </summary>
public sealed class Tier : IEquatable<Tier>
{
    /// <summary>The display name of this tier.</summary>
    public string Name { get; }

    private Tier(string name) => Name = name;

    /// <summary>Varsity competitive division.</summary>
    public static readonly Tier Varsity = new("Varsity");

    /// <summary>Junior Varsity competitive division.</summary>
    public static readonly Tier JV = new("JV");

    /// <summary>Open (no divisional restriction) competitive division. Used as the default.</summary>
    public static readonly Tier Open = new("Open");

    /// <inheritdoc/>
    public bool Equals(Tier? other) =>
        other is not null && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Tier);

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns <see langword="true"/> when both tiers are equal.</summary>
    public static bool operator ==(Tier? left, Tier? right) => left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the tiers differ.</summary>
    public static bool operator !=(Tier? left, Tier? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
