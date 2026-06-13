using Trakmark.Domain.Ids;

namespace Trakmark.Domain.Tests.Ids;

/// <summary>Tests for <see cref="UserAccountId"/> — external key pass-through.</summary>
public sealed class UserAccountIdTests
{
    [Fact]
    public void UserAccountId_KeepsExternalKeyAsIs()
    {
        // Arrange
        var externalKey = "some-guid-from-identity-abc123";

        // Act
        var id = new UserAccountId(externalKey);

        // Assert
        Assert.Equal(externalKey, id.Value);
    }

    [Theory]
    [InlineData("arbitrary-string")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("not-domain-format")]
    public void UserAccountId_AcceptsArbitraryStrings(string key)
    {
        // Arrange / Act
        var id = new UserAccountId(key);

        // Assert
        Assert.Equal(key, id.Value);
    }

    [Fact]
    public void UserAccountId_ToString_ReturnsValue()
    {
        // Arrange
        var key = "some-external-key-abc123";
        var id = new UserAccountId(key);

        // Act
        var result = id.ToString();

        // Assert
        Assert.Equal(key, result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void AllDomainIds_RoundTrip()
    {
        // Arrange
        var meetId = MeetId.NewId();
        var schoolId = SchoolId.NewId();
        var teamId = TeamId.NewId();
        var userId = RegisteredUserId.NewId();

        // Act / Assert
        Assert.Equal(meetId, MeetId.Parse(meetId.ToString()));
        Assert.Equal(schoolId, SchoolId.Parse(schoolId.ToString()));
        Assert.Equal(teamId, TeamId.Parse(teamId.ToString()));
        Assert.Equal(userId, RegisteredUserId.Parse(userId.ToString()));
    }
}
