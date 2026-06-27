using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Aggregates;

/// <summary>
/// Tests for the Student aggregate and Career value object, covering the
/// manage-students spec scenarios for section 4.
/// </summary>
public sealed class StudentCareerTests
{
    private static StudentId NewStudentId() => StudentId.NewId();

    private static SchoolId NewSchoolId() => SchoolId.NewId();

    private static PersonName AnyName() => new("Jane Doe");

    [Fact]
    public void Student_HasEmptyCareerOnCreation()
    {
        // Arrange / Act
        var student = new Student(NewStudentId(), AnyName());

        // Assert
        Assert.Empty(student.Career.Enrollments);
        Assert.Null(student.Career.Current);
    }

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
        Assert.Equal(year2024, student.Career.Current.SchoolYear);
    }

    [Fact]
    public void AddEnrollment_DuplicateYear_IsRejectedAndCareerUnchanged()
    {
        // Arrange
        var student = new Student(NewStudentId(), AnyName());
        var schoolId = NewSchoolId();
        var year = new SchoolYear(2024);
        student.AddEnrollment(schoolId, year, GradeLevel.Junior);

        // Act
        var result = student.AddEnrollment(schoolId, year, GradeLevel.Senior);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.FailureReason);
        Assert.Single(student.Career.Enrollments);
    }

    /// <summary>Supplies all four grade level names as serializable test cases.</summary>
    public static TheoryData<string> AllGradeLevelNames =>
        [nameof(GradeLevel.Freshman), nameof(GradeLevel.Sophomore), nameof(GradeLevel.Junior), nameof(GradeLevel.Senior)];

    private static GradeLevel ToGradeLevel(string name) => name switch
    {
        nameof(GradeLevel.Freshman) => GradeLevel.Freshman,
        nameof(GradeLevel.Sophomore) => GradeLevel.Sophomore,
        nameof(GradeLevel.Junior) => GradeLevel.Junior,
        nameof(GradeLevel.Senior) => GradeLevel.Senior,
        _ => throw new ArgumentOutOfRangeException(nameof(name)),
    };

    [Theory]
    [MemberData(nameof(AllGradeLevelNames))]
    public void AddEnrollment_StoresSuppliedGradeLevel(string gradeLevelName)
    {
        // Arrange
        var grade = ToGradeLevel(gradeLevelName);
        var student = new Student(NewStudentId(), AnyName());

        // Act
        student.AddEnrollment(NewSchoolId(), new SchoolYear(2024), grade);

        // Assert
        var enrollment = Assert.Single(student.Career.Enrollments);
        Assert.Equal(grade, enrollment.GradeLevel);
    }

    [Fact]
    public void Current_ReturnsEnrollmentWithLatestSchoolYear()
    {
        // Arrange
        var student = new Student(NewStudentId(), AnyName());
        var schoolId = NewSchoolId();
        student.AddEnrollment(schoolId, new SchoolYear(2023), GradeLevel.Sophomore);
        student.AddEnrollment(schoolId, new SchoolYear(2024), GradeLevel.Junior);

        // Act
        var current = student.Career.Current;

        // Assert
        Assert.NotNull(current);
        Assert.Equal(new SchoolYear(2024), current.SchoolYear);
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

    [Fact]
    public void Enrollment_DifferentSchoolId_NotEqual()
    {
        // Arrange
        var year = new SchoolYear(2024);
        var grade = GradeLevel.Junior;
        var a = new Enrollment(NewSchoolId(), year, grade);
        var b = new Enrollment(NewSchoolId(), year, grade);

        // Assert
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Student_UserAccountId_IsNullByDefault()
    {
        // Arrange / Act
        var student = new Student(NewStudentId(), AnyName());

        // Assert
        Assert.Null(student.UserAccountId);
    }
}
