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
        // Arrange / Act / Assert
        Assert.False(HurdleHeight.Inches39.Equals((HurdleHeight?)null));
        Assert.False(HurdleHeight.Inches39 == null);
        Assert.True(HurdleHeight.Inches39 != null);
        Assert.False(null == HurdleHeight.Inches39);
        Assert.True(null != HurdleHeight.Inches39);
    }

    [Fact]
    public void HurdleHeight_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(HurdleHeight.Inches39.Equals((object)"39\""));
    }

    [Fact]
    public void HurdleHeight_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange / Act / Assert
        Assert.Equal(HurdleHeight.Inches39.GetHashCode(), HurdleHeight.Inches39.GetHashCode());
    }

    [Fact]
    public void HurdleHeight_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("39\"", HurdleHeight.Inches39.ToString());
        Assert.Equal("33\"", HurdleHeight.Inches33.ToString());
    }
}
