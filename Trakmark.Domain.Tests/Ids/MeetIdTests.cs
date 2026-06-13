using Trakmark.Domain.Ids;

namespace Trakmark.Domain.Tests.Ids;

/// <summary>Tests for <see cref="MeetId"/> parse failure paths and round-trip.</summary>
public sealed class MeetIdTests
{
    [Theory]
    [InlineData("SCH-7F3K90")]
    [InlineData("SCH-7F3K9")]
    [InlineData("SCH-7F3K9MM")]
    [InlineData("SCH-7F3K9O")]
    public void MeetId_TryParse_ReturnsFalseForMalformedInput(string value)
    {
        // Arrange / Act / Assert
        Assert.False(MeetId.TryParse(value, out _));
    }

    [Theory]
    [InlineData("MEET-7F3K90")]
    [InlineData("wrong-format")]
    [InlineData("MEET-")]
    public void MeetId_Parse_ThrowsFormatExceptionForMalformedInput(string value)
    {
        // Arrange / Act / Assert
        Assert.Throws<FormatException>(() => MeetId.Parse(value));
    }

    [Fact]
    public void MeetId_RoundTrips()
    {
        // Arrange
        var original = MeetId.NewId();

        // Act / Assert
        Assert.Equal(original, MeetId.Parse(original.ToString()));
    }
}
