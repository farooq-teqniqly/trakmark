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
        // Arrange
        Tier? nullTier = null; // typed null invokes custom operator== null-left branch without triggering xUnit2024

        // Act / Assert
        Assert.False(Tier.Varsity.Equals(null));
        Assert.False(Tier.Varsity == nullTier);
        Assert.True(Tier.Varsity != nullTier);
        Assert.False(nullTier == Tier.Varsity);
        Assert.True(nullTier != Tier.Varsity);
    }

    [Fact]
    public void Tier_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        object wrongType = "Varsity";
        Assert.False(Tier.Varsity.Equals(wrongType));
    }

    [Fact]
    public void Tier_GetHashCode_UsedInHashSet_DeduplicatesEqualInstances()
    {
        // Arrange
        var set = new HashSet<Tier> { Tier.Varsity, Tier.Varsity, Tier.JV };

        // Act / Assert
        Assert.Equal(2, set.Count);
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
