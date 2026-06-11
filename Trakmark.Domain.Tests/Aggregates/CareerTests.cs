using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Aggregates;

/// <summary>
/// Tests for Career aggregate component: enrollment history management.
/// </summary>
public sealed class CareerTests
{
    // ── Adding enrollment for a new year ─────────────────────────────────

    [Fact]
    public void TryAddEnrollment_NewAndLaterYear_LatestYearBecomesCurrent()
    {
        // Arrange
        var career = new Career();
        var schoolId = SchoolId.NewId();
        var first = new Enrollment(schoolId, new SchoolYear(2023), GradeLevel.Junior);
        var second = new Enrollment(schoolId, new SchoolYear(2024), GradeLevel.Senior);

        // Act
        career.TryAddEnrollment(first);

        // Assert
        Assert.Same(first, career.Current);

        // Act
        career.TryAddEnrollment(second);

        // Assert
        Assert.Same(second, career.Current);
    }

    // ── Rejecting a duplicate year ────────────────────────────────────────

    [Fact]
    public void TryAddEnrollment_DuplicateYear_ReturnsFalseAndCareerUnchanged()
    {
        // Arrange
        var career = new Career();
        var schoolId = SchoolId.NewId();
        var year = new SchoolYear(2024);
        var first = new Enrollment(schoolId, year, GradeLevel.Junior);
        var duplicate = new Enrollment(schoolId, year, GradeLevel.Senior);
        career.TryAddEnrollment(first);
        var countBefore = career.Enrollments.Count;

        // Act
        var result = career.TryAddEnrollment(duplicate);

        // Assert
        Assert.False(result);
        Assert.Equal(countBefore, career.Enrollments.Count);
    }

    // ── Grade level stored correctly ──────────────────────────────────────

    [Fact]
    public void TryAddEnrollment_Grade_StoredOnEnrollment()
    {
        // Arrange
        var career = new Career();
        var schoolId = SchoolId.NewId();
        var enrollment = new Enrollment(schoolId, new SchoolYear(2024), GradeLevel.Sophomore);

        // Act
        career.TryAddEnrollment(enrollment);

        // Assert
        Assert.Same(GradeLevel.Sophomore, career.Current!.GradeLevel);
    }
}
