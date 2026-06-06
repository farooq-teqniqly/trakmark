using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// A single year's enrollment pairing a school, academic year, and grade level.
/// Two enrollments are equal when their <see cref="SchoolId"/>, <see cref="SchoolYear"/>,
/// and <see cref="GradeLevel"/> are all equal.
/// </summary>
public sealed class Enrollment : IEquatable<Enrollment>
{
    /// <summary>The school the student was enrolled in for this year.</summary>
    public SchoolId SchoolId { get; }

    /// <summary>The academic year of the enrollment.</summary>
    public SchoolYear SchoolYear { get; }

    /// <summary>The grade level the student was in during this enrollment.</summary>
    public GradeLevel GradeLevel { get; }

    /// <summary>Initializes an <see cref="Enrollment"/>.</summary>
    public Enrollment(SchoolId schoolId, SchoolYear schoolYear, GradeLevel gradeLevel)
    {
        ArgumentNullException.ThrowIfNull(gradeLevel);
        SchoolId = schoolId;
        SchoolYear = schoolYear;
        GradeLevel = gradeLevel;
    }

    /// <inheritdoc/>
    public bool Equals(Enrollment? other)
    {
        if (other is null)
        {
            return false;
        }

        return SchoolId == other.SchoolId
            && SchoolYear == other.SchoolYear
            && GradeLevel.Equals(other.GradeLevel);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Enrollment);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(SchoolId, SchoolYear, GradeLevel);

    /// <summary>Returns <see langword="true"/> when both enrollments are equal.</summary>
    public static bool operator ==(Enrollment? left, Enrollment? right) => left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the enrollments differ.</summary>
    public static bool operator !=(Enrollment? left, Enrollment? right) => !(left == right);
}
