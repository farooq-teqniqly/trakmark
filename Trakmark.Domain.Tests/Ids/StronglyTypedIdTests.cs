using System.Text.RegularExpressions;
using Trakmark.Domain.Ids;

namespace Trakmark.Domain.Tests.Ids;

/// <summary>
/// Tests for domain-identity-format spec: per-type prefix + six Crockford base32 chars.
/// </summary>
public sealed class StronglyTypedIdTests
{
    private static readonly Regex CrockfordBody = new(@"^[A-Z2-9]{6}$", RegexOptions.Compiled);

    // ── Scenario: New id matches the type's format ────────────────────────
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

    // ── Scenario: Each aggregate type uses its own prefix ─────────────────
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
            "SCH-"  => SchoolId.NewId().ToString(),
            "TEAM-" => TeamId.NewId().ToString(),
            "USR-"  => RegisteredUserId.NewId().ToString(),
            _       => throw new InvalidOperationException()
        };

        // Assert
        Assert.StartsWith(expectedPrefix, text);
        Assert.Matches(CrockfordBody, text[expectedPrefix.Length..]);
    }

    // ── Scenario: Reject a wrong prefix ───────────────────────────────────
    [Fact]
    public void ParseStudentId_RejectsWrongPrefix()
    {
        // Arrange / Act / Assert
        Assert.False(StudentId.TryParse("MEET-7F3K9M", out _));
    }

    // ── Scenario: Reject an ambiguous or out-of-charset character ─────────
    [Theory]
    [InlineData("STU-7F3K90")]  // contains '0'
    [InlineData("STU-7F3K9O")]  // contains 'O'
    [InlineData("STU-7F3K91")]  // contains '1'
    [InlineData("STU-7F3K9I")]  // contains 'I'
    [InlineData("STU-7F3K9L")]  // contains 'L'
    public void ParseStudentId_RejectsOutOfCharsetBody(string value)
    {
        // Arrange / Act / Assert
        Assert.False(StudentId.TryParse(value, out _));
    }

    // ── Scenario: Reject a wrong-length body ──────────────────────────────
    [Theory]
    [InlineData("STU-7F3K9")]    // five chars
    [InlineData("STU-7F3K9MM")]  // seven chars
    [InlineData("STU-")]         // zero chars
    public void ParseStudentId_RejectsWrongLengthBody(string value)
    {
        // Arrange / Act / Assert
        Assert.False(StudentId.TryParse(value, out _));
    }

    // ── Scenario: Identifiers round-trip through text ─────────────────────
    [Fact]
    public void StudentId_RoundTrips()
    {
        // Arrange
        var original = StudentId.NewId();

        // Act
        var roundTripped = StudentId.Parse(original.ToString());

        // Assert
        Assert.Equal(original, roundTripped);
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

    // ── Scenario: UserAccountId keeps the external key ────────────────────
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

    // ── Parse throws for ill-formed values ────────────────────────────────
    [Fact]
    public void Parse_ThrowsForIllFormedValue()
    {
        // Arrange / Act / Assert
        Assert.Throws<FormatException>(() => StudentId.Parse("MEET-7F3K9M"));
    }

    // ── Empty sentinel ────────────────────────────────────────────────────
    [Fact]
    public void StudentId_Empty_IsDistinctFromNewId()
    {
        // Arrange / Act
        var empty = StudentId.Empty;
        var newId = StudentId.NewId();

        // Assert
        Assert.NotEqual(empty, newId);
    }
}
