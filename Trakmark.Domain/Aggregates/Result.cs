using Trakmark.Domain.Catalog;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Domain.Aggregates;

/// <summary>
/// An entity recording a single student's outcome in one event at a meet.
/// The <see cref="Status"/> governs whether a <see cref="Mark"/> and
/// <see cref="Place"/> are present: only a <see cref="ResultStatus.Finished"/>
/// result carries them, and all other statuses must carry neither.
/// <see cref="Order"/> records the entry sequence of this result among the
/// student's results within the meet.
/// <see cref="MeetDate"/> is carried on the result so that read projections
/// can resolve the <see cref="SchoolYear"/> without requiring a separate join to
/// the owning <see cref="Meet"/>.
/// </summary>
public sealed class Result
{
    /// <summary>The student this result belongs to.</summary>
    public StudentId StudentId { get; }

    /// <summary>The event in which the result was recorded.</summary>
    public Event Event { get; }

    /// <summary>The completion status of this result.</summary>
    public ResultStatus Status { get; }

    /// <summary>
    /// The measured performance mark, present only when <see cref="Status"/> is
    /// <see cref="ResultStatus.Finished"/> and the discipline is not place-only.
    /// </summary>
    public Performance? Mark { get; }

    /// <summary>
    /// The finish placement, present only when <see cref="Status"/> is
    /// <see cref="ResultStatus.Finished"/>.
    /// </summary>
    public Placement? Place { get; }

    /// <summary>
    /// The competitive tier this result was set in. Defaults to <see cref="Tier.Open"/>
    /// when not supplied. Tier does not affect personal-best or season-best eligibility.
    /// </summary>
    public Tier Tier { get; }

    /// <summary>
    /// The 1-based entry sequence of this result among the student's results for
    /// this meet. Lower values were recorded earlier.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// The date of the meet at which this result was recorded.
    /// Carried here so that season-view projections can resolve the
    /// <see cref="SchoolYear"/> without a separate join to the owning meet.
    /// </summary>
    public MeetDate MeetDate { get; }

    internal Result(
        StudentId studentId,
        Event @event,
        ResultStatus status,
        Performance? mark,
        Placement? place,
        Tier tier,
        int order,
        MeetDate meetDate)
    {
        StudentId = studentId;
        Event = @event;
        Status = status;
        Mark = mark;
        Place = place;
        Tier = tier;
        Order = order;
        MeetDate = meetDate;
    }
}
