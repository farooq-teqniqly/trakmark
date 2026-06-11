using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Catalog;
using Trakmark.Domain.Ids;
using Trakmark.Domain.Services;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Tests.Services;

/// <summary>
/// Tests for section 7: Cross-aggregate domain services.
/// Covers the level-match service (record-meet-results spec) and
/// visibility resolution (view-athlete-season spec).
/// </summary>
public sealed class DomainServicesTests
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private static School CreateHsSchool() =>
        School.Create(new SchoolName("Lincoln High"), CompetitionLevel.HighSchool);

    private static School CreateMsSchool() =>
        School.Create(new SchoolName("Central Middle"), CompetitionLevel.MiddleSchool);

    private static Dictionary<SchoolId, School> SchoolMap(params School[] schools) =>
        schools.ToDictionary(s => s.Id);

    /// <summary>Creates a student enrolled at the given school for the 2025-26 year.</summary>
    private static Student StudentEnrolledAt(School school, SchoolYear year)
    {
        var student = new Student(StudentId.NewId(), new PersonName("Alice Johnson"));
        student.AddEnrollment(school.Id, year, GradeLevel.Junior);
        return student;
    }

    /// <summary>Creates a High School meet in the 2025-26 season (date in spring 2026).</summary>
    private static Meet HighSchoolMeet2025() =>
        Meet.Create(
            new MeetName("HS Spring Invitational"),
            new MeetDate(new DateOnly(2026, 4, 10)),
            CompetitionLevel.HighSchool,
            Sport.TrackAndField);

    /// <summary>Creates a Middle School meet in the 2025-26 season (date in spring 2026).</summary>
    private static Meet MiddleSchoolMeet2025() =>
        Meet.Create(
            new MeetName("MS Spring Invitational"),
            new MeetDate(new DateOnly(2026, 4, 10)),
            CompetitionLevel.MiddleSchool,
            Sport.TrackAndField);

    // ── CompetitionLevel match service ────────────────────────────────────────

    /// <summary>
    /// Scenario: Reject a level mismatch.
    /// A Middle School meet must reject a High School student.
    /// </summary>
    [Fact]
    public void LevelMatch_Validate_MismatchedLevel_ReturnsFalse()
    {
        // Arrange
        var hsSchool = CreateHsSchool();
        var student = StudentEnrolledAt(hsSchool, new SchoolYear(2025));
        var msMeet = MiddleSchoolMeet2025();
        var schools = SchoolMap(hsSchool);
        var service = new CompetitionLevelMatchService();

        // Act
        var isValid = service.IsLevelMatch(student, msMeet, schools);

        // Assert
        Assert.False(isValid);
    }

    /// <summary>
    /// Matching competition levels — High School student at a High School meet — are accepted.
    /// </summary>
    [Fact]
    public void LevelMatch_Validate_MatchingLevel_ReturnsTrue()
    {
        // Arrange
        var hsSchool = CreateHsSchool();
        var student = StudentEnrolledAt(hsSchool, new SchoolYear(2025));
        var hsMeet = HighSchoolMeet2025();
        var schools = SchoolMap(hsSchool);
        var service = new CompetitionLevelMatchService();

        // Act
        var isValid = service.IsLevelMatch(student, hsMeet, schools);

        // Assert
        Assert.True(isValid);
    }

    /// <summary>
    /// Scenario: Historical meets validate against the season's enrollment.
    /// Student was at Middle School in 2023-24 and moved to High School in 2025-26.
    /// A historical Middle School meet (date in 2023-24) should validate against MS enrollment.
    /// </summary>
    [Fact]
    public void LevelMatch_HistoricalMeet_ValidatesAgainstSeasonEnrollment_NotCurrentEnrollment()
    {
        // Arrange
        var msSchool = CreateMsSchool();
        var hsSchool = CreateHsSchool();

        var student = new Student(StudentId.NewId(), new PersonName("Carol Davis"));
        student.AddEnrollment(msSchool.Id, new SchoolYear(2023), GradeLevel.MiddleSchool8th);
        student.AddEnrollment(hsSchool.Id, new SchoolYear(2025), GradeLevel.Junior);

        // Historical meet: date 2024-03-15 resolves to SchoolYear(2023) = 2023-24
        var historicalMsMeet = Meet.Create(
            new MeetName("MS Regional 2023"),
            new MeetDate(new DateOnly(2024, 3, 15)),
            CompetitionLevel.MiddleSchool,
            Sport.TrackAndField);

        var schools = SchoolMap(msSchool, hsSchool);
        var service = new CompetitionLevelMatchService();

        // Act – student was MS in 2023-24, meet is MS ⇒ should be valid
        var isValid = service.IsLevelMatch(student, historicalMsMeet, schools);

        // Assert
        Assert.True(isValid);
    }

    /// <summary>
    /// A historical HS-level meet for a student who was enrolled at MS in that season is rejected.
    /// This confirms the service does NOT use the current enrollment.
    /// </summary>
    [Fact]
    public void LevelMatch_HistoricalMeet_MismatchedHistoricalLevel_ReturnsFalse()
    {
        // Arrange
        var msSchool = CreateMsSchool();
        var hsSchool = CreateHsSchool();

        var student = new Student(StudentId.NewId(), new PersonName("Dave Wilson"));
        student.AddEnrollment(msSchool.Id, new SchoolYear(2023), GradeLevel.MiddleSchool8th);
        student.AddEnrollment(hsSchool.Id, new SchoolYear(2025), GradeLevel.Junior);

        // Historical HS meet in the 2023-24 season — student was MS then
        var historicalHsMeet = Meet.Create(
            new MeetName("HS Regional 2023"),
            new MeetDate(new DateOnly(2024, 3, 15)),
            CompetitionLevel.HighSchool,
            Sport.TrackAndField);

        var schools = SchoolMap(msSchool, hsSchool);
        var service = new CompetitionLevelMatchService();

        // Act
        var isValid = service.IsLevelMatch(student, historicalHsMeet, schools);

        // Assert
        Assert.False(isValid);
    }

    /// <summary>
    /// When the student has an enrollment for the season but the school is not in the
    /// schools dictionary, the service returns false (covers the TryGetValue branch).
    /// </summary>
    [Fact]
    public void LevelMatch_SchoolNotInDictionary_ReturnsFalse()
    {
        // Arrange
        var hsSchool = CreateHsSchool();
        var student = StudentEnrolledAt(hsSchool, new SchoolYear(2025));
        var hsMeet = HighSchoolMeet2025();

        // Pass an empty schools map — enrollment exists but school cannot be resolved
        var emptySchools = new Dictionary<SchoolId, School>();
        var service = new CompetitionLevelMatchService();

        // Act
        var isValid = service.IsLevelMatch(student, hsMeet, emptySchools);

        // Assert
        Assert.False(isValid);
    }

    /// <summary>
    /// When the student has no enrollment for the meet's season, the service returns false.
    /// </summary>
    [Fact]
    public void LevelMatch_NoEnrollmentForSeason_ReturnsFalse()
    {
        // Arrange – student has no enrollment at all
        var student = new Student(StudentId.NewId(), new PersonName("Eve Thomas"));
        var meet = HighSchoolMeet2025();
        var service = new CompetitionLevelMatchService();

        // Act
        var isValid = service.IsLevelMatch(student, meet, new Dictionary<SchoolId, School>());

        // Assert
        Assert.False(isValid);
    }

    // ── Student visibility resolution ─────────────────────────────────────────

    /// <summary>
    /// Scenario: Followed students are visible.
    /// A student in the user's Following set is included.
    /// </summary>
    [Fact]
    public void Visibility_FollowedStudent_IsVisible()
    {
        // Arrange
        var user = RegisteredUser.Create(new UserAccountId("account-1"));
        var student = new Student(StudentId.NewId(), new PersonName("Alice Johnson"));
        user.Follow(student.Id);

        var allStudents = new[] { student };
        var service = new StudentVisibilityService();

        // Act
        var visible = service.GetVisibleStudents(user, allStudents).ToList();

        // Assert
        Assert.Contains(student, visible);
    }

    /// <summary>
    /// Scenario: Unrelated students are not visible.
    /// A student who is neither followed nor linked to the user's account is excluded.
    /// </summary>
    [Fact]
    public void Visibility_UnrelatedStudent_IsNotVisible()
    {
        // Arrange
        var user = RegisteredUser.Create(new UserAccountId("account-1"));
        var unrelated = new Student(StudentId.NewId(), new PersonName("Bob Smith"));

        var allStudents = new[] { unrelated };
        var service = new StudentVisibilityService();

        // Act
        var visible = service.GetVisibleStudents(user, allStudents).ToList();

        // Assert
        Assert.DoesNotContain(unrelated, visible);
    }

    /// <summary>
    /// A student whose <see cref="Student.UserAccountId"/> matches the user's account
    /// is visible even when not in the Following set.
    /// </summary>
    [Fact]
    public void Visibility_StudentWithMatchingUserAccountId_IsVisible()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);

        // Student linked to the same Identity account; user has not explicitly followed
        var linkedStudent = new Student(StudentId.NewId(), new PersonName("Carol Davis"), accountId);

        var allStudents = new[] { linkedStudent };
        var service = new StudentVisibilityService();

        // Act
        var visible = service.GetVisibleStudents(user, allStudents).ToList();

        // Assert
        Assert.Contains(linkedStudent, visible);
    }

    /// <summary>
    /// The visibility set is the union: followed ∪ account-linked.
    /// A student appearing via both paths is returned only once.
    /// </summary>
    [Fact]
    public void Visibility_FollowedAndLinked_ReturnedOnce()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);
        var linkedStudent = new Student(StudentId.NewId(), new PersonName("Dave Wilson"), accountId);
        user.Follow(linkedStudent.Id);

        var allStudents = new[] { linkedStudent };
        var service = new StudentVisibilityService();

        // Act
        var visible = service.GetVisibleStudents(user, allStudents).ToList();

        // Assert
        Assert.Single(visible);
        Assert.Contains(linkedStudent, visible);
    }

    /// <summary>
    /// Multiple followed students plus one account-linked student — all are returned;
    /// unrelated students are excluded.
    /// </summary>
    [Fact]
    public void Visibility_MultipleFollowedAndLinked_AllVisible()
    {
        // Arrange
        var accountId = new UserAccountId("account-1");
        var user = RegisteredUser.Create(accountId);

        var s1 = new Student(StudentId.NewId(), new PersonName("Alice Johnson"));
        var s2 = new Student(StudentId.NewId(), new PersonName("Bob Smith"));
        var linked = new Student(StudentId.NewId(), new PersonName("Carol Davis"), accountId);
        var unrelated = new Student(StudentId.NewId(), new PersonName("Eve Thomas"));

        user.Follow(s1.Id);
        user.Follow(s2.Id);

        var allStudents = new[] { s1, s2, linked, unrelated };
        var service = new StudentVisibilityService();

        // Act
        var visible = service.GetVisibleStudents(user, allStudents).ToList();

        // Assert
        Assert.Equal(3, visible.Count);
        Assert.Contains(s1, visible);
        Assert.Contains(s2, visible);
        Assert.Contains(linked, visible);
        Assert.DoesNotContain(unrelated, visible);
    }
}
