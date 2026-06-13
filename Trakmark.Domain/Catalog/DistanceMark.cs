namespace Trakmark.Domain.Catalog;

/// <summary>
/// A distance performance mark stored as an integer number of centimeters.
/// Higher is better (max wins).
/// </summary>
public sealed class DistanceMark : Performance, IEquatable<DistanceMark>
{
    /// <summary>The distance in centimeters.</summary>
    public int Centimeters { get; }

    /// <summary>Initializes a new <see cref="DistanceMark"/>.</summary>
    /// <param name="centimeters">Must be a positive value.</param>
    public DistanceMark(int centimeters)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(centimeters);
        Centimeters = centimeters;
    }

    /// <inheritdoc/>
    public override bool IsBetterThan(Performance other)
    {
        if (other is not DistanceMark otherDist) { return false; }

        return Centimeters > otherDist.Centimeters;
    }

    /// <inheritdoc/>
    public bool Equals(DistanceMark? other) => other is not null && Centimeters == other.Centimeters;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DistanceMark);

    /// <inheritdoc/>
    public override int GetHashCode() => Centimeters.GetHashCode();

    /// <summary>Returns <see langword="true"/> when both marks are equal.</summary>
    public static bool operator ==(DistanceMark? left, DistanceMark? right) => left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the marks differ.</summary>
    public static bool operator !=(DistanceMark? left, DistanceMark? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => $"{Centimeters}cm";
}