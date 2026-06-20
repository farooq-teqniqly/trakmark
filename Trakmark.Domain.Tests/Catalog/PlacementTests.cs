using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for <see cref="Placement"/> equality, hashing, and formatting.</summary>
public sealed class PlacementTests
{
    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(1, 2, false)]
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
        Assert.False(p.Equals(null));
        Assert.False(p == null);
        Assert.True(p != null);
        Assert.False(null == p);
        Assert.True(null != p);
    }

    [Fact]
    public void Placement_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var p = new Placement(1);

        // Act / Assert
        Assert.False(p.Equals("1"));
    }

    [Fact]
    public void Placement_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new Placement(1);
        var b = new Placement(1);

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Placement_GetHashCode_UsedInHashSet_DeduplicatesEqualInstances()
    {
        // Arrange
        var set = new HashSet<Placement> { new(1), new(1), new(2) };

        // Act / Assert
        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void Placement_ToString_ReturnsRank()
    {
        // Arrange / Act / Assert
        Assert.Equal("1", new Placement(1).ToString());
        Assert.Equal("3", new Placement(3).ToString());
    }
}
