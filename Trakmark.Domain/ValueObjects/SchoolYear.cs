namespace Trakmark.Domain.ValueObjects;

/// <summary>
/// An academic school year represented as the four-digit calendar year in which the
/// school year starts (e.g. 2024 for the 2024–25 school year). Orderable.
/// </summary>
public readonly record struct SchoolYear : IComparable<SchoolYear>
{
    /// <summary>The starting calendar year of the academic year.</summary>
    public int StartYear { get; }

    /// <summary>Initializes a <see cref="SchoolYear"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="startYear"/> is zero or negative.</exception>
    public SchoolYear(int startYear)
    {
        if (startYear <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(startYear),
                "School year must be a positive calendar year."
            );
        }

        StartYear = startYear;
    }

    /// <inheritdoc/>
    public int CompareTo(SchoolYear other) => StartYear.CompareTo(other.StartYear);

    /// <summary>Returns <see langword="true"/> when <paramref name="left"/> precedes <paramref name="right"/>.</summary>
    public static bool operator <(SchoolYear left, SchoolYear right) => left.CompareTo(right) < 0;

    /// <summary>Returns <see langword="true"/> when <paramref name="left"/> follows <paramref name="right"/>.</summary>
    public static bool operator >(SchoolYear left, SchoolYear right) => left.CompareTo(right) > 0;

    /// <summary>Returns <see langword="true"/> when <paramref name="left"/> precedes or equals <paramref name="right"/>.</summary>
    public static bool operator <=(SchoolYear left, SchoolYear right) => left.CompareTo(right) <= 0;

    /// <summary>Returns <see langword="true"/> when <paramref name="left"/> follows or equals <paramref name="right"/>.</summary>
    public static bool operator >=(SchoolYear left, SchoolYear right) => left.CompareTo(right) >= 0;

    /// <inheritdoc/>
    public override string ToString() => $"{StartYear}-{StartYear + 1}";
}
