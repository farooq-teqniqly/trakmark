using Trakmark.Domain.Ids;

namespace Trakmark.Domain.Tests.Ids;

/// <summary>Tests for <see cref="SchoolId"/> parse failure paths and round-trip.</summary>
public sealed class SchoolIdTests
{
    [Theory]
    [InlineData("MEET-7F3K9M")]
    [InlineData("SCH-7F3K9")]
    [InlineData("SCH-7F3K9MM")]
    [InlineData("SCH-7F3K9L")]
    public void SchoolId_TryParse_ReturnsFalseForMalformedInput(string value)
    {
        // Arrange / Act / Assert
        Assert.False(SchoolId.TryParse(value, out _));
    }

    [Theory]
    [InlineData("SCH-7F3K9L")]
    [InlineData("wrong-format")]
    [InlineData("SCH-")]
    public void SchoolId_Parse_ThrowsFormatExceptionForMalformedInput(string value)
    {
        // Arrange / Act / Assert
        Assert.Throws<FormatException>(() => SchoolId.Parse(value));
    }

    [Fact]
    public void SchoolId_RoundTrips()
    {
        // Arrange
        var original = SchoolId.NewId();

        // Act / Assert
        Assert.Equal(original, SchoolId.Parse(original.ToString()));
    }
}
