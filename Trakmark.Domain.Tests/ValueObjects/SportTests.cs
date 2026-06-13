using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="Sport"/> closed-set membership and equality.</summary>
public sealed class SportTests
{
    [Fact]
    public void Sport_ClosedSet_ContainsExpectedValues()
    {
        _ = Sport.TrackAndField;
        _ = Sport.CrossCountry;
    }

    [Theory]
    [InlineData("same", true)]   // same singleton → equal
    [InlineData("diff", false)]  // different singleton → not equal
    public void Sport_Equality_BySingleton(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = Sport.TrackAndField;
        var b = scenario == "same" ? Sport.TrackAndField : Sport.CrossCountry;

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void Sport_Equals_Null_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(Sport.TrackAndField.Equals((Sport?)null));
        Assert.False(Sport.TrackAndField == null);
        Assert.True(Sport.TrackAndField != null);
        Assert.False(null == Sport.TrackAndField);
        Assert.True(null != Sport.TrackAndField);
    }

    [Fact]
    public void Sport_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(Sport.TrackAndField.Equals((object)"Track & Field"));
    }

    [Fact]
    public void Sport_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("Track & Field", Sport.TrackAndField.ToString());
        Assert.Equal("Cross-Country", Sport.CrossCountry.ToString());
    }
}
