using Trakmark.Domain.Ids;

namespace Trakmark.Domain.Tests.Ids;

/// <summary>Tests for <see cref="RegisteredUserId"/> parse failure paths and round-trip.</summary>
public sealed class RegisteredUserIdTests
{
    [Theory]
    [InlineData("MEET-7F3K9M")]
    [InlineData("USR-7F3K9")]
    [InlineData("USR-7F3K9MM")]
    [InlineData("USR-7F3K9I")]
    public void RegisteredUserId_TryParse_ReturnsFalseForMalformedInput(string value)
    {
        // Arrange / Act / Assert
        Assert.False(RegisteredUserId.TryParse(value, out _));
    }

    [Theory]
    [InlineData("USR-7F3K9I")]
    [InlineData("wrong-format")]
    [InlineData("USR-")]
    public void RegisteredUserId_Parse_ThrowsFormatExceptionForMalformedInput(string value)
    {
        // Arrange / Act / Assert
        Assert.Throws<FormatException>(() => RegisteredUserId.Parse(value));
    }

    [Fact]
    public void RegisteredUserId_RoundTrips()
    {
        // Arrange
        var original = RegisteredUserId.NewId();

        // Act / Assert
        Assert.Equal(original, RegisteredUserId.Parse(original.ToString()));
    }
}
