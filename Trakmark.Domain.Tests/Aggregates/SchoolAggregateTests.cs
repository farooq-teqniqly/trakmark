using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Aggregates;

/// <summary>
/// Tests for the School aggregate: creation, name validation,
/// team catalog management, and roster-free Team invariant.
/// </summary>
public sealed class SchoolAggregateTests
{
    // ── Create a school ───────────────────────────────────────────────────

    [Fact]
    public void CreateSchool_WithValidNameAndLevel_AssignsNewIdAndStoresProperties()
    {
        // Arrange
        var name = new SchoolName("Springfield High");
        var level = CompetitionLevel.HighSchool;

        // Act
        var school = School.Create(name, level);

        // Assert
        Assert.NotEqual(SchoolId.Empty, school.Id);
        Assert.Equal(name, school.Name);
        Assert.Equal(level, school.Level);
    }

    // ── School name must be valid ─────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateSchool_WithInvalidName_Throws(string rawName)
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentException>(() =>
        {
            var name = new SchoolName(rawName);
            School.Create(name, CompetitionLevel.HighSchool);
        });
    }

    // ── Add a team for a sport ────────────────────────────────────────────

    [Fact]
    public void AddTeam_ForNewSport_AddsTeamToSchoolCatalog()
    {
        // Arrange
        var school = School.Create(new SchoolName("Lincoln MS"), CompetitionLevel.MiddleSchool);
        var sport = Sport.TrackAndField;

        // Act
        school.AddTeam(sport);

        // Assert
        Assert.Single(school.Teams);
        var team = school.Teams.Single();
        Assert.Equal(sport, team.Sport);
        Assert.NotEqual(TeamId.Empty, team.Id);
    }

    [Fact]
    public void AddTeam_ForSportAlreadyFielded_ThrowsInvalidOperation()
    {
        // Arrange
        var school = School.Create(new SchoolName("Lincoln MS"), CompetitionLevel.MiddleSchool);
        school.AddTeam(Sport.TrackAndField);

        // Act / Assert
        var ex = Assert.Throws<InvalidOperationException>(() => school.AddTeam(Sport.TrackAndField));
        Assert.Contains("already", ex.Message);
    }

    // ── Roster membership is not stored on the team ───────────────────────

    [Fact]
    public void Team_HasNoStoredRosterCollection()
    {
        // Arrange
        var school = School.Create(new SchoolName("Riverside Elementary"), CompetitionLevel.Elementary);
        school.AddTeam(Sport.CrossCountry);
        var team = school.Teams.Single();

        // Act
        var publicPropertyNames = typeof(Team)
            .GetProperties()
            .Select(p => p.Name)
            .ToHashSet(StringComparer.Ordinal);

        // Assert — Team must expose exactly Id and Sport; no roster collection
        Assert.Equal(new HashSet<string>(StringComparer.Ordinal) { "Id", "Sport" }, publicPropertyNames);
    }
}
