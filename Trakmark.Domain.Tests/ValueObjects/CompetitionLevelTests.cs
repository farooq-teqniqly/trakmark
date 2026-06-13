using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="CompetitionLevel"/> closed-set membership and equality.</summary>
public sealed class CompetitionLevelTests
{
    [Fact]
    public void CompetitionLevel_ClosedSet_ContainsExpectedValues()
    {
        _ = CompetitionLevel.HighSchool;
        _ = CompetitionLevel.MiddleSchool;
        _ = CompetitionLevel.Elementary;
    }

    [Theory]
    [InlineData("same", true)]   // same singleton → equal
    [InlineData("diff", false)]  // different singleton → not equal
    public void CompetitionLevel_Equality_BySingleton(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = CompetitionLevel.HighSchool;
        var b = scenario == "same" ? CompetitionLevel.HighSchool : CompetitionLevel.MiddleSchool;

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void CompetitionLevel_Equals_Null_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(CompetitionLevel.HighSchool.Equals((CompetitionLevel?)null));
        Assert.False(CompetitionLevel.HighSchool == null);
        Assert.True(CompetitionLevel.HighSchool != null);
        Assert.False(null == CompetitionLevel.HighSchool);
        Assert.True(null != CompetitionLevel.HighSchool);
    }

    [Fact]
    public void CompetitionLevel_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        Assert.False(CompetitionLevel.HighSchool.Equals((object)"High School"));
    }

    [Fact]
    public void CompetitionLevel_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange / Act / Assert
        Assert.Equal(
            CompetitionLevel.HighSchool.GetHashCode(),
            CompetitionLevel.HighSchool.GetHashCode());
    }

    [Fact]
    public void CompetitionLevel_GetHashCode_UsedInHashSet_DeduplicatesEqualInstances()
    {
        // Arrange
        var set = new HashSet<CompetitionLevel>
        {
            CompetitionLevel.HighSchool,
            CompetitionLevel.HighSchool,
            CompetitionLevel.MiddleSchool
        };

        // Act / Assert
        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void CompetitionLevel_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("High School", CompetitionLevel.HighSchool.ToString());
        Assert.Equal("Middle School", CompetitionLevel.MiddleSchool.ToString());
    }
}
