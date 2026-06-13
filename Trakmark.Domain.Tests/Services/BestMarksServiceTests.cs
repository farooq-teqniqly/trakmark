using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Catalog;
using Trakmark.Domain.Ids;
using Trakmark.Domain.Services;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Services;

/// <summary>Tests for <see cref="BestMarksService"/>.</summary>
public sealed class BestMarksServiceTests
{
    private static readonly SchoolId SchoolId = SchoolId.NewId();
    private static readonly Sport Tf = Sport.TrackAndField;

    private static readonly Discipline Run100 = Discipline.Run(100);
    private static readonly Discipline Run200 = Discipline.Run(200);
    private static readonly Discipline LongJump = Discipline.Jump("Long Jump");
    private static readonly Discipline PlaceOnlyDiscipline = Discipline.PlaceOnly("Pentathlon");
    private static readonly Discipline Relay4x100 = Discipline.RelayRun(400);

    private static readonly Event E100 = new(Run100, Tf);
    private static readonly Event E200 = new(Run200, Tf);
    private static readonly Event ELongJump = new(LongJump, Tf);
    private static readonly Event EPlaceOnly = new(PlaceOnlyDiscipline, Tf);
    private static readonly Event ERelay = new(Relay4x100, Tf);

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

    [Theory]
    [InlineData(12000, 11500, 11500)]
    [InlineData(11500, 12000, 11500)]
    [InlineData(11500, 11500, 11500)]
    public void SeasonBest_TimeDiscipline_LowestTimeWins(int ms1, int ms2, int expectedMs)
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(ms1), Place1, null);
        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(ms2), Place2, null);

        var service = new BestMarksService();

        // Act
        var best = service.SeasonBest(student, Run100, Season2025, meet.Results);

        // Assert
        Assert.NotNull(best);
        var timeMark = Assert.IsType<TimeMark>(best);
        Assert.Equal(expectedMs, timeMark.Milliseconds);
    }

    [Theory]
    [InlineData(540, 580, 580)]
    [InlineData(580, 540, 580)]
    [InlineData(540, 540, 540)]
    public void SeasonBest_DistanceDiscipline_HighestDistanceWins(int cm1, int cm2, int expectedCm)
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, ELongJump, ResultStatus.Finished, new DistanceMark(cm1), Place1, null);
        meet.RecordResult(student.Id, ELongJump, ResultStatus.Finished, new DistanceMark(cm2), Place2, null);

        var service = new BestMarksService();

        // Act
        var best = service.SeasonBest(student, LongJump, Season2025, meet.Results);

        // Assert
        Assert.NotNull(best);
        var distanceMark = Assert.IsType<DistanceMark>(best);
        Assert.Equal(expectedCm, distanceMark.Centimetres);
    }

    [Theory]
    [InlineData(ResultStatus.DidNotFinish)]
    [InlineData(ResultStatus.Disqualified)]
    [InlineData(ResultStatus.DidNotStart)]
    [InlineData(ResultStatus.NoMark)]
    public void SeasonBest_NonFinishedResults_Excluded(ResultStatus nonFinishedStatus)
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, E100, nonFinishedStatus, null, null, null);

        var service = new BestMarksService();

        // Act
        var best = service.SeasonBest(student, Run100, Season2025, meet.Results);

        // Assert
        Assert.Null(best);
    }

    [Fact]
    public void SeasonBest_PlaceOnlyDiscipline_ReturnsNull()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, EPlaceOnly, ResultStatus.Finished, null, Place1, null);

        var service = new BestMarksService();

        // Act
        var best = service.SeasonBest(student, PlaceOnlyDiscipline, Season2025, meet.Results);

        // Assert
        Assert.Null(best);
    }

    [Fact]
    public void SeasonBest_RelayResults_Excluded()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, ERelay, ResultStatus.Finished, new TimeMark(50000), Place1, null);

        var service = new BestMarksService();

        // Act
        var best = service.SeasonBest(student, Relay4x100, Season2025, meet.Results);

        // Assert
        Assert.Null(best);
    }

    [Fact]
    public void PersonalBest_SpansSeasons_SelectsOverallBest()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2024, Season2025);

        var meet2024 = CreateMeet(DateInSeason2024);
        var meet2025 = CreateMeet(DateInSeason2025);

        meet2024.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12000), Place2, null);
        meet2025.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(11500), Place1, null);

        var allResults = meet2024.Results.Concat(meet2025.Results);
        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, Run100, allResults);

        // Assert
        Assert.NotNull(pb);
        var timeMark = Assert.IsType<TimeMark>(pb);
        Assert.Equal(11500, timeMark.Milliseconds);
    }

    [Fact]
    public void PersonalBest_NewBetterResult_UpdatesImmediately()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12000), Place2, null);

        var service = new BestMarksService();
        var pbBefore = service.PersonalBest(student, Run100, meet.Results);

        // Act
        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(11500), Place1, null);
        var pbAfter = service.PersonalBest(student, Run100, meet.Results);

        // Assert
        var before = Assert.IsType<TimeMark>(pbBefore);
        var after = Assert.IsType<TimeMark>(pbAfter);
        Assert.Equal(12000, before.Milliseconds);
        Assert.Equal(11500, after.Milliseconds);
    }

    [Fact]
    public void PersonalBest_JvMark_IsEligible()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(11800), Place1, Tier.JV);

        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, Run100, meet.Results);

        // Assert
        Assert.NotNull(pb);
        var timeMark = Assert.IsType<TimeMark>(pb);
        Assert.Equal(11800, timeMark.Milliseconds);
    }

    [Fact]
    public void PersonalBest_JvAndVarsity_TierAgnostic_BestWins()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12000), Place2, Tier.Varsity);
        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(11700), Place1, Tier.JV);

        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, Run100, meet.Results);

        // Assert
        Assert.NotNull(pb);
        var timeMark = Assert.IsType<TimeMark>(pb);
        Assert.Equal(11700, timeMark.Milliseconds);
    }

    [Fact]
    public void PersonalBest_RelayResult_Excluded()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, ERelay, ResultStatus.Finished, new TimeMark(48000), Place1, null);

        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, Relay4x100, meet.Results);

        // Assert
        Assert.Null(pb);
    }

    [Fact]
    public void PersonalBest_PlaceOnlyDiscipline_ReturnsNull()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, EPlaceOnly, ResultStatus.Finished, null, Place1, null);

        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, PlaceOnlyDiscipline, meet.Results);

        // Assert
        Assert.Null(pb);
    }

    [Fact]
    public void PersonalBest_EmptyCollection_ReturnsNull()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, Run100, Enumerable.Empty<Result>());

        // Assert
        Assert.Null(pb);
    }

    [Fact]
    public void PersonalBest_NonMatchingDiscipline_ReturnsNull()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, E200, ResultStatus.Finished, new TimeMark(25000), Place1, null);

        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, Run100, meet.Results);

        // Assert
        Assert.Null(pb);
    }

    [Fact]
    public void PersonalBest_AllResultsEqualMark_ReturnsFirstEligibleMark()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12000), Place1, null);
        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12000), Place2, null);
        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12000), Place1, null);

        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, Run100, meet.Results);

        // Assert
        Assert.NotNull(pb);
        var timeMark = Assert.IsType<TimeMark>(pb);
        Assert.Equal(12000, timeMark.Milliseconds);
    }

    [Theory]
    [InlineData(540, 580, 580)]
    [InlineData(620, 570, 620)]
    public void PersonalBest_DistanceDiscipline_MultiSeason_HighestWins(int cm2024, int cm2025, int expectedCm)
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2024, Season2025);

        var meet2024 = CreateMeet(DateInSeason2024);
        var meet2025 = CreateMeet(DateInSeason2025);

        meet2024.RecordResult(student.Id, ELongJump, ResultStatus.Finished, new DistanceMark(cm2024), Place1, null);
        meet2025.RecordResult(student.Id, ELongJump, ResultStatus.Finished, new DistanceMark(cm2025), Place1, null);

        var allResults = meet2024.Results.Concat(meet2025.Results);
        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, LongJump, allResults);

        // Assert
        Assert.NotNull(pb);
        var distanceMark = Assert.IsType<DistanceMark>(pb);
        Assert.Equal(expectedCm, distanceMark.Centimetres);
    }
}
