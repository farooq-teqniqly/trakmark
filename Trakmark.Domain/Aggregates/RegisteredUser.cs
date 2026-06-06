using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// Aggregate root representing the authenticated account holder.
/// Maintains a following set of <see cref="StudentId"/>s and can create students.
/// </summary>
public sealed class RegisteredUser
{
    private readonly HashSet<StudentId> _following = [];

    /// <summary>The unique identifier for this user.</summary>
    public RegisteredUserId Id { get; }

    /// <summary>The external ASP.NET Identity account key bridging domain to auth.</summary>
    public UserAccountId AccountId { get; }

    /// <summary>The read-only set of <see cref="StudentId"/>s this user is following.</summary>
    public IReadOnlySet<StudentId> Following => _following;

    private RegisteredUser(RegisteredUserId id, UserAccountId accountId)
    {
        Id = id;
        AccountId = accountId;
    }

    /// <summary>Creates a new <see cref="RegisteredUser"/> for the given account.</summary>
    public static RegisteredUser Create(UserAccountId accountId) =>
        new(RegisteredUserId.NewId(), accountId);

    /// <summary>
    /// Adds <paramref name="studentId"/> to the following set.
    /// If already present, the set is unchanged (idempotent).
    /// </summary>
    public void Follow(StudentId studentId) => _following.Add(studentId);

    /// <summary>
    /// Removes <paramref name="studentId"/> from the following set.
    /// If not present, the set is unchanged.
    /// </summary>
    public void Unfollow(StudentId studentId) => _following.Remove(studentId);

    /// <summary>
    /// Creates a new <see cref="Student"/> with the given <paramref name="name"/>
    /// and immediately follows it.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="name"/> is invalid (propagated from <see cref="PersonName"/>).
    /// </exception>
    public Student AddStudent(PersonName name)
    {
        ArgumentNullException.ThrowIfNull(name);
        var student = new Student(StudentId.NewId(), name);
        Follow(student.Id);
        return student;
    }
}
