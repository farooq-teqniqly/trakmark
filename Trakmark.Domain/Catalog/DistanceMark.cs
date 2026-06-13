namespace Trakmark.Domain.Catalog;

/// <summary>
/// A distance performance mark stored as an integer number of centimetres.
/// Higher is better (max wins).
/// </summary>
public sealed class DistanceMark : Performance, IEquatable<DistanceMark>
{
    /// <summary>The distance in centimetres.</summary>
    public int Centimetres { get; }

    /// <summary>Initializes a new <see cref="DistanceMark"/>.</summary>
    /// <param name="centimetres">Must be a positive value.</param>
    public DistanceMark(int centimetres)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(centimetres);
        Centimetres = centimetres;
    }

    /// <inheritdoc/>
    public override bool IsBetterThan(Performance other)
    {
        if (other is not DistanceMark otherDist) { return false; }

        return Centimetres > otherDist.Centimetres;
    }

    /// <inheritdoc/>
    public bool Equals(DistanceMark? other) => other is not null && Centimetres == other.Centimetres;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DistanceMark);

    /// <inheritdoc/>
    public override int GetHashCode() => Centimetres.GetHashCode();

    /// <summary>Returns <see langword="true"/> when both marks are equal.</summary>
    public static bool operator ==(DistanceMark? left, DistanceMark? right) => left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the marks differ.</summary>
    public static bool operator !=(DistanceMark? left, DistanceMark? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => $"{Centimetres}cm";
}