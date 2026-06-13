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

    // ── Inserting a middle year maintains chronological order ────────────────

    [Fact]
    public void TryAddEnrollment_MiddleYear_InsertsInChronologicalOrder()
    {
        // Arrange
        var career = new Career();
        var schoolId = SchoolId.NewId();
        var earliest = new Enrollment(schoolId, new SchoolYear(2022), GradeLevel.Freshman);
        var latest = new Enrollment(schoolId, new SchoolYear(2024), GradeLevel.Senior);
        var middle = new Enrollment(schoolId, new SchoolYear(2023), GradeLevel.Junior);
        career.TryAddEnrollment(earliest);
        career.TryAddEnrollment(latest);

        // Act
        var result = career.TryAddEnrollment(middle);

        // Assert — all three accepted; list is in ascending year order
        Assert.True(result);
        Assert.Equal(3, career.Enrollments.Count);
        Assert.Equal(new SchoolYear(2022), career.Enrollments[0].SchoolYear);
        Assert.Equal(new SchoolYear(2023), career.Enrollments[1].SchoolYear);
        Assert.Equal(new SchoolYear(2024), career.Enrollments[2].SchoolYear);
        // Current is the latest year
        Assert.Equal(new SchoolYear(2024), career.Current!.SchoolYear);
    }

    // ── Inserting an earlier year keeps chronological order ───────────────

    [Fact]
    public void TryAddEnrollment_EarlierYear_InsertsBeforeExisting()
    {
        // Arrange — add a later year first, then an earlier year
        var career = new Career();
        var schoolId = SchoolId.NewId();
        var later = new Enrollment(schoolId, new SchoolYear(2025), GradeLevel.Senior);
        var earlier = new Enrollment(schoolId, new SchoolYear(2023), GradeLevel.Junior);

        career.TryAddEnrollment(later);

        // Act
        var result = career.TryAddEnrollment(earlier);

        // Assert — both enrollments accepted; list is in ascending year order
        Assert.True(result);
        Assert.Equal(2, career.Enrollments.Count);
        Assert.Equal(new SchoolYear(2023), career.Enrollments[0].SchoolYear);
        Assert.Equal(new SchoolYear(2025), career.Enrollments[1].SchoolYear);
        // Current is still the latest year
        Assert.Equal(new SchoolYear(2025), career.Current!.SchoolYear);
    }
}
