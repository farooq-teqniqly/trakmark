using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// The <c>School</c> aggregate root. Owns a catalog of <see cref="Team"/>s,
/// each bound to exactly one <see cref="ValueObjects.Sport"/>.
/// </summary>
public sealed class School
{
    private readonly List<Team> _teams = [];

    /// <summary>The unique identifier for this school.</summary>
    public SchoolId Id { get; }

    /// <summary>The name of this school.</summary>
    public SchoolName Name { get; }

    /// <summary>The competition level at which this school competes.</summary>
    public CompetitionLevel Level { get; }

    /// <summary>The catalog of sports teams fielded by this school.</summary>
    public IReadOnlyList<Team> Teams => _teams;

    private School(SchoolId id, SchoolName name, CompetitionLevel level)
    {
        Id = id;
        Name = name;
        Level = level;
    }

    /// <summary>
    /// Creates a new <see cref="School"/> with a generated <see cref="SchoolId"/>.
    /// </summary>
    /// <param name="name">The school's name.</param>
    /// <param name="level">The competition level.</param>
    public static School Create(SchoolName name, CompetitionLevel level) =>
        new(SchoolId.NewId(), name, level);

    /// <summary>
    /// Adds a <see cref="Team"/> for the given <paramref name="sport"/> to this school's catalog.
    /// </summary>
    /// <param name="sport">The sport the team competes in.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the school already fields a team for <paramref name="sport"/>.
    /// </exception>
    public void AddTeam(Sport sport)
    {
        if (_teams.Any(t => t.Sport.Equals(sport)))
        {
            throw new InvalidOperationException(
                $"School '{Name}' already fields a team for {sport}.");
        }

        _teams.Add(new Team(TeamId.NewId(), sport));
    }
}
