using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Aggregates;

/// <summary>
/// Tests for section 5: RegisteredUser aggregate.
/// Covers spec scenarios in manage-students.
/// </summary>
public sealed class RegisteredUserTests
{
    [Fact]
    public void AddStudent_ValidName_CreatesStudentAndFollowsWithEmptyCareer()
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
        Assert.Empty(student.Career.Enrollments);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddStudent_PersonNameValidation_RejectsEmptyName(string invalidName)
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);
        var followingBefore = user.Following.Count;
        // PersonName constructor throws; AddStudent does not add a guard of its own

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.AddStudent(new PersonName(invalidName)));
        Assert.Equal(followingBefore, user.Following.Count);
    }

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
        // Only the follow link is removed; the Student object itself is not part of
        // this aggregate and cannot be verified here without a repository.
        Assert.Empty(user.Following);
    }

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

    [Fact]
    public void AddStudent_ByDefault_StudentHasNoAccountLink()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);

        // Act
        var student = user.AddStudent(new PersonName("Dave Wilson"));

        // Assert
        Assert.Null(student.UserAccountId);
    }
}
