using System.Text.RegularExpressions;
using Trakmark.Domain.Ids;

namespace Trakmark.Domain.Tests.Ids;

/// <summary>Tests for the shared domain-identity-format spec: prefix + six Crockford base32 chars.</summary>
public sealed class DomainIdFormatTests
{
    private static readonly Regex CrockfordBody = new(@"^[A-Z2-9]{6}$", RegexOptions.Compiled);

    [Fact]
    public void NewStudentId_MatchesStandardFormat()
    {
        // Arrange / Act
        var id = StudentId.NewId();
        var text = id.ToString();

        // Assert
        Assert.StartsWith("STU-", text);
        Assert.Matches(CrockfordBody, text[4..]);
    }

    [Theory]
    [InlineData("MEET-")]
    [InlineData("SCH-")]
    [InlineData("TEAM-")]
    [InlineData("USR-")]
    public void NewId_PrefixMatchesAggregateType(string expectedPrefix)
    {
        // Arrange / Act
        var text = expectedPrefix switch
        {
            "MEET-" => MeetId.NewId().ToString(),
            "SCH-" => SchoolId.NewId().ToString(),
            "TEAM-" => TeamId.NewId().ToString(),
            "USR-" => RegisteredUserId.NewId().ToString(),
            _ => throw new InvalidOperationException(),
        };

        // Assert
        Assert.StartsWith(expectedPrefix, text);
        Assert.Matches(CrockfordBody, text[expectedPrefix.Length..]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DomainId_IsValid_NullOrEmpty_ReturnsFalse(string? value)
    {
        // Arrange / Act / Assert
        Assert.False(StudentId.TryParse(value!, out _));
    }
}
