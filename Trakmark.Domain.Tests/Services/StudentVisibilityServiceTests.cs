using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.Services;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Services;

/// <summary>Tests for <see cref="StudentVisibilityService"/>.</summary>
public sealed class StudentVisibilityServiceTests
{
    [Fact]
    public void Visibility_FollowedStudent_IsVisible()
    {
        // Arrange
        var user = RegisteredUser.Create(new UserAccountId("account-1"));
        var student = new Student(StudentId.NewId(), new PersonName("Alice Johnson"));
        user.Follow(student.Id);

        var allStudents = new[] { student };
        var service = new StudentVisibilityService();

        // Act
        var visible = service.GetVisibleStudents(user, allStudents).ToList();

        // Assert
        Assert.Contains(student, visible);
    }

    [Fact]
    public void Visibility_UnrelatedStudent_IsNotVisible()
    {
        // Arrange
        var user = RegisteredUser.Create(new UserAccountId("account-1"));
        var unrelated = new Student(StudentId.NewId(), new PersonName("Bob Smith"));

        var allStudents = new[] { unrelated };
        var service = new StudentVisibilityService();

        // Act
        var visible = service.GetVisibleStudents(user, allStudents).ToList();

        // Assert
        Assert.DoesNotContain(unrelated, visible);
    }

    [Fact]
    public void Visibility_StudentWithMatchingUserAccountId_IsVisible()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);
        var linkedStudent = new Student(StudentId.NewId(), new PersonName("Carol Davis"), accountId);

        var allStudents = new[] { linkedStudent };
        var service = new StudentVisibilityService();

        // Act
        var visible = service.GetVisibleStudents(user, allStudents).ToList();

        // Assert
        Assert.Contains(linkedStudent, visible);
    }

    [Fact]
    public void Visibility_FollowedAndLinked_ReturnedOnce()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);
        var linkedStudent = new Student(StudentId.NewId(), new PersonName("Dave Wilson"), accountId);
        user.Follow(linkedStudent.Id);

        var allStudents = new[] { linkedStudent };
        var service = new StudentVisibilityService();

        // Act
        var visible = service.GetVisibleStudents(user, allStudents).ToList();

        // Assert
        Assert.Single(visible);
        Assert.Contains(linkedStudent, visible);
    }

    [Fact]
    public void Visibility_MultipleFollowedAndLinked_AllVisible()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);

        var s1 = new Student(StudentId.NewId(), new PersonName("Alice Johnson"));
        var s2 = new Student(StudentId.NewId(), new PersonName("Bob Smith"));
        var linked = new Student(StudentId.NewId(), new PersonName("Carol Davis"), accountId);
        var unrelated = new Student(StudentId.NewId(), new PersonName("Eve Thomas"));

        user.Follow(s1.Id);
        user.Follow(s2.Id);

        var allStudents = new[] { s1, s2, linked, unrelated };
        var service = new StudentVisibilityService();

        // Act
        var visible = service.GetVisibleStudents(user, allStudents).ToList();

        // Assert
        Assert.Equal(3, visible.Count);
        Assert.Contains(s1, visible);
        Assert.Contains(s2, visible);
        Assert.Contains(linked, visible);
        Assert.DoesNotContain(unrelated, visible);
    }
}
