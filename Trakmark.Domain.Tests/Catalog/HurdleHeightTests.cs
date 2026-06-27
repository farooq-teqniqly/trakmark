using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for <see cref="HurdleHeight"/> equality and formatting.</summary>
public sealed class HurdleHeightTests
{
    [Theory]
    [InlineData("same", true)]
    [InlineData("diff", false)]
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
        // Arrange
        HurdleHeight? nullHeight = null;

        // Act / Assert
        Assert.False(HurdleHeight.Inches39.Equals(null));
        Assert.False(HurdleHeight.Inches39 == nullHeight);
        Assert.True(HurdleHeight.Inches39 != nullHeight);
        Assert.False(nullHeight == HurdleHeight.Inches39);
        Assert.True(nullHeight != HurdleHeight.Inches39);
    }

    [Fact]
    public void HurdleHeight_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        object wrongType = "39\"";
        Assert.False(HurdleHeight.Inches39.Equals(wrongType));
    }

    [Fact]
    public void HurdleHeight_GetHashCode_UsedInHashSet_DeduplicatesEqualInstances()
    {
        // Arrange
        var set = new HashSet<HurdleHeight>
        {
            HurdleHeight.Inches39,
            HurdleHeight.Inches39,
            HurdleHeight.Inches33,
        };

        // Act / Assert
        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void HurdleHeight_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("39\"", HurdleHeight.Inches39.ToString());
        Assert.Equal("33\"", HurdleHeight.Inches33.ToString());
    }
}
