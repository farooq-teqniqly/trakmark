using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Catalog;
using Trakmark.Domain.Ids;
using Trakmark.Domain.Services;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Services;

/// <summary>Tests for <see cref="SeasonViewService"/>.</summary>
public sealed class SeasonViewServiceTests
{
    private static readonly SchoolId SchoolId = SchoolId.NewId();
    private static readonly Sport Tf = Sport.TrackAndField;

    private static readonly Discipline Run100 = Discipline.Run(100);
    private static readonly Discipline Run200 = Discipline.Run(200);

    private static readonly Event E100 = new(Run100, Tf);
    private static readonly Event E200 = new(Run200, Tf);

    private static readonly Placement Place1 = new(1);
    private static readonly Placement Place2 = new(2);

    private static readonly SchoolYear Season2024 = new(2024);
    private static readonly SchoolYear Season2025 = new(2025);

    private static readonly MeetDate DateInSeason2024 = new(new DateOnly(2025, 4, 10));
    private static readonly MeetDate DateInSeason2025 = new(new DateOnly(2026, 4, 10));

    private static Student CreateStudentWithEnrollments(params SchoolYear[] years)
    {
        var student = new Student(StudentId.NewId(), new PersonName("Alice Johnson"));
        foreach (var year in years)
        {
            student.AddEnrollment(SchoolId, year, GradeLevel.Junior);
        }

        return student;
    }

    private static Meet CreateMeet(MeetDate date) =>
        Meet.Create(new MeetName("Test Meet"), date, CompetitionLevel.HighSchool, Tf);

    [Fact]
    public void GetSeasonResults_NullStudent_ThrowsArgumentNullException()
    {
        // Act / Assert
        Assert.Throws<ArgumentNullException>(() =>
            SeasonViewService
                .GetSeasonResults(null!, Enumerable.Empty<Result>(), Season2025)
                .ToList()
        );
    }

    [Fact]
    public void GetSeasonResults_NullResults_ThrowsArgumentNullException()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);

        // Act / Assert
        Assert.Throws<ArgumentNullException>(() =>
            SeasonViewService.GetSeasonResults(student, null!, Season2025).ToList()
        );
    }

    [Fact]
    public void GetSeasonResults_OtherStudentResults_AreExcluded()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var otherStudent = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(
            student.Id,
            E100,
            ResultStatus.Finished,
            new TimeMark(12000),
            Place1,
            null
        );
        meet.RecordResult(
            otherStudent.Id,
            E100,
            ResultStatus.Finished,
            new TimeMark(11000),
            Place1,
            null
        );

        // Act
        var results = SeasonViewService
            .GetSeasonResults(student, meet.Results, Season2025)
            .ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal(student.Id, results[0].StudentId);
    }

    [Fact]
    public void SeasonView_CurrentSeason_IsLatestEnrollment()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2024, Season2025);

        // Act
        var current = student.Career.Current;

        // Assert
        Assert.NotNull(current);
        Assert.Equal(Season2025, current.SchoolYear);
    }

    [Fact]
    public void SeasonView_PastSeasons_AreAllButLatestEnrollment()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2024, Season2025);

        // Act
        var past = student.Career.PastSeasons.ToList();

        // Assert
        Assert.Single(past);
        Assert.Equal(Season2024, past[0].SchoolYear);
    }

    [Fact]
    public void SeasonView_CurrentSeasonResults_InEntryOrder()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(
            student.Id,
            E100,
            ResultStatus.Finished,
            new TimeMark(12000),
            Place1,
            null
        );
        meet.RecordResult(
            student.Id,
            E200,
            ResultStatus.Finished,
            new TimeMark(25000),
            Place2,
            null
        );

        // Act
        var results = SeasonViewService
            .GetSeasonResults(student, meet.Results, Season2025)
            .ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal(1, results[0].Order);
        Assert.Equal(2, results[1].Order);
    }

    [Fact]
    public void SeasonView_PastSeasonResults_FilteredBySchoolYear()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2024, Season2025);
        var meetPast = CreateMeet(DateInSeason2024);
        var meetCurrent = CreateMeet(DateInSeason2025);

        meetPast.RecordResult(
            student.Id,
            E100,
            ResultStatus.Finished,
            new TimeMark(12500),
            Place1,
            null
        );
        meetCurrent.RecordResult(
            student.Id,
            E100,
            ResultStatus.Finished,
            new TimeMark(11900),
            Place1,
            null
        );

        var allResults = meetPast.Results.Concat(meetCurrent.Results);

        // Act
        var past = SeasonViewService.GetSeasonResults(student, allResults, Season2024).ToList();

        // Assert
        Assert.Single(past);
        Assert.Equal(new TimeMark(12500), past[0].Mark as TimeMark);
    }

    [Theory]
    [InlineData(8, 2025, 2025)] // August — fall start of season
    [InlineData(9, 2025, 2025)] // September — fall start of season
    [InlineData(12, 2025, 2025)] // December — still same season
    [InlineData(4, 2026, 2025)] // April — spring end of season, season started 2025
    public void SeasonBoundaries_CorrectlyResolveSchoolYearFromMeetDate(
        int month,
        int calendarYear,
        int expectedStartYear
    )
    {
        // Arrange
        var season = new SchoolYear(expectedStartYear);
        var student = CreateStudentWithEnrollments(season);
        var meet = Meet.Create(
            new MeetName("Test Meet"),
            new MeetDate(new DateOnly(calendarYear, month, 15)),
            CompetitionLevel.HighSchool,
            Tf
        );

        meet.RecordResult(
            student.Id,
            E100,
            ResultStatus.Finished,
            new TimeMark(12000),
            Place1,
            null
        );

        // Act
        var results = SeasonViewService.GetSeasonResults(student, meet.Results, season).ToList();

        // Assert
        Assert.Single(results);
    }
}
