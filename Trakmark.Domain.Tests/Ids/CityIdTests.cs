using Trakmark.Domain.Ids;

namespace Trakmark.Domain.Tests.Ids;

/// <summary>Tests for <see cref="CityId"/> parse failure paths and round-trip.</summary>
public sealed class CityIdTests
{
    [Theory]
    [InlineData("SCH-7F3K90")]
    [InlineData("CTY-7F3K9")]
    [InlineData("CTY-7F3K9MM")]
    [InlineData("CTY-7F3K9O")]
    public void CityId_TryParse_ReturnsFalseForMalformedInput(string value)
    {
        // Arrange / Act / Assert
        Assert.False(CityId.TryParse(value, out _));
    }

    [Theory]
    [InlineData("CTY-7F3K90")]
    [InlineData("wrong-format")]
    [InlineData("CTY-")]
    public void CityId_Parse_ThrowsFormatExceptionForMalformedInput(string value)
    {
        // Arrange / Act / Assert
        Assert.Throws<FormatException>(() => CityId.Parse(value));
    }

    [Fact]
    public void CityId_RoundTrips()
    {
        // Arrange
        var original = CityId.NewId();

        // Act / Assert
        Assert.Equal(original, CityId.Parse(original.ToString()));
    }

    [Fact]
    public void CityId_NewId_GeneratesUniqueValues()
    {
        // Arrange / Act
        var first = CityId.NewId();
        var second = CityId.NewId();

        // Assert
        Assert.NotEqual(first, second);
    }
}
