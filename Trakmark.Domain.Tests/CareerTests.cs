using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests;

/// <summary>
/// Tests for Career aggregate component: enrollment history management.
/// </summary>
public sealed class CareerTests
{
    // ── Adding enrollment for a new year ─────────────────────────────────

    [Fact]
    public void AddEnrollment_NewAndLaterYear_LatestYearBecomesCurrent()
    {
        // Arrange
        var career = new Career();
        var schoolId = SchoolId.NewId();
        var first = new Enrollment(schoolId, new SchoolYear(2023), GradeLevel.Junior);
        var second = new Enrollment(schoolId, new SchoolYear(2024), GradeLevel.Senior);

        // Act
        career.AddEnrollment(first);

        // Assert
        Assert.Same(first, career.Current);

        // Act
        career.AddEnrollment(second);

        // Assert
        Assert.Same(second, career.Current);
    }

    // ── Rejecting a duplicate year ────────────────────────────────────────

    [Fact]
    public void AddEnrollment_DuplicateYear_ThrowsAndCareerUnchanged()
    {
        // Arrange
        var career = new Career();
        var schoolId = SchoolId.NewId();
        var year = new SchoolYear(2024);
        var first = new Enrollment(schoolId, year, GradeLevel.Junior);
        var duplicate = new Enrollment(schoolId, year, GradeLevel.Senior);
        career.AddEnrollment(first);
        var countBefore = career.Enrollments.Count;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => career.AddEnrollment(duplicate));
        Assert.Equal(countBefore, career.Enrollments.Count);
    }

    // ── Grade level stored correctly ──────────────────────────────────────

    [Fact]
    public void AddEnrollment_Grade_StoredOnEnrollment()
    {
        // Arrange
        var career = new Career();
        var schoolId = SchoolId.NewId();
        var enrollment = new Enrollment(schoolId, new SchoolYear(2024), GradeLevel.Sophomore);

        // Act
        career.AddEnrollment(enrollment);

        // Assert
        Assert.Same(GradeLevel.Sophomore, career.Current!.Grade);
    }
}
