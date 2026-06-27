using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.ValueObjects;

/// <summary>Tests for <see cref="GradeLevel"/> closed-set membership and equality.</summary>
public sealed class GradeLevelTests
{
    [Fact]
    public void GradeLevel_ClosedSet_ContainsExpectedValues()
    {
        // Arrange / Act / Assert — smoke-test the closed set
        Assert.NotNull(GradeLevel.Freshman);
        Assert.NotNull(GradeLevel.Sophomore);
        Assert.NotNull(GradeLevel.Junior);
        Assert.NotNull(GradeLevel.Senior);
        Assert.NotNull(GradeLevel.MiddleSchool8th);
    }

    [Theory]
    [InlineData("same", true)] // same singleton → equal
    [InlineData("diff", false)] // different singleton → not equal
    public void GradeLevel_Equality_BySingleton(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = GradeLevel.Freshman;
        var b = scenario == "same" ? GradeLevel.Freshman : GradeLevel.Senior;

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void GradeLevel_Equals_Null_ReturnsFalse()
    {
        // Arrange
        GradeLevel? nullLevel = null; // typed null invokes custom operator== null-left branch without triggering xUnit2024

        // Act / Assert
        Assert.False(GradeLevel.Freshman.Equals(null));
        Assert.False(GradeLevel.Freshman == nullLevel);
        Assert.True(GradeLevel.Freshman != nullLevel);
        Assert.False(nullLevel == GradeLevel.Freshman);
        Assert.True(nullLevel != GradeLevel.Freshman);
    }

    [Fact]
    public void GradeLevel_Equals_WrongType_ReturnsFalse()
    {
        // Arrange / Act / Assert
        object wrongType = "Freshman";
        Assert.False(GradeLevel.Freshman.Equals(wrongType));
    }

    [Fact]
    public void GradeLevel_GetHashCode_UsedInHashSet_DeduplicatesEqualInstances()
    {
        // Arrange
        var set = new HashSet<GradeLevel>
        {
            GradeLevel.Freshman,
            GradeLevel.Freshman,
            GradeLevel.Senior,
        };

        // Act / Assert
        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void GradeLevel_ToString_ReturnsName()
    {
        // Arrange / Act / Assert
        Assert.Equal("Freshman", GradeLevel.Freshman.ToString());
        Assert.Equal("Senior", GradeLevel.Senior.ToString());
    }
}
