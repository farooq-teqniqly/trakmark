using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Services;

/// <summary>
/// Converts calendar dates to <see cref="SchoolYear"/> values.
/// School years start in August: dates in August or later belong to the year
/// starting that calendar year; earlier dates belong to the year that started
/// the prior calendar year.
/// </summary>
internal static class SchoolYearHelper
{
    /// <summary>
    /// Returns the <see cref="SchoolYear"/> that contains <paramref name="date"/>.
    /// </summary>
    internal static SchoolYear ToSchoolYear(DateOnly date)
    {
        var startYear = date.Month >= 8 ? date.Year : date.Year - 1;
        return new SchoolYear(startYear);
    }
}
