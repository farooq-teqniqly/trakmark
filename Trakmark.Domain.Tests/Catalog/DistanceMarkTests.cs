using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for <see cref="DistanceMark"/> comparison, equality, and formatting.</summary>
public sealed class DistanceMarkTests
{
    [Fact]
    public void DistanceMark_IsBetterThan_ReturnsTrueForGreaterDistance()
    {
        // Arrange
        var shorter = new DistanceMark(1000);
        var longer = new DistanceMark(1100);

        // Act / Assert
        Assert.True(longer.IsBetterThan(shorter));
        Assert.False(shorter.IsBetterThan(longer));
    }

    [Theory]
    [InlineData(1100, 1000, true)]   // 1100cm > 1000cm: 1100 is better
    [InlineData(900,  1000, false)]  // 900cm < 1000cm: 900 is NOT better
    [InlineData(1200, 1200, false)]  // equal — neither is better
    public void DistanceMark_IsBetterThan_RespectsMaxDirection(int candidateCm, int currentBestCm, bool expectedResult)
    {
        // Arrange
        var candidate = new DistanceMark(candidateCm);
        var currentBest = new DistanceMark(currentBestCm);

        // Act / Assert
        Assert.Equal(expectedResult, candidate.IsBetterThan(currentBest));
    }

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
        Assert.False(null == d);
        Assert.True(null != d);
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
}
