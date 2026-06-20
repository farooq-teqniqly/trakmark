using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for <see cref="ImplementWeight"/> equality and formatting.</summary>
public sealed class ImplementWeightTests
{
    [Theory]
    [InlineData("same", true)]
    [InlineData("diff", false)]
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
        Assert.False(ImplementWeight.Kg4.Equals(null));
        Assert.False(ImplementWeight.Kg4 == null);
        Assert.True(ImplementWeight.Kg4 != null);
        Assert.False(null == ImplementWeight.Kg4);
        Assert.True(null != ImplementWeight.Kg4);
    }

    [Fact]
    public void ImplementWeight_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        object wrongType = "4 kg";
        Assert.False(ImplementWeight.Kg4.Equals(wrongType));
    }

    [Fact]
    public void ImplementWeight_GetHashCode_UsedInHashSet_DeduplicatesEqualInstances()
    {
        // Arrange
        var set = new HashSet<ImplementWeight>
        {
            ImplementWeight.Kg4,
            ImplementWeight.Kg4,
            ImplementWeight.Kg6,
        };

        // Act / Assert
        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void ImplementWeight_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("4 kg", ImplementWeight.Kg4.ToString());
        Assert.Equal("6 kg", ImplementWeight.Kg6.ToString());
    }
}
