using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for cross-type <see cref="Performance.IsBetterThan"/> behaviour.</summary>
public sealed class PerformanceTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Performance_IsBetterThan_WrongType_ReturnsFalse(bool firstIsTime)
    {
        // Arrange
        Performance mark = firstIsTime ? new TimeMark(5000) : new DistanceMark(500);
        Performance wrongType = firstIsTime ? new DistanceMark(500) : new TimeMark(5000);

        // Act
        var result = mark.IsBetterThan(wrongType);

        // Assert
        Assert.False(result);
    }
}
