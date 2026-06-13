using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for <see cref="Tier"/> equality and formatting.</summary>
public sealed class TierTests
{
    [Theory]
    [InlineData("same", true)]
    [InlineData("diff", false)]
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
        Assert.False(null == Tier.Varsity);
        Assert.True(null != Tier.Varsity);
    }

    [Fact]
    public void Tier_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(Tier.Varsity.Equals((object)"Varsity"));
    }

    [Fact]
    public void Tier_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange / Act / Assert
        Assert.Equal(Tier.Varsity.GetHashCode(), Tier.Varsity.GetHashCode());
    }

    [Fact]
    public void Tier_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("Varsity", Tier.Varsity.ToString());
        Assert.Equal("JV", Tier.JV.ToString());
        Assert.Equal("Open", Tier.Open.ToString());
    }
}
