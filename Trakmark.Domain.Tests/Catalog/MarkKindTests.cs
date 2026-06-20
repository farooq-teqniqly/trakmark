using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for <see cref="MarkKind"/> equality and comparison direction.</summary>
public sealed class MarkKindTests
{
    [Theory]
    [InlineData("same", true)] // same singleton → equal
    [InlineData("diff", false)] // different singleton → not equal
    public void MarkKind_Equality_BySingleton(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = MarkKind.Time;
        var b = scenario == "same" ? MarkKind.Time : MarkKind.Distance;

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void MarkKind_Equals_Null_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(MarkKind.Time.Equals(null));
        Assert.False(MarkKind.Time == null);
        Assert.True(MarkKind.Time != null);
        Assert.False(null == MarkKind.Time);
        Assert.True(null != MarkKind.Time);
    }

    [Fact]
    public void MarkKind_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(MarkKind.Time.Equals("Time"));
    }

    [Fact]
    public void MarkKind_GetHashCode_UsedInHashSet_DeduplicatesEqualInstances()
    {
        // Arrange
        var set = new HashSet<MarkKind> { MarkKind.Time, MarkKind.Time, MarkKind.Distance };

        // Act / Assert
        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void MarkKind_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("Time", MarkKind.Time.ToString());
        Assert.Equal("Distance", MarkKind.Distance.ToString());
        Assert.Equal("PlaceOnly", MarkKind.PlaceOnly.ToString());
    }

    [Theory]
    [InlineData(nameof(MarkKind.Time), ComparisonDirection.LowerIsBetter)]
    [InlineData(nameof(MarkKind.Distance), ComparisonDirection.HigherIsBetter)]
    [InlineData(nameof(MarkKind.PlaceOnly), ComparisonDirection.None)]
    public void MarkKind_Direction_MatchesKind(string kindName, ComparisonDirection expected)
    {
        // Arrange
        var kind = kindName switch
        {
            nameof(MarkKind.Time) => MarkKind.Time,
            nameof(MarkKind.Distance) => MarkKind.Distance,
            nameof(MarkKind.PlaceOnly) => MarkKind.PlaceOnly,
            _ => throw new ArgumentOutOfRangeException(nameof(kindName)),
        };

        // Act / Assert
        Assert.Equal(expected, kind.Direction);
    }
}
