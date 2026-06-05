using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// An immutable value object pairing a school, an academic year, and a grade level.
/// Two enrollments are equal when all three components are equal.
/// </summary>
public sealed class Enrollment : IEquatable<Enrollment>
{
    /// <summary>The school the student was enrolled in.</summary>
    public SchoolId SchoolId { get; }

    /// <summary>The academic year of the enrollment.</summary>
    public SchoolYear Year { get; }

    /// <summary>The grade level at which the student competed during this enrollment.</summary>
    public GradeLevel Grade { get; }

    /// <summary>Initializes an <see cref="Enrollment"/>.</summary>
    public Enrollment(SchoolId schoolId, SchoolYear year, GradeLevel grade)
    {
        SchoolId = schoolId;
        Year = year;
        Grade = grade;
    }

    /// <inheritdoc/>
    public bool Equals(Enrollment? other)
    {
        if (other is null)
        {
            return false;
        }

        return SchoolId == other.SchoolId
            && Year == other.Year
            && Grade.Equals(other.Grade);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Enrollment);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(SchoolId, Year, Grade);
}
