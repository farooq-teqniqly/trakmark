using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests;

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
        Assert.Equal(sport, school.Teams.Single().Sport);
    }

    [Fact]
    public void AddTeam_ForSportAlreadyFielded_ThrowsInvalidOperation()
    {
        // Arrange
        var school = School.Create(new SchoolName("Lincoln MS"), CompetitionLevel.MiddleSchool);
        school.AddTeam(Sport.TrackAndField);

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() => school.AddTeam(Sport.TrackAndField));
    }

    [Fact]
    public void AddTeam_AssignsNewTeamId()
    {
        // Arrange
        var school = School.Create(new SchoolName("Central HS"), CompetitionLevel.HighSchool);

        // Act
        school.AddTeam(Sport.CrossCountry);

        // Assert
        var team = school.Teams.Single();
        Assert.NotEqual(TeamId.Empty, team.Id);
    }

    // ── Roster membership is not stored on the team ───────────────────────

    [Fact]
    public void Team_HasNoStoredRosterCollection()
    {
        // Arrange
        var school = School.Create(new SchoolName("Riverside Elementary"), CompetitionLevel.Elementary);
        school.AddTeam(Sport.CrossCountry);
        var team = school.Teams.Single();

        // Act / Assert — Team must not expose any membership collection
        Assert.False(HasRosterProperty(team), "Team must not contain a stored list of student members.");
    }

    /// <summary>
    /// Reflects whether the <see cref="Team"/> type exposes any property
    /// that holds a collection of student membership records.
    /// </summary>
    private static bool HasRosterProperty(Team team)
    {
        var type = team.GetType();
        return type.GetProperties()
            .Any(p =>
                p.Name.Contains("roster", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains("member", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains("student", StringComparison.OrdinalIgnoreCase));
    }
}
