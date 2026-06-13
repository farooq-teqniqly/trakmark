namespace Trakmark.Domain.Catalog;

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

    /// <summary>Returns <see langword="true"/> when both marks are equal.</summary>
    public static bool operator ==(TimeMark? left, TimeMark? right) => left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the marks differ.</summary>
    public static bool operator !=(TimeMark? left, TimeMark? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => $"{Milliseconds}ms";
}