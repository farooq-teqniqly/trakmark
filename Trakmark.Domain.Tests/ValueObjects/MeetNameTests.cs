using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="MeetName"/> invariants and equality.</summary>
public sealed class MeetNameTests
{
    [Theory]
    [InlineData("Invitational 2024")]
    [InlineData("  Spring Classic  ")]
    public void MeetName_AcceptsValidName(string input)
    {
        // Arrange / Act
        var name = new MeetName(input);

        // Assert
        Assert.Equal(input.Trim(), name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void MeetName_RejectsEmptyOrWhitespace(string input)
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentException>(() => new MeetName(input));
    }

    [Theory]
    [InlineData("Spring Classic", "Spring Classic", true)] // same value → equal
    [InlineData("Spring Classic", "spring classic", true)] // case-insensitive → equal
    [InlineData("Spring Classic", "Fall Classic", false)] // different value → not equal
    public void MeetName_Equality_CaseInsensitive(string valA, string valB, bool expectedEqual)
    {
        // Arrange
        var a = new MeetName(valA);
        var b = new MeetName(valB);

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void MeetName_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var n = new MeetName("Spring Classic");
        MeetName? nullName = null; // typed null invokes custom operator== null-left branch without triggering xUnit2024

        // Act / Assert
        Assert.False(n.Equals(null));
        Assert.False(n == nullName);
        Assert.True(n != nullName);
        Assert.False(nullName == n);
        Assert.True(nullName != n);
    }

    [Fact]
    public void MeetName_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var n = new MeetName("Spring Classic");

        // Act / Assert
        object wrongType = "Spring Classic";
        Assert.False(n.Equals(wrongType));
    }

    [Fact]
    public void MeetName_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new MeetName("Spring Classic");
        var b = new MeetName("spring classic");

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void MeetName_ToString_ReturnsValue()
    {
        // Arrange / Act / Assert
        Assert.Equal("Spring Classic", new MeetName("Spring Classic").ToString());
    }
}
