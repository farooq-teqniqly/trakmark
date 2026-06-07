namespace Trakmark.Domain.Aggregates;

/// <summary>
/// The completion status of a student's result in a competitive event.
/// Status determines whether a mark and place must, may, or must not be present.
/// </summary>
public enum ResultStatus
{
    /// <summary>The athlete completed the event and a measured outcome is recorded.</summary>
    Finished,

    /// <summary>The athlete started but did not finish (e.g., withdrew mid-race).</summary>
    DidNotFinish,

    /// <summary>The athlete was disqualified from the event.</summary>
    Disqualified,

    /// <summary>The athlete did not start the event.</summary>
    DidNotStart,

    /// <summary>The athlete competed but no mark was recorded (e.g., no valid attempt).</summary>
    NoMark,
}
