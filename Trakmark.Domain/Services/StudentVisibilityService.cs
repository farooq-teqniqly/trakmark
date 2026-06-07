using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;

namespace Trakmark.Domain.Services;

/// <summary>
/// Domain service that resolves which students a registered user may view.
/// </summary>
/// <remarks>
/// A user sees students = their <see cref="RegisteredUser.Following"/> set
/// ∪ the student whose <see cref="Student.UserAccountId"/> equals the user's account.
/// This is a cross-aggregate rule because <see cref="RegisteredUser"/> and
/// <see cref="Student"/> are separate aggregate roots.
/// </remarks>
public sealed class StudentVisibilityService
{
    /// <summary>
    /// Returns the subset of <paramref name="allStudents"/> that <paramref name="user"/>
    /// is permitted to view.
    /// </summary>
    /// <param name="user">The registered user requesting the view.</param>
    /// <param name="allStudents">The full collection of students to filter.</param>
    /// <returns>
    /// Students that are in the user's <see cref="RegisteredUser.Following"/> set
    /// or whose <see cref="Student.UserAccountId"/> equals the user's account.
    /// Each student appears at most once in the result.
    /// </returns>
    public IEnumerable<Student> GetVisibleStudents(
        RegisteredUser user,
        IEnumerable<Student> allStudents)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(allStudents);

        var seen = new HashSet<StudentId>();
        foreach (var student in allStudents)
        {
            var isFollowed = user.Following.Contains(student.Id);
            var isLinked = student.UserAccountId == user.AccountId;

            if ((isFollowed || isLinked) && seen.Add(student.Id))
            {
                yield return student;
            }
        }
    }
}
