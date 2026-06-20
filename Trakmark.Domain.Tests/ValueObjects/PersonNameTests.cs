using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="PersonName"/> invariants and equality.</summary>
public sealed class PersonNameTests
{
    [Theory]
    [InlineData("Alice")]
    [InlineData("Bob Smith")]
    [InlineData("  Alice  ")] // trimmed to non-empty
    public void PersonName_AcceptsValidName(string input)
    {
        // Arrange / Act
        var name = new PersonName(input);

        // Assert
        Assert.Equal(input.Trim(), name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void PersonName_RejectsEmptyOrWhitespace(string input)
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentException>(() => new PersonName(input));
    }

    [Theory]
    [InlineData("Alice", "Alice", true)] // same value → equal
    [InlineData("Alice", "alice", true)] // case-insensitive → equal
    [InlineData("Alice", "Bob", false)] // different value → not equal
    public void PersonName_Equality_CaseInsensitive(string valA, string valB, bool expectedEqual)
    {
        // Arrange
        var a = new PersonName(valA);
        var b = new PersonName(valB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void PersonName_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var n = new PersonName("Alice");

        // Act / Assert
        Assert.False(n.Equals(null));
        Assert.False(n == null);
        Assert.True(n != null);
        Assert.False(null == n);
        Assert.True(null != n);
    }

    [Fact]
    public void PersonName_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var n = new PersonName("Alice");

        // Act / Assert
        object wrongType = "Alice";
        Assert.False(n.Equals(wrongType));
    }

    [Fact]
    public void PersonName_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new PersonName("Alice");
        var b = new PersonName("ALICE");

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void PersonName_ToString_ReturnsValue()
    {
        // Arrange / Act / Assert
        Assert.Equal("Alice", new PersonName("Alice").ToString());
    }
}
