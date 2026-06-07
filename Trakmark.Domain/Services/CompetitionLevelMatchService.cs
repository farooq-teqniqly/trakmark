using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Services;

/// <summary>
/// Domain service that validates whether a meet's <see cref="CompetitionLevel"/>
/// matches the student's enrollment level for the meet's season.
/// </summary>
/// <remarks>
/// This is a cross-aggregate invariant: a meet's level is owned by <see cref="Meet"/>,
/// while the student's enrollment level is derived from the <see cref="School"/> the
/// student was enrolled in for the season whose <see cref="SchoolYear"/> corresponds
/// to the meet's date. Historical meets validate against the then-current enrollment,
/// not the student's current enrollment.
/// </remarks>
public sealed class CompetitionLevelMatchService
{
    /// <summary>
    /// Returns <see langword="true"/> when the meet's <see cref="CompetitionLevel"/>
    /// matches the student's school level for the season containing the meet's date.
    /// </summary>
    /// <param name="student">The student whose enrollment to check.</param>
    /// <param name="meet">The meet whose level and date are used for validation.</param>
    /// <param name="schools">
    /// A map of <see cref="SchoolId"/> to <see cref="School"/> used to resolve each
    /// enrollment's competition level.
    /// </param>
    /// <returns>
    /// <see langword="false"/> when the student has no enrollment for the meet's season,
    /// the enrollment's school cannot be found in <paramref name="schools"/>, or the
    /// school's competition level differs from the meet's level.
    /// </returns>
    public bool IsLevelMatch(
        Student student,
        Meet meet,
        IReadOnlyDictionary<SchoolId, School> schools)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(meet);
        ArgumentNullException.ThrowIfNull(schools);

        var season = SchoolYearHelper.ToSchoolYear(meet.Date.Value);

        var enrollment = student.Career.Enrollments
            .FirstOrDefault(e => e.SchoolYear == season);

        if (enrollment is null)
        {
            return false;
        }

        if (!schools.TryGetValue(enrollment.SchoolId, out var school))
        {
            return false;
        }

        return school.Level.Equals(meet.Level);
    }
}
