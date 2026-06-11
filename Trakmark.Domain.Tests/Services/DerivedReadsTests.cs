using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Catalog;
using Trakmark.Domain.Ids;
using Trakmark.Domain.Services;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Services;

/// <summary>
/// Tests for section 8: Derived reads — season view and personal/season bests.
/// Covers <c>view-athlete-season</c> and <c>personal-and-season-best</c> specs.
/// </summary>
public sealed class DerivedReadsTests
{
    // ── Fixtures ──────────────────────────────────────────────────────────────

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

    /// <summary>School year starting in 2024 (2024-25 season).</summary>
    private static readonly SchoolYear Season2024 = new(2024);

    /// <summary>School year starting in 2025 (2025-26 season).</summary>
    private static readonly SchoolYear Season2025 = new(2025);

    /// <summary>A meet date in spring 2025 — falls in the 2024-25 school year.</summary>
    private static readonly MeetDate DateInSeason2024 = new(new DateOnly(2025, 4, 10));

    /// <summary>A meet date in spring 2026 — falls in the 2025-26 school year.</summary>
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

    // ── Season view: current and past seasons ─────────────────────────────────

    /// <summary>
    /// Scenario: Current season is the current enrollment.
    /// The enrollment with the latest SchoolYear is identified as current.
    /// </summary>
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

    /// <summary>
    /// Scenario: Current season is the current enrollment — past seasons are remaining.
    /// Past seasons are all enrollments except the latest.
    /// </summary>
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

    /// <summary>
    /// Scenario: Show a student's current season results.
    /// Results for meets in the current season's SchoolYear are returned in entry order.
    /// </summary>
    [Fact]
    public void SeasonView_CurrentSeasonResults_InEntryOrder()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12000), Place1, null);
        meet.RecordResult(student.Id, E200, ResultStatus.Finished, new TimeMark(25000), Place2, null);

        var service = new SeasonViewService();

        // Act
        var results = service.GetSeasonResults(student, meet.Results, Season2025).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal(1, results[0].Order);
        Assert.Equal(2, results[1].Order);
    }

    /// <summary>
    /// Scenario: Navigate to a past season.
    /// Only results whose meet falls within the selected past SchoolYear are returned.
    /// </summary>
    [Fact]
    public void SeasonView_PastSeasonResults_FilteredBySchoolYear()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2024, Season2025);
        var meetPast = CreateMeet(DateInSeason2024);
        var meetCurrent = CreateMeet(DateInSeason2025);

        meetPast.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12500), Place1, null);
        meetCurrent.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(11900), Place1, null);

        var allResults = meetPast.Results.Concat(meetCurrent.Results);
        var service = new SeasonViewService();

        // Act
        var past = service.GetSeasonResults(student, allResults, Season2024).ToList();

        // Assert — only the meet in the 2024-25 season
        Assert.Single(past);
        Assert.Equal(new TimeMark(12500), past[0].Mark as TimeMark);
    }

    // ── Season best ───────────────────────────────────────────────────────────

    /// <summary>
    /// Scenario: Best time wins for a time discipline.
    /// Lower TimeMark is the season best.
    /// </summary>
    [Theory]
    [InlineData(12000, 11500, 11500)] // faster second result wins
    [InlineData(11500, 12000, 11500)] // faster first result wins
    [InlineData(11500, 11500, 11500)] // tie — either is fine, assert equal
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

    /// <summary>
    /// Scenario: Best distance wins for a distance discipline.
    /// Higher DistanceMark is the season best.
    /// </summary>
    [Theory]
    [InlineData(540, 580, 580)] // longer second result wins
    [InlineData(580, 540, 580)] // longer first result wins
    [InlineData(540, 540, 540)] // tie — either is fine
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

    /// <summary>
    /// Scenario: Non-finished results are excluded.
    /// DNF, DQ, DNS, and NoMark results must not contribute to season best.
    /// </summary>
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

    /// <summary>
    /// Scenario: Place-only disciplines have no best.
    /// SeasonBest returns null for place-only disciplines.
    /// </summary>
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

    /// <summary>
    /// Relay marks are excluded from season best.
    /// </summary>
    [Fact]
    public void SeasonBest_RelayResults_Excluded()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        // Record a relay result — shared time
        meet.RecordResult(student.Id, ERelay, ResultStatus.Finished, new TimeMark(50000), Place1, null);

        var service = new BestMarksService();

        // Act
        var best = service.SeasonBest(student, Relay4x100, Season2025, meet.Results);

        // Assert — relays excluded; no best
        Assert.Null(best);
    }

    // ── Personal best ─────────────────────────────────────────────────────────

    /// <summary>
    /// Scenario: Personal best spans seasons.
    /// The best mark across all seasons is selected.
    /// </summary>
    [Fact]
    public void PersonalBest_SpansSeasons_SelectsOverallBest()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2024, Season2025);

        var meet2024 = CreateMeet(DateInSeason2024);
        var meet2025 = CreateMeet(DateInSeason2025);

        // 2024-25 season has slower time; 2025-26 is faster
        meet2024.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12000), Place2, null);
        meet2025.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(11500), Place1, null);

        var allResults = meet2024.Results.Concat(meet2025.Results);
        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, Run100, allResults);

        // Assert — best across both seasons is 11500ms
        Assert.NotNull(pb);
        var timeMark = Assert.IsType<TimeMark>(pb);
        Assert.Equal(11500, timeMark.Milliseconds);
    }

    /// <summary>
    /// Scenario: Personal best updates as results are added.
    /// A new finished result better than the existing PB is reflected immediately.
    /// The personal best is never stored as a flag; it's recomputed.
    /// </summary>
    [Fact]
    public void PersonalBest_NewBetterResult_UpdatesImmediately()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(12000), Place2, null);

        var service = new BestMarksService();
        var pbBefore = service.PersonalBest(student, Run100, meet.Results);

        // Act — record a new, better result
        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(11500), Place1, null);
        var pbAfter = service.PersonalBest(student, Run100, meet.Results);

        // Assert
        var before = Assert.IsType<TimeMark>(pbBefore);
        var after = Assert.IsType<TimeMark>(pbAfter);
        Assert.Equal(12000, before.Milliseconds);
        Assert.Equal(11500, after.Milliseconds);
    }

    /// <summary>
    /// Scenario: A JV mark can be a personal best.
    /// Tier does not affect PB eligibility; a JV result is included.
    /// </summary>
    [Fact]
    public void PersonalBest_JvMark_IsEligible()
    {
        // Arrange
        var student = CreateStudentWithEnrollments(Season2025);
        var meet = CreateMeet(DateInSeason2025);

        // Only a JV result for this discipline
        meet.RecordResult(student.Id, E100, ResultStatus.Finished, new TimeMark(11800), Place1, Tier.JV);

        var service = new BestMarksService();

        // Act
        var pb = service.PersonalBest(student, Run100, meet.Results);

        // Assert — JV result counts as PB
        Assert.NotNull(pb);
        var timeMark = Assert.IsType<TimeMark>(pb);
        Assert.Equal(11800, timeMark.Milliseconds);
    }

    /// <summary>
    /// Scenario: A JV mark can be a personal best — tier does NOT segment bests.
    /// A JV best mark and a Varsity mark for the same discipline compete;
    /// the fastest wins regardless of tier.
    /// </summary>
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

        // Assert — JV 11700ms beats Varsity 12000ms
        Assert.NotNull(pb);
        var timeMark = Assert.IsType<TimeMark>(pb);
        Assert.Equal(11700, timeMark.Milliseconds);
    }

    /// <summary>
    /// Scenario: Relay marks are excluded from personal best.
    /// </summary>
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

        // Assert — relay excluded; no PB
        Assert.Null(pb);
    }

    /// <summary>
    /// Scenario: Place-only disciplines have no personal best.
    /// </summary>
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

    /// <summary>
    /// Personal best across two seasons with distance discipline — highest distance wins.
    /// </summary>
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
