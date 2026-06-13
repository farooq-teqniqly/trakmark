using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="SchoolYear"/> invariants, ordering, and comparison operators.</summary>
public sealed class SchoolYearTests
{
    [Fact]
    public void SchoolYear_IsOrderable()
    {
        // Arrange
        var earlier = new SchoolYear(2023);
        var later = new SchoolYear(2024);

        // Assert
        Assert.True(earlier < later);
        Assert.True(later > earlier);
        Assert.Equal(earlier, new SchoolYear(2023));
    }

    [Fact]
    public void SchoolYear_RejectsInvalidYear()
    {
        // Arrange / Act / Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SchoolYear(0));
    }

    [Theory]
    [InlineData(2024, 2024, false, true,  false, true)]   // equal:   < false, <= true,  > false, >= true
    [InlineData(2025, 2024, false, false, true,  true)]   // later:   < false, <= false, > true,  >= true
    [InlineData(2024, 2025, true,  true,  false, false)]  // earlier: < true,  <= true,  > false, >= false
    public void SchoolYear_ComparisonOperators_Correct(
        int leftYear, int rightYear,
        bool expectedLt, bool expectedLte,
        bool expectedGt, bool expectedGte)
    {
        // Arrange
        var left  = new SchoolYear(leftYear);
        var right = new SchoolYear(rightYear);

        // Act / Assert
        Assert.Equal(expectedLt,  left < right);
        Assert.Equal(expectedLte, left <= right);
        Assert.Equal(expectedGt,  left > right);
        Assert.Equal(expectedGte, left >= right);
    }
}
