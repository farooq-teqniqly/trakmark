using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for <see cref="TimeMark"/> comparison, equality, and formatting.</summary>
public sealed class TimeMarkTests
{
    [Fact]
    public void TimeMark_IsBetterThan_ReturnsTrueForLowerTime()
    {
        // Arrange
        var slower = new TimeMark(6000);
        var faster = new TimeMark(5900);

        // Act / Assert
        Assert.True(faster.IsBetterThan(slower));
        Assert.False(slower.IsBetterThan(faster));
    }

    [Theory]
    [InlineData(3500, 3600, true)] // 3500ms < 3600ms: candidate is better (lower wins)
    [InlineData(6100, 6000, false)] // 6100ms > 6000ms: candidate is NOT better
    [InlineData(5000, 5000, false)] // equal — neither is better
    public void TimeMark_IsBetterThan_RespectsMinDirection(
        int candidateMs,
        int currentBestMs,
        bool expectedResult
    )
    {
        // Arrange
        var candidate = new TimeMark(candidateMs);
        var currentBest = new TimeMark(currentBestMs);

        // Act / Assert
        Assert.Equal(expectedResult, candidate.IsBetterThan(currentBest));
    }

    [Theory]
    [InlineData(5000, 5000, true)] // same value → equal
    [InlineData(5000, 6000, false)] // different value → not equal
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
        TimeMark? nullTimeMark = null; // typed null invokes custom operator== null-left branch without triggering xUnit2024

        // Act / Assert
        Assert.False(t.Equals(null));
        Assert.False(t == nullTimeMark);
        Assert.True(t != nullTimeMark);
        Assert.False(nullTimeMark == t);
        Assert.True(nullTimeMark != t);
    }

    [Fact]
    public void TimeMark_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var t = new TimeMark(5000);

        // Act / Assert
        object wrongType = "5000ms";
        Assert.False(t.Equals(wrongType));
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
    public void TimeMark_ToString_FormatsWithMsSuffix()
    {
        // Arrange
        var t = new TimeMark(11500);

        // Act
        var result = t.ToString();

        // Assert
        Assert.Equal("11500ms", result);
    }
}
