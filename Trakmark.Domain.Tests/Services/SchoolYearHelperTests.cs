using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Catalog;
using Trakmark.Domain.Ids;
using Trakmark.Domain.Services;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Services;

/// <summary>Tests for the <c>SchoolYearHelper.ToSchoolYear</c> season-resolution logic.</summary>
public sealed class SchoolYearHelperTests
{
    private static readonly SchoolId SchoolId = SchoolId.NewId();
    private static readonly Sport Tf = Sport.TrackAndField;
    private static readonly Discipline Run100 = Discipline.Run(100);
    private static readonly Event E100 = new(Run100, Tf);
    private static readonly Placement Place1 = new(1);

    private static Student CreateStudentWithEnrollments(params SchoolYear[] years)
    {
        var student = new Student(StudentId.NewId(), new PersonName("Alice Johnson"));
        foreach (var year in years)
        {
            student.AddEnrollment(SchoolId, year, GradeLevel.Junior);
        }

        return student;
    }

    [Theory]
    [InlineData(8,  2025, 2025)]
    [InlineData(9,  2025, 2025)]
    [InlineData(12, 2025, 2025)]
    [InlineData(4,  2026, 2025)]
    public void ToSchoolYear_CorrectlyResolvesSeasonFromMeetDate(
        int month, int calendarYear, int expectedStartYear)
    {
        // Arrange
        var student = CreateStudentWithEnrollments(new SchoolYear(expectedStartYear));
        var meetDate = new MeetDate(new DateOnly(calendarYear, month, 15));
        var meet = Meet.Create(
            new MeetName("Test Meet"),
            meetDate,
            CompetitionLevel.HighSchool,
            Tf);

        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12000), Place1, null);

        var service = new SeasonViewService();

        // Act
        var results = service.GetSeasonResults(student, meet.Results, new SchoolYear(expectedStartYear)).ToList();

        // Assert
        Assert.Single(results);
    }
}
