using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="TeamName"/> invariants and equality.</summary>
public sealed class TeamNameTests
{
    [Theory]
    [InlineData("Varsity Boys")]
    [InlineData("  JV Girls  ")]
    public void TeamName_AcceptsValidName(string input)
    {
        // Arrange / Act
        var name = new TeamName(input);

        // Assert
        Assert.Equal(input.Trim(), name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void TeamName_RejectsEmptyOrWhitespace(string input)
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentException>(() => new TeamName(input));
    }

    [Theory]
    [InlineData("Varsity Boys", "Varsity Boys", true)] // same value → equal
    [InlineData("Varsity Boys", "varsity boys", true)] // case-insensitive → equal
    [InlineData("Varsity Boys", "JV Girls", false)] // different value → not equal
    public void TeamName_Equality_CaseInsensitive(string valA, string valB, bool expectedEqual)
    {
        // Arrange
        var a = new TeamName(valA);
        var b = new TeamName(valB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void TeamName_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var n = new TeamName("Varsity Boys");

        // Act / Assert
        Assert.False(n.Equals(null));
        Assert.False(n == null);
        Assert.True(n != null);
        Assert.False(null == n);
        Assert.True(null != n);
    }

    [Fact]
    public void TeamName_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var n = new TeamName("Varsity Boys");

        // Act / Assert
        Assert.False(n.Equals("Varsity Boys"));
    }

    [Fact]
    public void TeamName_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new TeamName("Varsity Boys");
        var b = new TeamName("VARSITY BOYS");

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void TeamName_ToString_ReturnsValue()
    {
        // Arrange / Act / Assert
        Assert.Equal("Varsity Boys", new TeamName("Varsity Boys").ToString());
    }
}
