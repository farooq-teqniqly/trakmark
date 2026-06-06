using Trakmark.Domain.Aggregates;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Services;

/// <summary>
/// Domain service that projects a student's results for a given season.
/// A season corresponds to a <see cref="SchoolYear"/> derived from each
/// result's <see cref="Result.MeetDate"/>. Results are returned in entry
/// <see cref="Result.Order"/> ascending.
/// </summary>
public sealed class SeasonViewService
{
    /// <summary>
    /// Returns the subset of <paramref name="allResults"/> that belong to
    /// <paramref name="student"/> and whose meet date falls within
    /// <paramref name="season"/>, ordered by <see cref="Result.Order"/> ascending.
    /// </summary>
    /// <param name="student">The student whose results to project.</param>
    /// <param name="allResults">
    /// All results available for filtering. Each result carries its meet date
    /// via <see cref="Result.MeetDate"/>, which is used to resolve the school year.
    /// </param>
    /// <param name="season">The school year to filter by.</param>
    /// <returns>
    /// The student's results in the given season, ordered by entry order.
    /// </returns>
    public IEnumerable<Result> GetSeasonResults(
        Student student,
        IEnumerable<Result> allResults,
        SchoolYear season)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(allResults);

        return allResults
            .Where(r => r.StudentId == student.Id && ToSchoolYear(r.MeetDate.Value) == season)
            .OrderBy(r => r.Order);
    }

    /// <summary>
    /// Converts a calendar date to the <see cref="SchoolYear"/> that contains it.
    /// Dates in August or later belong to the year starting that calendar year;
    /// earlier dates belong to the year that started the prior calendar year.
    /// </summary>
    private static SchoolYear ToSchoolYear(DateOnly date)
    {
        var startYear = date.Month >= 8 ? date.Year : date.Year - 1;
        return new SchoolYear(startYear);
    }
}
