using Trakmark.Domain.Catalog;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests;

/// <summary>
/// Tests for section 2: discipline catalog, mark kinds, performance marks, and events.
/// Covers spec scenarios in personal-and-season-best and record-meet-results.
/// </summary>
public sealed class DisciplineAndMarkTests
{
    // ── MarkKind implies comparison direction ─────────────────────────────

    [Fact]
    public void MarkKind_Time_HasLowerIsBetterDirection()
    {
        // Arrange / Act
        var kind = MarkKind.Time;

        // Assert
        Assert.Equal(ComparisonDirection.LowerIsBetter, kind.Direction);
    }

    [Fact]
    public void MarkKind_Distance_HasHigherIsBetterDirection()
    {
        // Arrange / Act
        var kind = MarkKind.Distance;

        // Assert
        Assert.Equal(ComparisonDirection.HigherIsBetter, kind.Direction);
    }

    [Fact]
    public void MarkKind_PlaceOnly_HasNoneDirection()
    {
        // Arrange / Act
        var kind = MarkKind.PlaceOnly;

        // Assert — place-only disciplines have no measurable direction
        Assert.Equal(ComparisonDirection.None, kind.Direction);
    }

    // ── TimeMark comparison — min wins ────────────────────────────────────

    [Fact]
    public void TimeMark_IsBetterThan_ReturnsTrueForLowerTime()
    {
        // Arrange — scenario: Best time wins for a time discipline
        var slower = new TimeMark(6000);
        var faster = new TimeMark(5900);

        // Act / Assert
        Assert.True(faster.IsBetterThan(slower));
        Assert.False(slower.IsBetterThan(faster));
    }

    [Theory]
    [InlineData(3500, 3600, true)]   // 3500ms < 3600ms: candidate is better (lower wins)
    [InlineData(6100, 6000, false)]  // 6100ms > 6000ms: candidate is NOT better
    [InlineData(5000, 5000, false)]  // equal — neither is better
    public void TimeMark_IsBetterThan_RespectsMinDirection(int candidateMs, int currentBestMs, bool expectedResult)
    {
        // Arrange
        var candidate = new TimeMark(candidateMs);
        var currentBest = new TimeMark(currentBestMs);

        // Act / Assert
        Assert.Equal(expectedResult, candidate.IsBetterThan(currentBest));
    }

    // ── DistanceMark comparison — max wins ────────────────────────────────

    [Fact]
    public void DistanceMark_IsBetterThan_ReturnsTrueForGreaterDistance()
    {
        // Arrange — scenario: Best distance wins for a distance discipline
        var shorter = new DistanceMark(1000);
        var longer = new DistanceMark(1100);

        // Act / Assert
        Assert.True(longer.IsBetterThan(shorter));
        Assert.False(shorter.IsBetterThan(longer));
    }

    [Theory]
    [InlineData(1100, 1000, true)]   // 1100cm > 1000cm: 1100 is better
    [InlineData(900, 1000, false)]   // 900cm < 1000cm: 900 is NOT better
    [InlineData(1200, 1200, false)]  // equal — neither is better
    public void DistanceMark_IsBetterThan_RespectsMaxDirection(int candidateCm, int currentBestCm, bool expectedResult)
    {
        // Arrange
        var candidate = new DistanceMark(candidateCm);
        var currentBest = new DistanceMark(currentBestCm);

        // Act / Assert
        Assert.Equal(expectedResult, candidate.IsBetterThan(currentBest));
    }

    // ── Discipline identity includes setup parameters ─────────────────────

    [Fact]
    public void Discipline_DifferentHurdleHeights_YieldDistinctDisciplines()
    {
        // Arrange — scenario: distinct setup parameters yield distinct disciplines
        var hurdles39 = Discipline.HurdleRun(100, HurdleHeight.Inches39);
        var hurdles33 = Discipline.HurdleRun(100, HurdleHeight.Inches33);

        // Act / Assert
        Assert.NotEqual(hurdles39, hurdles33);
    }

    [Fact]
    public void Discipline_SameHurdleParameters_YieldEqualDisciplines()
    {
        // Arrange
        var a = Discipline.HurdleRun(110, HurdleHeight.Inches39);
        var b = Discipline.HurdleRun(110, HurdleHeight.Inches39);

        // Act / Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void Discipline_DifferentImplementWeights_YieldDistinctDisciplines()
    {
        // Arrange
        var shot4kg = Discipline.ImplementThrow("Shot Put", ImplementWeight.Kg4);
        var shot6kg = Discipline.ImplementThrow("Shot Put", ImplementWeight.Kg6);

        // Act / Assert
        Assert.NotEqual(shot4kg, shot6kg);
    }

    [Fact]
    public void Discipline_SameImplementWeight_YieldEqualDisciplines()
    {
        // Arrange
        var a = Discipline.ImplementThrow("Shot Put", ImplementWeight.Kg4);
        var b = Discipline.ImplementThrow("Shot Put", ImplementWeight.Kg4);

        // Act / Assert
        Assert.Equal(a, b);
    }

    [Theory]
    [InlineData(100, 200)]  // different distances are distinct
    [InlineData(200, 400)]
    public void Discipline_PlainRun_DifferentDistances_YieldDistinctDisciplines(int distanceA, int distanceB)
    {
        // Arrange
        var a = Discipline.Run(distanceA);
        var b = Discipline.Run(distanceB);

        // Act / Assert
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Discipline_PlainRun_SameDistance_YieldsEqualDisciplines()
    {
        // Arrange
        var a = Discipline.Run(100);
        var b = Discipline.Run(100);

        // Act / Assert
        Assert.Equal(a, b);
    }

    // ── Discipline relay-ness ─────────────────────────────────────────────

    [Fact]
    public void Discipline_RelayRun_IsRelay()
    {
        // Arrange
        var relay = Discipline.RelayRun(400);

        // Act / Assert
        Assert.True(relay.IsRelay);
    }

    [Fact]
    public void Discipline_PlainRun_IsNotRelay()
    {
        // Arrange
        var run = Discipline.Run(100);

        // Act / Assert
        Assert.False(run.IsRelay);
    }

    // ── Event exposes relay-ness from its discipline ──────────────────────

    [Fact]
    public void Event_RelayDiscipline_IsRelay()
    {
        // Arrange — scenario: Event exposing relay-ness from its discipline
        var discipline = Discipline.RelayRun(400);
        var ev = new Event(discipline, Sport.TrackAndField);

        // Act / Assert
        Assert.True(ev.IsRelay);
    }

    [Fact]
    public void Event_IndividualDiscipline_IsNotRelay()
    {
        // Arrange
        var discipline = Discipline.Run(100);
        var ev = new Event(discipline, Sport.TrackAndField);

        // Act / Assert
        Assert.False(ev.IsRelay);
    }

    [Fact]
    public void Event_ExposesItsDisciplineAndSport()
    {
        // Arrange
        var discipline = Discipline.Run(400);
        var ev = new Event(discipline, Sport.CrossCountry);

        // Act / Assert
        Assert.Equal(discipline, ev.Discipline);
        Assert.Equal(Sport.CrossCountry, ev.Sport);
    }
}
