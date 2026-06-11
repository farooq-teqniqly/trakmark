using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Aggregates;

/// <summary>
/// Tests for <see cref="Enrollment"/> equality: all fields, field-by-field variation,
/// null, wrong type, operators, and consistent hash codes.
/// </summary>
public sealed class EnrollmentTests
{
    private static readonly SchoolId SchoolA = SchoolId.NewId();
    private static readonly SchoolId SchoolB = SchoolId.NewId();

    // ── Enrollment equality ───────────────────────────────────────────────

    [Theory]
    [InlineData("equal",          true)]   // same all-fields → equal
    [InlineData("diff_school",    false)]  // differ by SchoolId → not equal
    [InlineData("diff_year",      false)]  // differ by SchoolYear → not equal
    [InlineData("diff_grade",     false)]  // differ by GradeLevel → not equal
    public void Enrollment_Equality_CoversAllFields(string scenario, bool expectedEqual)
    {
        // Arrange
        var a = new Enrollment(SchoolA, new SchoolYear(2024), GradeLevel.Freshman);

        var b = scenario switch
        {
            "equal"       => new Enrollment(SchoolA, new SchoolYear(2024), GradeLevel.Freshman),
            "diff_school" => new Enrollment(SchoolB, new SchoolYear(2024), GradeLevel.Freshman),
            "diff_year"   => new Enrollment(SchoolA, new SchoolYear(2023), GradeLevel.Freshman),
            "diff_grade"  => new Enrollment(SchoolA, new SchoolYear(2024), GradeLevel.Senior),
            _             => throw new ArgumentOutOfRangeException(nameof(scenario))
        };

        // Act / Assert
        Assert.Equal(expectedEqual, a.Equals(b));
        Assert.Equal(expectedEqual, a == b);
        Assert.Equal(!expectedEqual, a != b);
    }

    [Fact]
    public void Enrollment_Equals_Null_ReturnsFalse()
    {
        // Arrange
        var e = new Enrollment(SchoolA, new SchoolYear(2024), GradeLevel.Freshman);

        // Act / Assert
        Assert.False(e.Equals((Enrollment?)null));
        Assert.False(e == null);
        Assert.True(e != null);
    }

    [Fact]
    public void Enrollment_Equals_WrongType_ReturnsFalse()
    {
        // Arrange
        var e = new Enrollment(SchoolA, new SchoolYear(2024), GradeLevel.Freshman);

        // Act / Assert
        Assert.False(e.Equals((object)"not an enrollment"));
    }

    [Fact]
    public void Enrollment_GetHashCode_EqualInstances_SameHash()
    {
        // Arrange
        var a = new Enrollment(SchoolA, new SchoolYear(2024), GradeLevel.Freshman);
        var b = new Enrollment(SchoolA, new SchoolYear(2024), GradeLevel.Freshman);

        // Act / Assert
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}
