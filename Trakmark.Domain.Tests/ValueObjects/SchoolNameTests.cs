using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="SchoolName"/> invariants and equality.</summary>
public sealed class SchoolNameTests
{
    [Theory]
    [InlineData("Springfield High")]
    [InlineData("  Central Middle School  ")]
    public void SchoolName_AcceptsValidName(string input)
    {
        // Arrange / Act
        var name = new SchoolName(input);

        // Assert
        Assert.Equal(input.Trim(), name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SchoolName_RejectsEmptyOrWhitespace(string input)
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentException>(() => new SchoolName(input));
    }

    [Theory]
    [InlineData("Springfield High", "Springfield High", true)] // same value → equal
    [InlineData("Springfield High", "springfield high", true)] // case-insensitive → equal
    [InlineData("Springfield High", "Central High", false)] // different value → not equal
    public void SchoolName_Equality_CaseInsensitive(string valA, string valB, bool expectedEqual)
    {
        // Arrange
        var a = new SchoolName(valA);
        var b = new SchoolName(valB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void SchoolName_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var n = new SchoolName("Springfield High");
        SchoolName? nullName = null; // typed null invokes custom operator== null-left branch without triggering xUnit2024

        // Act / Assert
        Assert.False(n.Equals(null));
        Assert.False(n == nullName);
        Assert.True(n != nullName);
        Assert.False(nullName == n);
        Assert.True(nullName != n);
    }

    [Fact]
    public void SchoolName_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var n = new SchoolName("Springfield High");

        // Act / Assert
        object wrongType = "Springfield High";
        Assert.False(n.Equals(wrongType));
    }

    [Fact]
    public void SchoolName_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new SchoolName("Springfield High");
        var b = new SchoolName("SPRINGFIELD HIGH");

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void SchoolName_ToString_ReturnsValue()
    {
        // Arrange / Act / Assert
        Assert.Equal("Springfield High", new SchoolName("Springfield High").ToString());
    }
}
