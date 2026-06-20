using Trakmark.Domain.Catalog;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for <see cref="Event"/> equality and formatting.</summary>
public sealed class EventTests
{
    [Theory]
    [InlineData("equal", true)]
    [InlineData("diff_sport", false)]
    [InlineData("diff_discipline", false)]
    public void Event_Equality_CoversBothFields(string scenario, bool expectedEqual)
    {
        // Arrange
        var disc100 = Discipline.Run(100);
        var disc200 = Discipline.Run(200);

        Event a = new(disc100, Sport.TrackAndField);
        Event b = scenario switch
        {
            "equal" => new(disc100, Sport.TrackAndField),
            "diff_sport" => new(disc100, Sport.CrossCountry),
            "diff_discipline" => new(disc200, Sport.TrackAndField),
            _ => throw new ArgumentOutOfRangeException(nameof(scenario)),
        };

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void Event_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var ev = new Event(Discipline.Run(100), Sport.TrackAndField);

        // Act / Assert
        Assert.False(ev.Equals(null));
        Assert.False(ev == null);
        Assert.True(ev != null);
        Assert.False(null == ev);
        Assert.True(null != ev);
    }

    [Fact]
    public void Event_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var ev = new Event(Discipline.Run(100), Sport.TrackAndField);

        // Act / Assert
        Assert.False(ev.Equals("not an event"));
    }

    [Fact]
    public void Event_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new Event(Discipline.Run(100), Sport.TrackAndField);
        var b = new Event(Discipline.Run(100), Sport.TrackAndField);

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Event_ToString_IsNonEmpty()
    {
        // Arrange
        var ev = new Event(Discipline.Run(400), Sport.TrackAndField);

        // Act
        var result = ev.ToString();

        // Assert
        Assert.NotEmpty(result);
    }
}
