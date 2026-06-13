using Trakmark.Domain.Ids;

namespace Trakmark.Domain.Tests.Ids;

/// <summary>Tests for <see cref="TeamId"/> parse failure paths and round-trip.</summary>
public sealed class TeamIdTests
{
    [Theory]
    [InlineData("SCH-7F3K9M")]
    [InlineData("TEAM-7F3K9")]
    [InlineData("TEAM-7F3K9MM")]
    [InlineData("TEAM-7F3K91")]
    public void TeamId_TryParse_ReturnsFalseForMalformedInput(string value)
    {
        // Arrange / Act / Assert
        Assert.False(TeamId.TryParse(value, out _));
    }

    [Theory]
    [InlineData("TEAM-7F3K91")]
    [InlineData("wrong-format")]
    [InlineData("TEAM-")]
    public void TeamId_Parse_ThrowsFormatExceptionForMalformedInput(string value)
    {
        // Arrange / Act / Assert
        Assert.Throws<FormatException>(() => TeamId.Parse(value));
    }

    [Fact]
    public void TeamId_RoundTrips()
    {
        // Arrange
        var original = TeamId.NewId();

        // Act / Assert
        Assert.Equal(original, TeamId.Parse(original.ToString()));
    }
}
