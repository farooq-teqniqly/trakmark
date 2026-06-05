namespace Trakmark.Domain.Aggregates;

/// <summary>
/// An ordered set of <see cref="Enrollment"/>s for a student, enforcing the invariant
/// that no two enrollments share the same <see cref="ValueObjects.SchoolYear"/>.
/// The current enrollment is the one with the latest school year.
/// </summary>
public sealed class Career
{
    private readonly List<Enrollment> _enrollments = [];

    /// <summary>All enrollments in the career, ordered by school year ascending.</summary>
    public IReadOnlyList<Enrollment> Enrollments => _enrollments.AsReadOnly();

    /// <summary>
    /// The enrollment with the latest school year, or <see langword="null"/> when the
    /// career is empty.
    /// </summary>
    public Enrollment? Current => _enrollments.Count > 0
        ? _enrollments[^1]
        : null;

    /// <summary>
    /// The current enrollment (latest school year), or <see langword="null"/> when the
    /// career is empty.
    /// </summary>
    public Enrollment? CurrentSeason => Current;

    /// <summary>
    /// All enrollments except the current (latest) one, ordered by school year ascending.
    /// Returns an empty sequence when fewer than two enrollments exist.
    /// </summary>
    public IEnumerable<Enrollment> PastSeasons => _enrollments.Count > 1
        ? _enrollments.Take(_enrollments.Count - 1)
        : [];

    /// <summary>
    /// Attempts to add an <paramref name="enrollment"/> to the career.
    /// Returns <see langword="false"/> when an enrollment for the same school year already exists.
    /// </summary>
    internal bool TryAdd(Enrollment enrollment)
    {
        if (_enrollments.Any(e => e.SchoolYear == enrollment.SchoolYear))
        {
            return false;
        }

        _enrollments.Add(enrollment);
        _enrollments.Sort((a, b) => a.SchoolYear.CompareTo(b.SchoolYear));
        return true;
    }
}
