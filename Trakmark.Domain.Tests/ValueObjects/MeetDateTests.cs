using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="MeetDate"/> wrapping, equality, and formatting.</summary>
public sealed class MeetDateTests
{
    [Fact]
    public void MeetDate_WrapsDateOnly()
    {
        // Arrange
        var date = new DateOnly(2024, 5, 15);

        // Act
        var meetDate = new MeetDate(date);

        // Assert
        Assert.Equal(date, meetDate.Value);
    }

    [Fact]
    public void MeetDate_Equality_WorksCorrectly()
    {
        // Arrange
        var date = new DateOnly(2024, 5, 15);
        var a = new MeetDate(date);
        var b = new MeetDate(date);

        // Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void MeetDate_ToString_ReturnsIsoFormattedDate()
    {
        // Arrange
        var meetDate = new MeetDate(new DateOnly(2025, 4, 10));

        // Act
        var result = meetDate.ToString();

        // Assert
        Assert.Equal("2025-04-10", result);
        Assert.NotEmpty(result);
    }
}
