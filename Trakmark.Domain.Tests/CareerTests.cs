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
    public void AddEnrollment_NewYear_BecomesCurrentEnrollment()
    {
        // Arrange
        var career = new Career();
        var schoolId = SchoolId.NewId();
        var year = new SchoolYear(2024);
        var enrollment = new Enrollment(schoolId, year, GradeLevel.Junior);

        // Act
        career.AddEnrollment(enrollment);

        // Assert
        Assert.Same(enrollment, career.Current);
    }

    [Fact]
    public void AddEnrollment_LaterYear_LaterYearBecomesCurrent()
    {
        // Arrange
        var career = new Career();
        var schoolId = SchoolId.NewId();
        var earlier = new Enrollment(schoolId, new SchoolYear(2023), GradeLevel.Junior);
        var later = new Enrollment(schoolId, new SchoolYear(2024), GradeLevel.Senior);

        // Act
        career.AddEnrollment(earlier);
        career.AddEnrollment(later);

        // Assert
        Assert.Same(later, career.Current);
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
