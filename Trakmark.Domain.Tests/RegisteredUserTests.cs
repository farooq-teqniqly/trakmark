using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests;

/// <summary>
/// Tests for section 5: RegisteredUser aggregate.
/// Covers spec scenarios in manage-students.
/// </summary>
public sealed class RegisteredUserTests
{
    // ── Adding a student creates and follows it ───────────────────────────

    [Fact]
    public void AddStudent_ValidName_CreatesStudentAndFollows()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);
        var name = new PersonName("Alice Johnson");

        // Act
        var student = user.AddStudent(name);

        // Assert
        Assert.NotNull(student);
        Assert.Contains(student.Id, user.Following);
    }

    [Fact]
    public void AddStudent_ValidName_StudentHasEmptyCareer()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);
        var name = new PersonName("Bob Smith");

        // Act
        var student = user.AddStudent(name);

        // Assert
        Assert.Empty(student.Career.Enrollments);
    }

    // ── Person name must be valid ─────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddStudent_InvalidName_ThrowsAndNoStudentAdded(string invalidName)
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);
        var followingBefore = user.Following.Count;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.AddStudent(new PersonName(invalidName)));
        Assert.Equal(followingBefore, user.Following.Count);
    }

    // ── Unfollow removes the link only ────────────────────────────────────

    [Fact]
    public void Unfollow_ExistingStudentId_RemovesFromFollowingOnly()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);
        var student = user.AddStudent(new PersonName("Carol Davis"));
        var studentId = student.Id;

        // Act
        user.Unfollow(studentId);

        // Assert
        Assert.DoesNotContain(studentId, user.Following);
        Assert.Equal(studentId, student.Id);
    }

    // ── Following is idempotent ───────────────────────────────────────────

    [Fact]
    public void Follow_AlreadyFollowing_NoopAndNoDuplicate()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);
        var studentId = StudentId.NewId();
        user.Follow(studentId);
        var countAfterFirst = user.Following.Count;

        // Act
        user.Follow(studentId);

        // Assert
        Assert.Equal(countAfterFirst, user.Following.Count);
        Assert.Contains(studentId, user.Following);
    }

    // ── A student has no account link by default ──────────────────────────

    [Fact]
    public void AddStudent_ByDefault_StudentHasNoAccountLink()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);

        // Act
        var student = user.AddStudent(new PersonName("Dave Wilson"));

        // Assert
        Assert.Null(student.AccountLink);
    }
}
