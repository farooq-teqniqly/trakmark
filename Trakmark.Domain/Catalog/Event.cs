using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Catalog;

/// <summary>
/// A pairing of a <see cref="Catalog.Discipline"/> with its <see cref="ValueObjects.Sport"/>,
/// forming the specific competitive event that a result is recorded against.
/// Relay-ness is delegated to the discipline.
/// </summary>
public sealed class Event : IEquatable<Event>
{
    /// <summary>The discipline that defines this event's rules and mark kind.</summary>
    public Discipline Discipline { get; }

    /// <summary>The sport this event belongs to.</summary>
    public Sport Sport { get; }

    /// <summary>Whether this event is a relay, as determined by its discipline.</summary>
    public bool IsRelay => Discipline.IsRelay;

    /// <summary>Initializes a new <see cref="Event"/>.</summary>
    /// <param name="discipline">The discipline.</param>
    /// <param name="sport">The sport.</param>
    public Event(Discipline discipline, Sport sport)
    {
        Discipline = discipline;
        Sport = sport;
    }

    /// <inheritdoc/>
    public bool Equals(Event? other)
    {
        if (other is null) { return false; }

        return Discipline.Equals(other.Discipline) && Sport.Equals(other.Sport);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Event);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Discipline, Sport);

    /// <inheritdoc/>
    public override string ToString() => $"{Sport} — {Discipline}";
}
