using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// Aggregate root representing a student athlete.
/// Owns the student's career (a set of yearly enrollments).
/// </summary>
public sealed class Student
{
    /// <summary>The unique identifier for this student.</summary>
    public StudentId Id { get; }

    /// <summary>The student's full name.</summary>
    public PersonName Name { get; }

    /// <summary>
    /// The optional link to an authenticated account.
    /// <see langword="null"/> until the student's account is linked.
    /// </summary>
    public UserAccountId? AccountLink { get; private set; }

    /// <summary>The student's career record.</summary>
    public Career Career { get; }

    /// <summary>Initializes a new <see cref="Student"/> with a new <see cref="StudentId"/> and an empty career.</summary>
    public Student(PersonName name)
    {
        Id = StudentId.NewId();
        Name = name;
        Career = new Career();
        AccountLink = null;
    }
}
