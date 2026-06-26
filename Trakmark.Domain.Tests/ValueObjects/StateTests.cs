using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="State"/> closed-set membership and equality.</summary>
public sealed class StateTests
{
    [Fact]
    public void State_ClosedSet_ContainsExpectedValues()
    {
        // Arrange / Act / Assert
        Assert.NotNull(State.California);
        Assert.NotNull(State.Texas);
        Assert.NotNull(State.NewYork);
        Assert.NotNull(State.DistrictOfColumbia);
    }

    [Theory]
    [InlineData("CA", "CA", true)]
    [InlineData("CA", "ca", true)]
    [InlineData("ca", "CA", true)]
    [InlineData("CA", "TX", false)]
    [InlineData("NY", "TX", false)]
    public void State_Equality_ByAbbreviation_CaseInsensitive(
        string firstAbbreviation,
        string secondAbbreviation,
        bool expectedEqual)
    {
        // Arrange
        var first = StateFromAbbreviation(firstAbbreviation);
        var second = StateFromAbbreviation(secondAbbreviation);

        // Act / Assert
        Assert.Equal(expectedEqual, first.Equals(second));
        Assert.Equal(expectedEqual, first == second);
        Assert.Equal(!expectedEqual, first != second);
    }

    [Fact]
    public void State_Equals_Null_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(State.California.Equals(null));
        Assert.False(State.California == null);
        Assert.True(State.California != null);
        Assert.False(null == State.California);
        Assert.True(null != State.California);
    }

    [Fact]
    public void State_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        object wrongType = "CA";
        Assert.False(State.California.Equals(wrongType));
    }

    [Fact]
    public void State_GetHashCode_MatchesForEqualAbbreviations_DifferentCase()
    {
        // Arrange
        var lower = StateFromAbbreviation("ca");
        var upper = StateFromAbbreviation("CA");

        // Act / Assert
        Assert.Equal(lower.GetHashCode(), upper.GetHashCode());
    }

    [Fact]
    public void State_ToString_ReturnsAbbreviation()
    {
        // Arrange / Act / Assert
        Assert.Equal("CA", State.California.ToString());
    }

    private static State StateFromAbbreviation(string abbreviation) =>
        abbreviation.ToUpperInvariant() switch
        {
            "CA" => State.California,
            "TX" => State.Texas,
            "NY" => State.NewYork,
            _ => throw new ArgumentOutOfRangeException(nameof(abbreviation)),
        };
}
