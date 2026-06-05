using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests;

/// <summary>
/// Tests for the Student aggregate and Career value object, covering the
/// manage-students spec scenarios for section 4.
/// </summary>
public sealed class StudentCareerTests
{
    // ── Shared helpers ─────────────────────────────────────────────────────

    private static StudentId NewStudentId() => StudentId.NewId();
    private static SchoolId NewSchoolId() => SchoolId.NewId();
    private static PersonName AnyName() => new("Jane Doe");

    // ── Empty career until enrollment ──────────────────────────────────────

    [Fact]
    public void Student_HasEmptyCareerOnCreation()
    {
        // Arrange / Act
        var student = new Student(NewStudentId(), AnyName());

        // Assert
        Assert.Empty(student.Career.Enrollments);
        Assert.Null(student.Career.Current);
    }

    // ── Add an enrollment for a new school year ────────────────────────────

    [Fact]
    public void AddEnrollment_ForNewYear_AddsToCareer()
    {
        // Arrange
        var student = new Student(NewStudentId(), AnyName());
        var schoolId = NewSchoolId();
        var year = new SchoolYear(2024);
        var grade = GradeLevel.Junior;

        // Act
        var result = student.AddEnrollment(schoolId, year, grade);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(student.Career.Enrollments);
    }

    [Fact]
    public void AddEnrollment_LatestYearIsCurrent()
    {
        // Arrange
        var student = new Student(NewStudentId(), AnyName());
        var schoolId = NewSchoolId();
        var year2023 = new SchoolYear(2023);
        var year2024 = new SchoolYear(2024);

        // Act
        student.AddEnrollment(schoolId, year2023, GradeLevel.Sophomore);
        student.AddEnrollment(schoolId, year2024, GradeLevel.Junior);

        // Assert
        Assert.NotNull(student.Career.Current);
        Assert.Equal(year2024, student.Career.Current!.SchoolYear);
    }

    // ── Reject a duplicate school year ────────────────────────────────────

    [Fact]
    public void AddEnrollment_DuplicateYear_IsRejectedAndCareerUnchanged()
    {
        // Arrange
        var student = new Student(NewStudentId(), AnyName());
        var schoolId = NewSchoolId();
        var year = new SchoolYear(2024);
        student.AddEnrollment(schoolId, year, GradeLevel.Junior);

        // Act — attempt to add same year again
        var result = student.AddEnrollment(schoolId, year, GradeLevel.Senior);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(student.Career.Enrollments);
    }

    // ── Grade level is recorded independently ──────────────────────────────

    [Theory]
    [InlineData("Freshman")]
    [InlineData("Sophomore")]
    [InlineData("Junior")]
    [InlineData("Senior")]
    public void AddEnrollment_StoresSuppliedGradeLevel(string gradeName)
    {
        // Arrange
        var student = new Student(NewStudentId(), AnyName());
        var grade = gradeName switch
        {
            "Freshman" => GradeLevel.Freshman,
            "Sophomore" => GradeLevel.Sophomore,
            "Junior" => GradeLevel.Junior,
            "Senior" => GradeLevel.Senior,
            _ => throw new InvalidOperationException()
        };

        // Act
        student.AddEnrollment(NewSchoolId(), new SchoolYear(2024), grade);

        // Assert
        var enrollment = Assert.Single(student.Career.Enrollments);
        Assert.Equal(grade, enrollment.GradeLevel);
    }

    // ── Season helpers ─────────────────────────────────────────────────────

    [Fact]
    public void CurrentSeason_ReturnsCurrentEnrollment()
    {
        // Arrange
        var student = new Student(NewStudentId(), AnyName());
        var schoolId = NewSchoolId();
        student.AddEnrollment(schoolId, new SchoolYear(2023), GradeLevel.Sophomore);
        student.AddEnrollment(schoolId, new SchoolYear(2024), GradeLevel.Junior);

        // Act
        var current = student.Career.CurrentSeason;

        // Assert
        Assert.NotNull(current);
        Assert.Equal(new SchoolYear(2024), current!.SchoolYear);
    }

    [Fact]
    public void PastSeasons_ReturnsPriorEnrollments()
    {
        // Arrange
        var student = new Student(NewStudentId(), AnyName());
        var schoolId = NewSchoolId();
        var year2023 = new SchoolYear(2023);
        var year2024 = new SchoolYear(2024);
        student.AddEnrollment(schoolId, year2023, GradeLevel.Sophomore);
        student.AddEnrollment(schoolId, year2024, GradeLevel.Junior);

        // Act
        var past = student.Career.PastSeasons.ToList();

        // Assert
        Assert.Single(past);
        Assert.Equal(year2023, past[0].SchoolYear);
    }

    [Fact]
    public void PastSeasons_IsEmpty_WhenOnlyOneEnrollment()
    {
        // Arrange
        var student = new Student(NewStudentId(), AnyName());
        student.AddEnrollment(NewSchoolId(), new SchoolYear(2024), GradeLevel.Junior);

        // Act
        var past = student.Career.PastSeasons.ToList();

        // Assert
        Assert.Empty(past);
    }

    // ── Enrollment value object equality ──────────────────────────────────

    [Fact]
    public void Enrollment_EqualityBySchoolIdYearAndGrade()
    {
        // Arrange
        var schoolId = NewSchoolId();
        var year = new SchoolYear(2024);
        var grade = GradeLevel.Junior;

        // Act
        var a = new Enrollment(schoolId, year, grade);
        var b = new Enrollment(schoolId, year, grade);

        // Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void Enrollment_DifferentYear_NotEqual()
    {
        // Arrange
        var schoolId = NewSchoolId();
        var grade = GradeLevel.Junior;
        var a = new Enrollment(schoolId, new SchoolYear(2023), grade);
        var b = new Enrollment(schoolId, new SchoolYear(2024), grade);

        // Assert
        Assert.NotEqual(a, b);
    }

    // ── Student.UserAccountId is absent by default ─────────────────────────

    [Fact]
    public void Student_UserAccountId_IsNullByDefault()
    {
        // Arrange / Act
        var student = new Student(NewStudentId(), AnyName());

        // Assert
        Assert.Null(student.UserAccountId);
    }
}
