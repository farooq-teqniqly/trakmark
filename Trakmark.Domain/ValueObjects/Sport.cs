namespace Trakmark.Domain.ValueObjects;

/// <summary>
/// A closed set of sports supported by the domain (Track &amp; Field and Cross-Country).
/// </summary>
public sealed class Sport : IEquatable<Sport>
{
    /// <summary>The display name of this sport.</summary>
    public string Name { get; }

    private Sport(string name) => Name = name;

    /// <summary>Track and field.</summary>
    public static readonly Sport TrackAndField = new("Track & Field");

    /// <summary>Cross-country.</summary>
    public static readonly Sport CrossCountry = new("Cross-Country");

    /// <inheritdoc/>
    public bool Equals(Sport? other) => ReferenceEquals(this, other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Sport);

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode(StringComparison.Ordinal);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
