using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="City"/> creation invariants and equality.</summary>
public sealed class CityTests
{
    [Fact]
    public void Create_ValidNameAndState_CreatesCityWithNewIdNameAndState()
    {
        // Arrange
        const string name = "Springfield";
        var state = State.Illinois;

        // Act
        var city = City.Create(name, state);

        // Assert
        Assert.NotEqual(default, city.Id);
        Assert.Equal(name, city.Name);
        Assert.Equal(state, city.State);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_EmptyOrWhitespaceName_ThrowsArgumentException(string invalidName)
    {
        // Arrange
        var state = State.Illinois;

        // Act / Assert
        Assert.Throws<ArgumentException>(() => City.Create(invalidName, state));
    }

    [Fact]
    public void Create_NameExceedsMaximumLength_ThrowsArgumentException()
    {
        // Arrange
        var tooLongName = new string('a', 101);
        var state = State.Illinois;

        // Act / Assert
        Assert.Throws<ArgumentException>(() => City.Create(tooLongName, state));
    }

    [Fact]
    public void Create_NameAtMaximumLength_CreatesCity()
    {
        // Arrange
        var maxLengthName = new string('a', 100);
        var state = State.Illinois;

        // Act
        var city = City.Create(maxLengthName, state);

        // Assert
        Assert.Equal(maxLengthName, city.Name);
    }

    [Fact]
    public void Create_NameWithSurroundingWhitespace_TrimsName()
    {
        // Arrange
        const string name = "  Springfield  ";
        var state = State.Illinois;

        // Act
        var city = City.Create(name, state);

        // Assert
        Assert.Equal("Springfield", city.Name);
    }

    [Fact]
    public void Create_NullName_ThrowsArgumentNullException()
    {
        // Arrange
        var state = State.Illinois;

        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => City.Create(null!, state));
    }

    [Fact]
    public void Create_NullState_ThrowsArgumentNullException()
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentNullException>(() => City.Create("Springfield", null!));
    }

    [Theory]
    [InlineData("Springfield", "Springfield", "IL", "IL", true)]
    [InlineData("Springfield", "springfield", "IL", "IL", true)]
    [InlineData("Springfield", "SPRINGFIELD", "IL", "IL", true)]
    [InlineData("Springfield", "Springfield", "IL", "MO", false)]
    [InlineData("Springfield", "Chicago", "IL", "IL", false)]
    public void Equals_ComparesByNameCaseInsensitiveAndState(
        string firstName,
        string secondName,
        string firstStateAbbreviation,
        string secondStateAbbreviation,
        bool expectedEqual)
    {
        // Arrange
        var first = City.Create(firstName, StateFromAbbreviation(firstStateAbbreviation));
        var second = City.Create(secondName, StateFromAbbreviation(secondStateAbbreviation));

        // Act / Assert
        Assert.Equal(expectedEqual, first.Equals(second));
        Assert.Equal(expectedEqual, first == second);
        Assert.Equal(!expectedEqual, first != second);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        // Arrange
        var city = City.Create("Springfield", State.Illinois);
        City? nullCity = null; // typed null invokes custom operator== null-left branch without triggering xUnit2024

        // Act / Assert
        Assert.False(city.Equals(null));
        Assert.False(city == nullCity);
        Assert.True(city != nullCity);
        Assert.False(nullCity == city);
        Assert.True(nullCity != city);
    }

    [Fact]
    public void Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var city = City.Create("Springfield", State.Illinois);
        object wrongType = "Springfield";

        // Act / Assert
        Assert.False(city.Equals(wrongType));
    }

    [Fact]
    public void GetHashCode_MatchesForEqualNamesWithDifferentCase()
    {
        // Arrange
        var lower = City.Create("springfield", State.Illinois);
        var upper = City.Create("SPRINGFIELD", State.Illinois);

        // Act / Assert
        Assert.Equal(lower.GetHashCode(), upper.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsNameAndState()
    {
        // Arrange
        var city = City.Create("Springfield", State.Illinois);

        // Act
        var result = city.ToString();

        // Assert
        Assert.Equal("Springfield, IL", result);
    }

    private static State StateFromAbbreviation(string abbreviation) =>
        abbreviation switch
        {
            "IL" => State.Illinois,
            "MO" => State.Missouri,
            _ => throw new ArgumentOutOfRangeException(nameof(abbreviation)),
        };
}
