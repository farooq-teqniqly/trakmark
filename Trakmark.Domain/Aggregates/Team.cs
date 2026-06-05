using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// An entity representing a sport team fielded by a <see cref="School"/>.
/// Roster membership is derived from student enrollments and event participation;
/// it is not stored on this entity.
/// </summary>
public sealed class Team
{
    /// <summary>The unique identifier for this team.</summary>
    public TeamId Id { get; }

    /// <summary>The sport this team is bound to.</summary>
    public Sport Sport { get; }

    /// <summary>Initializes a <see cref="Team"/> bound to the given <paramref name="sport"/>.</summary>
    internal Team(TeamId id, Sport sport)
    {
        Id = id;
        Sport = sport;
    }
}
