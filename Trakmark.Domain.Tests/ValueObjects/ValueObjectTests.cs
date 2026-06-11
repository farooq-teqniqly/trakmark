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

    [Theory]
    [InlineData(2024, 2024, false, true,  false, true)]   // equal: < false, <= true, > false, >= true
    [InlineData(2025, 2024, false, false, true,  true)]   // later: < false, <= false, > true, >= true
    [InlineData(2024, 2025, true,  true,  false, false)]  // earlier: < true, <= true, > false, >= false
    public void SchoolYear_ComparisonOperators_Correct(
        int leftYear, int rightYear,
        bool expectedLt, bool expectedLte,
        bool expectedGt, bool expectedGte)
    {
        // Arrange
        var left  = new SchoolYear(leftYear);
        var right = new SchoolYear(rightYear);

        // Act / Assert
        Assert.Equal(expectedLt,  left < right);
        Assert.Equal(expectedLte, left <= right);
        Assert.Equal(expectedGt,  left > right);
        Assert.Equal(expectedGte, left >= right);
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

    // ── MeetName equality ─────────────────────────────────────────────────

    [Theory]
    [InlineData("Spring Classic",  "Spring Classic",  true)]   // same value → equal
    [InlineData("Spring Classic",  "spring classic",  true)]   // case-insensitive → equal
    [InlineData("Spring Classic",  "Fall Classic",    false)]  // different value → not equal
    public void MeetName_Equality_CaseInsensitive(string valA, string valB, bool expectedEqual)
    {
        // Arrange
        var a = new MeetName(valA);
        var b = new MeetName(valB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void MeetName_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var n = new MeetName("Spring Classic");

        // Act / Assert
        Assert.False(n.Equals((MeetName?)null));
        Assert.False(n == null);
        Assert.True(n != null);
        // null as left operand — exercises the `?? right is null` branch of op_Equality
        Assert.False(null == n);
        Assert.True(null != n);
    }

    [Fact]
    public void MeetName_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var n = new MeetName("Spring Classic");

        // Act / Assert
        Assert.False(n.Equals((object)"Spring Classic"));
    }

    [Fact]
    public void MeetName_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new MeetName("Spring Classic");
        var b = new MeetName("spring classic");

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void MeetName_ToString_ReturnsValue()
    {
        // Arrange / Act / Assert
        Assert.Equal("Spring Classic", new MeetName("Spring Classic").ToString());
    }

    // ── PersonName equality ───────────────────────────────────────────────

    [Theory]
    [InlineData("Alice",  "Alice",  true)]   // same value → equal
    [InlineData("Alice",  "alice",  true)]   // case-insensitive → equal
    [InlineData("Alice",  "Bob",    false)]  // different value → not equal
    public void PersonName_Equality_CaseInsensitive(string valA, string valB, bool expectedEqual)
    {
        // Arrange
        var a = new PersonName(valA);
        var b = new PersonName(valB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void PersonName_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var n = new PersonName("Alice");

        // Act / Assert
        Assert.False(n.Equals((PersonName?)null));
        Assert.False(n == null);
        Assert.True(n != null);
        // null as left operand — exercises the `?? right is null` branch of op_Equality
        Assert.False(null == n);
        Assert.True(null != n);
    }

    [Fact]
    public void PersonName_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var n = new PersonName("Alice");

        // Act / Assert
        Assert.False(n.Equals((object)"Alice"));
    }

    [Fact]
    public void PersonName_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new PersonName("Alice");
        var b = new PersonName("ALICE");

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void PersonName_ToString_ReturnsValue()
    {
        // Arrange / Act / Assert
        Assert.Equal("Alice", new PersonName("Alice").ToString());
    }

    // ── SchoolName equality ───────────────────────────────────────────────

    [Theory]
    [InlineData("Springfield High",  "Springfield High",  true)]   // same value → equal
    [InlineData("Springfield High",  "springfield high",  true)]   // case-insensitive → equal
    [InlineData("Springfield High",  "Central High",      false)]  // different value → not equal
    public void SchoolName_Equality_CaseInsensitive(string valA, string valB, bool expectedEqual)
    {
        // Arrange
        var a = new SchoolName(valA);
        var b = new SchoolName(valB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void SchoolName_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var n = new SchoolName("Springfield High");

        // Act / Assert
        Assert.False(n.Equals((SchoolName?)null));
        Assert.False(n == null);
        Assert.True(n != null);
        // null as left operand — exercises the `?? right is null` branch of op_Equality
        Assert.False(null == n);
        Assert.True(null != n);
    }

    [Fact]
    public void SchoolName_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var n = new SchoolName("Springfield High");

        // Act / Assert
        Assert.False(n.Equals((object)"Springfield High"));
    }

    [Fact]
    public void SchoolName_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new SchoolName("Springfield High");
        var b = new SchoolName("SPRINGFIELD HIGH");

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void SchoolName_ToString_ReturnsValue()
    {
        // Arrange / Act / Assert
        Assert.Equal("Springfield High", new SchoolName("Springfield High").ToString());
    }

    // ── TeamName equality ─────────────────────────────────────────────────

    [Theory]
    [InlineData("Varsity Boys",  "Varsity Boys",  true)]   // same value → equal
    [InlineData("Varsity Boys",  "varsity boys",  true)]   // case-insensitive → equal
    [InlineData("Varsity Boys",  "JV Girls",      false)]  // different value → not equal
    public void TeamName_Equality_CaseInsensitive(string valA, string valB, bool expectedEqual)
    {
        // Arrange
        var a = new TeamName(valA);
        var b = new TeamName(valB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void TeamName_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var n = new TeamName("Varsity Boys");

        // Act / Assert
        Assert.False(n.Equals((TeamName?)null));
        Assert.False(n == null);
        Assert.True(n != null);
        // null as left operand — exercises the `?? right is null` branch of op_Equality
        Assert.False(null == n);
        Assert.True(null != n);
    }

    [Fact]
    public void TeamName_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var n = new TeamName("Varsity Boys");

        // Act / Assert
        Assert.False(n.Equals((object)"Varsity Boys"));
    }

    [Fact]
    public void TeamName_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new TeamName("Varsity Boys");
        var b = new TeamName("VARSITY BOYS");

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void TeamName_ToString_ReturnsValue()
    {
        // Arrange / Act / Assert
        Assert.Equal("Varsity Boys", new TeamName("Varsity Boys").ToString());
    }

    // ── GradeLevel equality ───────────────────────────────────────────────

    [Theory]
    [InlineData("same",  true)]   // same singleton → equal
    [InlineData("diff",  false)]  // different singleton → not equal
    public void GradeLevel_Equality_BySingleton(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = GradeLevel.Freshman;
        var b = scenario == "same" ? GradeLevel.Freshman : GradeLevel.Senior;

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void GradeLevel_Equals_Null_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(GradeLevel.Freshman.Equals((GradeLevel?)null));
        Assert.False(GradeLevel.Freshman == null);
        Assert.True(GradeLevel.Freshman != null);
        // null as left operand — exercises the `?? right is null` branch of op_Equality
        Assert.False(null == GradeLevel.Freshman);
        Assert.True(null != GradeLevel.Freshman);
    }

    [Fact]
    public void GradeLevel_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(GradeLevel.Freshman.Equals((object)"Freshman"));
    }

    [Fact]
    public void GradeLevel_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("Freshman", GradeLevel.Freshman.ToString());
        Assert.Equal("Senior", GradeLevel.Senior.ToString());
    }

    // ── CompetitionLevel equality ─────────────────────────────────────────

    [Theory]
    [InlineData("same",  true)]   // same singleton → equal
    [InlineData("diff",  false)]  // different singleton → not equal
    public void CompetitionLevel_Equality_BySingleton(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = CompetitionLevel.HighSchool;
        var b = scenario == "same" ? CompetitionLevel.HighSchool : CompetitionLevel.MiddleSchool;

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void CompetitionLevel_Equals_Null_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(CompetitionLevel.HighSchool.Equals((CompetitionLevel?)null));
        Assert.False(CompetitionLevel.HighSchool == null);
        Assert.True(CompetitionLevel.HighSchool != null);
        // null as left operand — exercises the `?? right is null` branch of op_Equality
        Assert.False(null == CompetitionLevel.HighSchool);
        Assert.True(null != CompetitionLevel.HighSchool);
    }

    [Fact]
    public void CompetitionLevel_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(CompetitionLevel.HighSchool.Equals((object)"High School"));
    }

    [Fact]
    public void CompetitionLevel_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("High School", CompetitionLevel.HighSchool.ToString());
        Assert.Equal("Middle School", CompetitionLevel.MiddleSchool.ToString());
    }

    // ── Sport equality ────────────────────────────────────────────────────

    [Theory]
    [InlineData("same",  true)]   // same singleton → equal
    [InlineData("diff",  false)]  // different singleton → not equal
    public void Sport_Equality_BySingleton(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = Sport.TrackAndField;
        var b = scenario == "same" ? Sport.TrackAndField : Sport.CrossCountry;

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void Sport_Equals_Null_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(Sport.TrackAndField.Equals((Sport?)null));
        Assert.False(Sport.TrackAndField == null);
        Assert.True(Sport.TrackAndField != null);
        // null as left operand — exercises the `?? right is null` branch of op_Equality
        Assert.False(null == Sport.TrackAndField);
        Assert.True(null != Sport.TrackAndField);
    }

    [Fact]
    public void Sport_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(Sport.TrackAndField.Equals((object)"Track & Field"));
    }

    [Fact]
    public void Sport_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("Track & Field", Sport.TrackAndField.ToString());
        Assert.Equal("Cross-Country", Sport.CrossCountry.ToString());
    }
}
