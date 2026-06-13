using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Catalog;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Services;

/// <summary>
/// Domain service that computes season bests and personal bests for a student
/// and discipline. Bests are always derived from stored results — never cached
/// or flagged on individual results.
/// </summary>
/// <remarks>
/// Eligibility rules (applied consistently to both season best and personal best):
/// <list type="bullet">
///   <item><description>Only <see cref="ResultStatus.Finished"/> results count.</description></item>
///   <item><description>Only individual (non-relay) results count.</description></item>
///   <item><description>The discipline must not be <see cref="MarkKind.PlaceOnly"/>.</description></item>
///   <item><description>A measured <see cref="Performance"/> mark must be present.</description></item>
///   <item><description>Tier is ignored — marks from any tier are eligible.</description></item>
/// </list>
/// </remarks>
public sealed class BestMarksService
{
    /// <summary>
    /// Computes the season best for <paramref name="student"/> in
    /// <paramref name="discipline"/> during <paramref name="season"/>.
    /// </summary>
    /// <param name="student">The student to compute the season best for.</param>
    /// <param name="discipline">The discipline to filter by.</param>
    /// <param name="season">The school year that defines the season boundary.</param>
    /// <param name="allResults">
    /// All available results. Each result carries its meet date via
    /// <see cref="Result.MeetDate"/>, which is used to resolve the school year.
    /// </param>
    /// <returns>
    /// The best <see cref="Performance"/> mark for the season, or
    /// <see langword="null"/> when no eligible results exist or the discipline
    /// is place-only or a relay.
    /// </returns>
    public static Performance? SeasonBest(
        Student student,
        Discipline discipline,
        SchoolYear season,
        IEnumerable<Result> allResults)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(discipline);
        ArgumentNullException.ThrowIfNull(allResults);

        if (!HasMeasurableBest(discipline))
        {
            return null;
        }

        var eligible = allResults
            .Where(r =>
                r.StudentId == student.Id
                && r.Event.Discipline.Equals(discipline)
                && r.Status == ResultStatus.Finished
                && !r.Event.IsRelay
                && r.Mark is not null
                && SchoolYearHelper.ToSchoolYear(r.MeetDate.Value) == season);

        return SelectBest(eligible);
    }

    /// <summary>
    /// Computes the personal best for <paramref name="student"/> in
    /// <paramref name="discipline"/> across all seasons.
    /// </summary>
    /// <param name="student">The student to compute the personal best for.</param>
    /// <param name="discipline">The discipline to filter by.</param>
    /// <param name="allResults">
    /// All available results across all meets and seasons.
    /// </param>
    /// <returns>
    /// The best <see cref="Performance"/> mark across all seasons, or
    /// <see langword="null"/> when no eligible results exist or the discipline
    /// is place-only or a relay.
    /// </returns>
    public static Performance? PersonalBest(
        Student student,
        Discipline discipline,
        IEnumerable<Result> allResults)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(discipline);
        ArgumentNullException.ThrowIfNull(allResults);

        if (!HasMeasurableBest(discipline))
        {
            return null;
        }

        var eligible = allResults
            .Where(r =>
                r.StudentId == student.Id
                && r.Event.Discipline.Equals(discipline)
                && r.Status == ResultStatus.Finished
                && !r.Event.IsRelay
                && r.Mark is not null);

        return SelectBest(eligible);
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="discipline"/> supports
    /// a measurable best (i.e. is not place-only and is not a relay).
    /// </summary>
    private static bool HasMeasurableBest(Discipline discipline) =>
        discipline.MarkKind != MarkKind.PlaceOnly && !discipline.IsRelay;

    /// <summary>
    /// Selects the best mark from <paramref name="eligible"/> results using each
    /// mark's own <see cref="Performance.IsBetterThan"/> comparison (direction is
    /// baked into the concrete <see cref="Performance"/> subtype).
    /// </summary>
    private static Performance? SelectBest(IEnumerable<Result> eligible)
    {
        Performance? best = null;

        // S3267: cannot simplify with Where — requires running-best accumulation via IsBetterThan.
#pragma warning disable S3267
        foreach (var result in eligible)
        {
            if (best is null || result.Mark!.IsBetterThan(best))
            {
                best = result.Mark;
            }
        }
#pragma warning restore S3267

        return best;
    }
}
