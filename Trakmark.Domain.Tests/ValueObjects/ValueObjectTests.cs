using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>
/// Tests for value-object invariants: PersonName, SchoolName, MeetName, TeamName,
/// plus closed-set types GradeLevel, CompetitionLevel, Sport, SchoolYear, MeetDate.
/// </summary>
public sealed class ValueObjectTests
{
    // ── PersonName ────────────────────────────────────────────────────────
    [Theory]
    [InlineData("Alice")]
    [InlineData("Bob Smith")]
    [InlineData("  Alice  ")]  // trimmed to non-empty
    public void PersonName_AcceptsValidName(string input)
    {
        // Arrange / Act
        var name = new PersonName(input);

        // Assert
        Assert.Equal(input.Trim(), name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void PersonName_RejectsEmptyOrWhitespace(string input)
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentException>(() => new PersonName(input));
    }

    // ── SchoolName ────────────────────────────────────────────────────────
    [Theory]
    [InlineData("Springfield High")]
    [InlineData("  Central Middle School  ")]
    public void SchoolName_AcceptsValidName(string input)
    {
        // Arrange / Act
        var name = new SchoolName(input);

        // Assert
        Assert.Equal(input.Trim(), name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SchoolName_RejectsEmptyOrWhitespace(string input)
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentException>(() => new SchoolName(input));
    }

    // ── MeetName ──────────────────────────────────────────────────────────
    [Theory]
    [InlineData("Invitational 2024")]
    [InlineData("  Spring Classic  ")]
    public void MeetName_AcceptsValidName(string input)
    {
        // Arrange / Act
        var name = new MeetName(input);

        // Assert
        Assert.Equal(input.Trim(), name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void MeetName_RejectsEmptyOrWhitespace(string input)
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentException>(() => new MeetName(input));
    }

    // ── TeamName ──────────────────────────────────────────────────────────
    [Theory]
    [InlineData("Varsity Boys")]
    [InlineData("  JV Girls  ")]
    public void TeamName_AcceptsValidName(string input)
    {
        // Arrange / Act
        var name = new TeamName(input);

        // Assert
        Assert.Equal(input.Trim(), name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void TeamName_RejectsEmptyOrWhitespace(string input)
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentException>(() => new TeamName(input));
    }

    // ── SchoolYear ────────────────────────────────────────────────────────
    [Fact]
    public void SchoolYear_IsOrderable()
    {
        // Arrange
        var earlier = new SchoolYear(2023);
        var later = new SchoolYear(2024);

        // Assert
        Assert.True(earlier < later);
        Assert.True(later > earlier);
        Assert.Equal(earlier, new SchoolYear(2023));
    }

    [Fact]
    public void SchoolYear_RejectsInvalidYear()
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SchoolYear(0));
    }

    // ── GradeLevel ────────────────────────────────────────────────────────
    [Fact]
    public void GradeLevel_ClosedSet_ContainsExpectedValues()
    {
        // Arrange / Act / Assert — smoke-test the closed set
        _ = GradeLevel.Freshman;
        _ = GradeLevel.Sophomore;
        _ = GradeLevel.Junior;
        _ = GradeLevel.Senior;
        _ = GradeLevel.MiddleSchool8th;
    }

    // ── CompetitionLevel ──────────────────────────────────────────────────
    [Fact]
    public void CompetitionLevel_ClosedSet_ContainsExpectedValues()
    {
        _ = CompetitionLevel.HighSchool;
        _ = CompetitionLevel.MiddleSchool;
        _ = CompetitionLevel.Elementary;
    }

    // ── Sport ─────────────────────────────────────────────────────────────
    [Fact]
    public void Sport_ClosedSet_ContainsExpectedValues()
    {
        _ = Sport.TrackAndField;
        _ = Sport.CrossCountry;
    }

    // ── MeetDate ──────────────────────────────────────────────────────────
    [Fact]
    public void MeetDate_WrapsDateOnly()
    {
        // Arrange
        var date = new DateOnly(2024, 5, 15);

        // Act
        var meetDate = new MeetDate(date);

        // Assert
        Assert.Equal(date, meetDate.Value);
    }

    [Fact]
    public void MeetDate_Equality_WorksCorrectly()
    {
        // Arrange
        var date = new DateOnly(2024, 5, 15);
        var a = new MeetDate(date);
        var b = new MeetDate(date);

        // Assert
        Assert.Equal(a, b);
    }
}
