using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for <see cref="Discipline"/> identity, relay flag, and equality.</summary>
public sealed class DisciplineTests
{
    [Fact]
    public void Discipline_DifferentHurdleHeights_YieldDistinctDisciplines()
    {
        // Arrange
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
    [InlineData(100, 200)]
    [InlineData(200, 400)]
    public void Discipline_PlainRun_DifferentDistances_YieldDistinctDisciplines(
        int distanceA,
        int distanceB
    )
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

    [Theory]
    [InlineData(true)] // RelayRun → IsRelay = true
    [InlineData(false)] // Run → IsRelay = false
    public void Discipline_IsRelay_MatchesDisciplineFactory(bool expectRelay)
    {
        // Arrange
        var discipline = expectRelay ? Discipline.RelayRun(400) : Discipline.Run(100);

        // Act / Assert
        Assert.Equal(expectRelay, discipline.IsRelay);
    }

    [Theory]
    [InlineData("same_key", true)] // same identity key → equal
    [InlineData("diff_key", false)] // different factory → not equal
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
        Assert.False(d.Equals(null));
        Assert.False(d == null);
        Assert.True(d != null);
        Assert.False(null == d);
        Assert.True(null != d);
    }

    [Fact]
    public void Discipline_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var d = Discipline.Run(100);

        // Act / Assert
        object wrongType = "not a discipline";
        Assert.False(d.Equals(wrongType));
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
}
