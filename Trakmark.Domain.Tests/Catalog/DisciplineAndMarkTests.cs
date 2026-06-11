using Trakmark.Domain.Catalog;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>
/// Tests for section 2: discipline catalog, mark kinds, performance marks, and events.
/// Covers spec scenarios in personal-and-season-best and record-meet-results.
/// </summary>
public sealed class DisciplineAndMarkTests
{
    // ── MarkKind implies comparison direction ─────────────────────────────

    [Theory]
    [InlineData(nameof(MarkKind.Time),      ComparisonDirection.LowerIsBetter)]
    [InlineData(nameof(MarkKind.Distance),  ComparisonDirection.HigherIsBetter)]
    [InlineData(nameof(MarkKind.PlaceOnly), ComparisonDirection.None)]
    public void MarkKind_Direction_MatchesKind(string kindName, ComparisonDirection expected)
    {
        // Arrange
        var kind = kindName switch
        {
            nameof(MarkKind.Time)      => MarkKind.Time,
            nameof(MarkKind.Distance)  => MarkKind.Distance,
            nameof(MarkKind.PlaceOnly) => MarkKind.PlaceOnly,
            _                          => throw new ArgumentOutOfRangeException(nameof(kindName))
        };

        // Act / Assert
        Assert.Equal(expected, kind.Direction);
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

    [Theory]
    [InlineData(true)]   // RelayRun → IsRelay = true
    [InlineData(false)]  // Run     → IsRelay = false
    public void Discipline_IsRelay_MatchesDisciplineFactory(bool expectRelay)
    {
        // Arrange
        var discipline = expectRelay ? Discipline.RelayRun(400) : Discipline.Run(100);

        // Act / Assert
        Assert.Equal(expectRelay, discipline.IsRelay);
    }

    // ── Event exposes relay-ness from its discipline ──────────────────────

    [Theory]
    [InlineData(true)]   // relay discipline → Event.IsRelay = true
    [InlineData(false)]  // plain discipline → Event.IsRelay = false
    public void Event_IsRelay_ReflectsDisciplineRelayness(bool expectRelay)
    {
        // Arrange
        var discipline = expectRelay ? Discipline.RelayRun(400) : Discipline.Run(100);
        var ev = new Event(discipline, Sport.TrackAndField);

        // Act / Assert
        Assert.Equal(expectRelay, ev.IsRelay);
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

    // ── Discipline equality ───────────────────────────────────────────────

    [Theory]
    [InlineData("same_key",    true)]   // same identity key → equal
    [InlineData("diff_key",    false)]  // different factory method → not equal
    public void Discipline_Equality_ByIdentityKey(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = Discipline.Run(100);
        var b = scenario == "same_key" ? Discipline.Run(100) : Discipline.Run(200);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void Discipline_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var d = Discipline.Run(100);

        // Act / Assert
        Assert.False(d.Equals((Discipline?)null));
        Assert.False(d == null);
        Assert.True(d != null);
    }

    [Fact]
    public void Discipline_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var d = Discipline.Run(100);

        // Act / Assert
        Assert.False(d.Equals((object)"not a discipline"));
    }

    [Fact]
    public void Discipline_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = Discipline.Run(100);
        var b = Discipline.Run(100);

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Discipline_ToString_ReturnsName()
    {
        // Arrange
        var d = Discipline.Run(400);

        // Act / Assert
        Assert.Equal("400m", d.ToString());
    }

    // ── Event equality ────────────────────────────────────────────────────

    [Theory]
    [InlineData("equal",              true)]   // same discipline + sport → equal
    [InlineData("diff_sport",         false)]  // same discipline, different sport → not equal
    [InlineData("diff_discipline",    false)]  // different discipline, same sport → not equal
    public void Event_Equality_CoversBothFields(string scenario, bool expectedEqual)
    {
        // Arrange
        var disc100 = Discipline.Run(100);
        var disc200 = Discipline.Run(200);

        Event a = new(disc100, Sport.TrackAndField);
        Event b = scenario switch
        {
            "equal"           => new(disc100, Sport.TrackAndField),
            "diff_sport"      => new(disc100, Sport.CrossCountry),
            "diff_discipline" => new(disc200, Sport.TrackAndField),
            _                 => throw new ArgumentOutOfRangeException(nameof(scenario))
        };

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void Event_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var ev = new Event(Discipline.Run(100), Sport.TrackAndField);

        // Act / Assert
        Assert.False(ev.Equals((Event?)null));
        Assert.False(ev == null);
        Assert.True(ev != null);
    }

    [Fact]
    public void Event_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var ev = new Event(Discipline.Run(100), Sport.TrackAndField);

        // Act / Assert
        Assert.False(ev.Equals((object)"not an event"));
    }

    // ── Tier equality ─────────────────────────────────────────────────────

    [Theory]
    [InlineData("same",  true)]   // same singleton → equal
    [InlineData("diff",  false)]  // different singleton → not equal
    public void Tier_Equality_BySingleton(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = Tier.Varsity;
        var b = scenario == "same" ? Tier.Varsity : Tier.JV;

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void Tier_Equals_Null_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(Tier.Varsity.Equals((Tier?)null));
        Assert.False(Tier.Varsity == null);
        Assert.True(Tier.Varsity != null);
    }

    [Fact]
    public void Tier_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(Tier.Varsity.Equals((object)"Varsity"));
    }

    [Fact]
    public void Tier_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("Varsity", Tier.Varsity.ToString());
        Assert.Equal("JV", Tier.JV.ToString());
        Assert.Equal("Open", Tier.Open.ToString());
    }

    // ── HurdleHeight equality ─────────────────────────────────────────────

    [Theory]
    [InlineData("same",  true)]   // same singleton → equal
    [InlineData("diff",  false)]  // different singleton → not equal
    public void HurdleHeight_Equality_BySingleton(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = HurdleHeight.Inches39;
        var b = scenario == "same" ? HurdleHeight.Inches39 : HurdleHeight.Inches33;

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void HurdleHeight_Equals_Null_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(HurdleHeight.Inches39.Equals((HurdleHeight?)null));
        Assert.False(HurdleHeight.Inches39 == null);
        Assert.True(HurdleHeight.Inches39 != null);
    }

    [Fact]
    public void HurdleHeight_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("39\"", HurdleHeight.Inches39.ToString());
        Assert.Equal("33\"", HurdleHeight.Inches33.ToString());
    }

    // ── ImplementWeight equality ──────────────────────────────────────────

    [Theory]
    [InlineData("same",  true)]   // same singleton → equal
    [InlineData("diff",  false)]  // different singleton → not equal
    public void ImplementWeight_Equality_BySingleton(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = ImplementWeight.Kg4;
        var b = scenario == "same" ? ImplementWeight.Kg4 : ImplementWeight.Kg6;

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void ImplementWeight_Equals_Null_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(ImplementWeight.Kg4.Equals((ImplementWeight?)null));
        Assert.False(ImplementWeight.Kg4 == null);
        Assert.True(ImplementWeight.Kg4 != null);
    }

    [Fact]
    public void ImplementWeight_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("4 kg", ImplementWeight.Kg4.ToString());
        Assert.Equal("6 kg", ImplementWeight.Kg6.ToString());
    }

    // ── Placement equality ────────────────────────────────────────────────

    [Theory]
    [InlineData(1, 1, true)]   // same rank → equal
    [InlineData(1, 2, false)]  // different rank → not equal
    public void Placement_Equality_ByRank(int rankA, int rankB, bool expectedEqual)
    {
        // Arrange
        var a = new Placement(rankA);
        var b = new Placement(rankB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void Placement_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var p = new Placement(1);

        // Act / Assert
        Assert.False(p.Equals((Placement?)null));
        Assert.False(p == null);
        Assert.True(p != null);
    }

    [Fact]
    public void Placement_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var p = new Placement(1);

        // Act / Assert
        Assert.False(p.Equals((object)"1"));
    }

    [Fact]
    public void Placement_ToString_ReturnsRank()
    {
        // Arrange / Act / Assert
        Assert.Equal("1", new Placement(1).ToString());
        Assert.Equal("3", new Placement(3).ToString());
    }

    // ── DistanceMark equality ─────────────────────────────────────────────

    [Theory]
    [InlineData(1000, 1000, true)]   // same value → equal
    [InlineData(1000, 1100, false)]  // different value → not equal
    public void DistanceMark_Equality_ByValue(int cmA, int cmB, bool expectedEqual)
    {
        // Arrange
        var a = new DistanceMark(cmA);
        var b = new DistanceMark(cmB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void DistanceMark_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var d = new DistanceMark(500);

        // Act / Assert
        Assert.False(d.Equals((DistanceMark?)null));
        Assert.False(d == null);
        Assert.True(d != null);
    }

    [Fact]
    public void DistanceMark_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var d = new DistanceMark(500);

        // Act / Assert
        Assert.False(d.Equals((object)"500cm"));
    }

    [Fact]
    public void DistanceMark_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new DistanceMark(750);
        var b = new DistanceMark(750);

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void DistanceMark_ToString_IsNonEmpty()
    {
        // Arrange
        var d = new DistanceMark(600);

        // Act
        var result = d.ToString();

        // Assert
        Assert.NotEmpty(result);
    }

    // ── TimeMark equality ─────────────────────────────────────────────────

    [Theory]
    [InlineData(5000, 5000, true)]   // same value → equal
    [InlineData(5000, 6000, false)]  // different value → not equal
    public void TimeMark_Equality_ByValue(int msA, int msB, bool expectedEqual)
    {
        // Arrange
        var a = new TimeMark(msA);
        var b = new TimeMark(msB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void TimeMark_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var t = new TimeMark(5000);

        // Act / Assert
        Assert.False(t.Equals((TimeMark?)null));
        Assert.False(t == null);
        Assert.True(t != null);
    }

    [Fact]
    public void TimeMark_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var t = new TimeMark(5000);

        // Act / Assert
        Assert.False(t.Equals((object)"5000ms"));
    }

    [Fact]
    public void TimeMark_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new TimeMark(12000);
        var b = new TimeMark(12000);

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void TimeMark_ToString_IsNonEmpty()
    {
        // Arrange
        var t = new TimeMark(11500);

        // Act
        var result = t.ToString();

        // Assert
        Assert.NotEmpty(result);
    }
}
