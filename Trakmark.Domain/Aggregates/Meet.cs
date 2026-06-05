using Trakmark.Domain.Catalog;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// The <c>Meet</c> aggregate root. A meet is bound to exactly one sport and one
/// competition level. Results are recorded against events whose sport matches
/// the meet's sport.
/// </summary>
public sealed class Meet
{
    private readonly List<Result> _results = [];

    /// <summary>The unique identifier for this meet.</summary>
    public MeetId Id { get; }

    /// <summary>The display name of the meet.</summary>
    public MeetName Name { get; }

    /// <summary>The date on which the meet takes place.</summary>
    public MeetDate Date { get; }

    /// <summary>The competition level for this meet (e.g., High School).</summary>
    public CompetitionLevel Level { get; }

    /// <summary>The sport all events in this meet belong to.</summary>
    public Sport Sport { get; }

    /// <summary>The results recorded so far, in entry order.</summary>
    public IReadOnlyList<Result> Results => _results;

    private Meet(MeetId id, MeetName name, MeetDate date, CompetitionLevel level, Sport sport)
    {
        Id = id;
        Name = name;
        Date = date;
        Level = level;
        Sport = sport;
    }

    /// <summary>
    /// Creates a new <see cref="Meet"/> with a generated <see cref="MeetId"/>.
    /// </summary>
    /// <param name="name">The meet name.</param>
    /// <param name="date">The meet date.</param>
    /// <param name="level">The competition level.</param>
    /// <param name="sport">The sport for all events in this meet.</param>
    public static Meet Create(MeetName name, MeetDate date, CompetitionLevel level, Sport sport) =>
        new(MeetId.NewId(), name, date, level, sport);

    /// <summary>
    /// Records a result for a student in an event at this meet.
    /// </summary>
    /// <param name="studentId">The student the result belongs to.</param>
    /// <param name="event">The event. Its sport must match <see cref="Sport"/>.</param>
    /// <param name="status">The completion status.</param>
    /// <param name="mark">
    /// The performance mark. Required and must match the discipline's mark kind when
    /// <paramref name="status"/> is <see cref="ResultStatus.Finished"/> and the discipline
    /// is not place-only. Must be <see langword="null"/> for non-finished statuses.
    /// </param>
    /// <param name="place">
    /// The finish placement. Required when <paramref name="status"/> is
    /// <see cref="ResultStatus.Finished"/>. Must be <see langword="null"/> otherwise.
    /// </param>
    /// <param name="tier">
    /// The competitive tier. Defaults to <see cref="Tier.Open"/> when <see langword="null"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the event's sport does not match the meet's sport, when a finished
    /// result is missing a required mark or place, when a non-finished result supplies
    /// a mark or place, or when the mark kind does not match the discipline.
    /// </exception>
    public void RecordResult(
        StudentId studentId,
        Event @event,
        ResultStatus status,
        Performance? mark,
        Placement? place,
        Tier? tier)
    {
        EnforceSportMatch(@event);
        EnforceStatusInvariant(@event, status, mark, place);

        var effectiveTier = tier ?? Tier.Open;
        var order = NextOrderFor(studentId);
        _results.Add(new Result(studentId, @event, status, mark, place, effectiveTier, order));
    }

    private void EnforceSportMatch(Event @event)
    {
        if (!@event.Sport.Equals(Sport))
        {
            throw new InvalidOperationException(
                $"Event sport '{@event.Sport}' does not match meet sport '{Sport}'.");
        }
    }

    private static void EnforceStatusInvariant(Event @event, ResultStatus status, Performance? mark, Placement? place)
    {
        if (status == ResultStatus.Finished)
        {
            EnforceFinishedInvariant(@event, mark, place);
        }
        else
        {
            EnforceNonFinishedInvariant(mark, place);
        }
    }

    private static void EnforceFinishedInvariant(Event @event, Performance? mark, Placement? place)
    {
        if (place is null)
        {
            throw new InvalidOperationException("A finished result must have a place.");
        }

        var markKind = @event.Discipline.MarkKind;

        if (markKind == MarkKind.PlaceOnly)
        {
            if (mark is not null)
            {
                throw new InvalidOperationException(
                    "A place-only discipline must not carry a measured mark.");
            }

            return;
        }

        if (mark is null)
        {
            throw new InvalidOperationException(
                "A finished result for a timed or distance discipline must have a mark.");
        }

        EnforceMarkKindMatch(markKind, mark);
    }

    private static void EnforceMarkKindMatch(MarkKind markKind, Performance mark)
    {
        if (markKind == MarkKind.Time && mark is not TimeMark)
        {
            throw new InvalidOperationException(
                $"A time discipline requires a {nameof(TimeMark)}, but received {mark.GetType().Name}.");
        }

        if (markKind == MarkKind.Distance && mark is not DistanceMark)
        {
            throw new InvalidOperationException(
                $"A distance discipline requires a {nameof(DistanceMark)}, but received {mark.GetType().Name}.");
        }
    }

    private static void EnforceNonFinishedInvariant(Performance? mark, Placement? place)
    {
        if (mark is not null)
        {
            throw new InvalidOperationException("A non-finished result must not carry a mark.");
        }

        if (place is not null)
        {
            throw new InvalidOperationException("A non-finished result must not carry a place.");
        }
    }

    private int NextOrderFor(StudentId studentId)
    {
        var existingCount = _results.Count(r => r.StudentId == studentId);
        return existingCount + 1;
    }
}
