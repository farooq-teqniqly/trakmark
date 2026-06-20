using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.Services;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Services;

/// <summary>Tests for <see cref="CompetitionLevelMatchService"/>.</summary>
public sealed class CompetitionLevelMatchServiceTests
{
    private static School CreateHsSchool() =>
        School.Create(new SchoolName("Lincoln High"), CompetitionLevel.HighSchool);

    private static School CreateMsSchool() =>
        School.Create(new SchoolName("Central Middle"), CompetitionLevel.MiddleSchool);

    private static Dictionary<SchoolId, School> SchoolMap(params School[] schools) =>
        schools.ToDictionary(s => s.Id);

    private static Student StudentEnrolledAt(School school, SchoolYear year)
    {
        var student = new Student(StudentId.NewId(), new PersonName("Alice Johnson"));
        student.AddEnrollment(school.Id, year, GradeLevel.Junior);
        return student;
    }

    private static Meet HighSchoolMeet2025() =>
        Meet.Create(
            new MeetName("HS Spring Invitational"),
            new MeetDate(new DateOnly(2026, 4, 10)),
            CompetitionLevel.HighSchool,
            Sport.TrackAndField
        );

    private static Meet MiddleSchoolMeet2025() =>
        Meet.Create(
            new MeetName("MS Spring Invitational"),
            new MeetDate(new DateOnly(2026, 4, 10)),
            CompetitionLevel.MiddleSchool,
            Sport.TrackAndField
        );

    [Fact]
    public void LevelMatch_Validate_MismatchedLevel_ReturnsFalse()
    {
        // Arrange
        var hsSchool = CreateHsSchool();
        var student = StudentEnrolledAt(hsSchool, new SchoolYear(2025));
        var msMeet = MiddleSchoolMeet2025();
        var schools = SchoolMap(hsSchool);
        // Act
        var isValid = CompetitionLevelMatchService.IsLevelMatch(student, msMeet, schools);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void LevelMatch_Validate_MatchingLevel_ReturnsTrue()
    {
        // Arrange
        var hsSchool = CreateHsSchool();
        var student = StudentEnrolledAt(hsSchool, new SchoolYear(2025));
        var hsMeet = HighSchoolMeet2025();
        var schools = SchoolMap(hsSchool);
        // Act
        var isValid = CompetitionLevelMatchService.IsLevelMatch(student, hsMeet, schools);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void LevelMatch_HistoricalMeet_ValidatesAgainstSeasonEnrollment_NotCurrentEnrollment()
    {
        // Arrange
        var msSchool = CreateMsSchool();
        var hsSchool = CreateHsSchool();

        var student = new Student(StudentId.NewId(), new PersonName("Carol Davis"));
        student.AddEnrollment(msSchool.Id, new SchoolYear(2023), GradeLevel.MiddleSchool8th);
        student.AddEnrollment(hsSchool.Id, new SchoolYear(2025), GradeLevel.Junior);

        var historicalMsMeet = Meet.Create(
            new MeetName("MS Regional 2023"),
            new MeetDate(new DateOnly(2024, 3, 15)),
            CompetitionLevel.MiddleSchool,
            Sport.TrackAndField
        );

        var schools = SchoolMap(msSchool, hsSchool);

        // Act
        var isValid = CompetitionLevelMatchService.IsLevelMatch(student, historicalMsMeet, schools);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void LevelMatch_HistoricalMeet_MismatchedHistoricalLevel_ReturnsFalse()
    {
        // Arrange
        var msSchool = CreateMsSchool();
        var hsSchool = CreateHsSchool();

        var student = new Student(StudentId.NewId(), new PersonName("Dave Wilson"));
        student.AddEnrollment(msSchool.Id, new SchoolYear(2023), GradeLevel.MiddleSchool8th);
        student.AddEnrollment(hsSchool.Id, new SchoolYear(2025), GradeLevel.Junior);

        var historicalHsMeet = Meet.Create(
            new MeetName("HS Regional 2023"),
            new MeetDate(new DateOnly(2024, 3, 15)),
            CompetitionLevel.HighSchool,
            Sport.TrackAndField
        );

        var schools = SchoolMap(msSchool, hsSchool);

        // Act
        var isValid = CompetitionLevelMatchService.IsLevelMatch(student, historicalHsMeet, schools);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void LevelMatch_SchoolNotInDictionary_ReturnsFalse()
    {
        // Arrange
        var hsSchool = CreateHsSchool();
        var student = StudentEnrolledAt(hsSchool, new SchoolYear(2025));
        var hsMeet = HighSchoolMeet2025();
        var emptySchools = new Dictionary<SchoolId, School>();

        // Act
        var isValid = CompetitionLevelMatchService.IsLevelMatch(student, hsMeet, emptySchools);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void LevelMatch_NoEnrollmentForSeason_ReturnsFalse()
    {
        // Arrange
        var student = new Student(StudentId.NewId(), new PersonName("Eve Thomas"));
        var meet = HighSchoolMeet2025();

        // Act
        var isValid = CompetitionLevelMatchService.IsLevelMatch(
            student,
            meet,
            new Dictionary<SchoolId, School>()
        );

        // Assert
        Assert.False(isValid);
    }
}
