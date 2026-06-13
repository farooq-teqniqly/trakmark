using Trakmark.Domain.Catalog;

namespace Trakmark.Domain.Tests.Catalog;

/// <summary>Tests for cross-type <see cref="Performance.IsBetterThan"/> behaviour.</summary>
public sealed class PerformanceTests
{
    public static TheoryData<Performance, Performance> WrongTypeMarkPairs() =>
        new()
        {
            { new TimeMark(5000), new DistanceMark(500) },
            { new DistanceMark(500), new TimeMark(5000) },
        };

    [Theory]
    [MemberData(nameof(WrongTypeMarkPairs))]
    public void Performance_IsBetterThan_WrongType_ReturnsFalse(Performance mark, Performance wrongType)
    {
        // Arrange / Act
        var result = mark.IsBetterThan(wrongType);

        // Assert
        Assert.False(result);
    }
}
