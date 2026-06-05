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

/// <summary>
/// A timed performance mark stored as an integer number of milliseconds.
/// Lower is better (min wins).
/// </summary>
public sealed class TimeMark : Performance, IEquatable<TimeMark>
{
    /// <summary>The elapsed time in milliseconds.</summary>
    public int Milliseconds { get; }

    /// <summary>Initializes a new <see cref="TimeMark"/>.</summary>
    /// <param name="milliseconds">Must be a positive value.</param>
    public TimeMark(int milliseconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(milliseconds);
        Milliseconds = milliseconds;
    }

    /// <inheritdoc/>
    public override bool IsBetterThan(Performance other)
    {
        if (other is not TimeMark otherTime) { return false; }

        return Milliseconds < otherTime.Milliseconds;
    }

    /// <inheritdoc/>
    public bool Equals(TimeMark? other) => other is not null && Milliseconds == other.Milliseconds;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as TimeMark);

    /// <inheritdoc/>
    public override int GetHashCode() => Milliseconds.GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => $"{Milliseconds}ms";
}

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

    /// <inheritdoc/>
    public override string ToString() => $"{Centimetres}cm";
}
