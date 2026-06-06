using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// The Student aggregate root. Owns a <see cref="Aggregates.Career"/> and
/// optionally references an ASP.NET Identity account via <see cref="UserAccountId"/>.
/// </summary>
public sealed class Student
{
    /// <summary>The unique identifier for this student.</summary>
    public StudentId Id { get; }

    /// <summary>The student's name.</summary>
    public PersonName Name { get; }

    /// <summary>
    /// The optional link to an authenticated account.
    /// <see langword="null"/> in the pilot (dormant).
    /// </summary>
    public UserAccountId? UserAccountId { get; }

    /// <summary>The student's career (set of yearly enrollments).</summary>
    public Career Career { get; } = new Career();

    /// <summary>Initializes a <see cref="Student"/>, optionally linking an account.</summary>
    public Student(StudentId id, PersonName name, UserAccountId? accountId = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        Id = id;
        Name = name;
        UserAccountId = accountId;
    }

    /// <summary>
    /// Adds an enrollment for the specified school, year, and grade.
    /// Fails when the student already has an enrollment for <paramref name="schoolYear"/>.
    /// </summary>
    public OperationResult AddEnrollment(SchoolId schoolId, SchoolYear schoolYear, GradeLevel gradeLevel)
    {
        ArgumentNullException.ThrowIfNull(gradeLevel);
        var enrollment = new Enrollment(schoolId, schoolYear, gradeLevel);

        if (!Career.TryAdd(enrollment))
        {
            return OperationResult.Failure($"An enrollment for school year {schoolYear} already exists.");
        }

        return OperationResult.Success();
    }
}
