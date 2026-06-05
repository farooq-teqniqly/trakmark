using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// Records a student's academic career as a set of yearly enrollments.
/// One enrollment per <see cref="SchoolYear"/> is enforced; the enrollment
/// with the latest year is the current enrollment.
/// </summary>
public sealed class Career
{
    private readonly List<Enrollment> _enrollments = [];

    /// <summary>The read-only ordered set of enrollments in this career.</summary>
    public IReadOnlyList<Enrollment> Enrollments => _enrollments.AsReadOnly();

    /// <summary>The enrollment with the latest <see cref="SchoolYear"/>, or <see langword="null"/> when no enrollments exist.</summary>
    public Enrollment? Current => _enrollments.Count == 0
        ? null
        : _enrollments.MaxBy(e => e.Year);

    /// <summary>
    /// Adds an <see cref="Enrollment"/> to the career.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when a duplicate <see cref="SchoolYear"/> already exists.</exception>
    public void AddEnrollment(Enrollment enrollment)
    {
        if (_enrollments.Any(e => e.Year == enrollment.Year))
        {
            throw new InvalidOperationException(
                $"An enrollment for school year {enrollment.Year} already exists.");
        }

        _enrollments.Add(enrollment);
    }
}
