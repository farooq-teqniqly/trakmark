using Trakmark.Domain.ValueObjects;

namespace Trakmark.Helpers;

/// <summary>
/// Provides a sorted list of all U.S. states plus D.C. for use in UI dropdowns,
/// and a lookup from two-letter abbreviation to <see cref="State"/> instance.
/// </summary>
internal static class StateHelper
{
    /// <summary>All 51 <see cref="State"/> instances sorted alphabetically by name.</summary>
    internal static readonly IReadOnlyList<State> AllStates =
    [
        State.Alabama,
        State.Alaska,
        State.Arizona,
        State.Arkansas,
        State.California,
        State.Colorado,
        State.Connecticut,
        State.Delaware,
        State.DistrictOfColumbia,
        State.Florida,
        State.Georgia,
        State.Hawaii,
        State.Idaho,
        State.Illinois,
        State.Indiana,
        State.Iowa,
        State.Kansas,
        State.Kentucky,
        State.Louisiana,
        State.Maine,
        State.Maryland,
        State.Massachusetts,
        State.Michigan,
        State.Minnesota,
        State.Mississippi,
        State.Missouri,
        State.Montana,
        State.Nebraska,
        State.Nevada,
        State.NewHampshire,
        State.NewJersey,
        State.NewMexico,
        State.NewYork,
        State.NorthCarolina,
        State.NorthDakota,
        State.Ohio,
        State.Oklahoma,
        State.Oregon,
        State.Pennsylvania,
        State.RhodeIsland,
        State.SouthCarolina,
        State.SouthDakota,
        State.Tennessee,
        State.Texas,
        State.Utah,
        State.Vermont,
        State.Virginia,
        State.Washington,
        State.WestVirginia,
        State.Wisconsin,
        State.Wyoming,
    ];

    private static readonly Dictionary<string, State> ByAbbreviation = AllStates.ToDictionary(
        s => s.Abbreviation,
        s => s,
        StringComparer.OrdinalIgnoreCase
    );

    /// <summary>
    /// Returns the <see cref="State"/> with the given two-letter abbreviation,
    /// or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="abbreviation">Two-letter state abbreviation (e.g. <c>"IL"</c>).</param>
    internal static State? GetByAbbreviation(string? abbreviation)
    {
        if (string.IsNullOrEmpty(abbreviation))
        {
            return null;
        }

        return ByAbbreviation.TryGetValue(abbreviation, out var state) ? state : null;
    }
}
