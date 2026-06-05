namespace Trakmark.Domain.ValueObjects;

/// <summary>A value object wrapping a <see cref="DateOnly"/> that represents the date of a meet.</summary>
public readonly record struct MeetDate
{
    /// <summary>The date of the meet.</summary>
    public DateOnly Value { get; }

    /// <summary>Initializes a <see cref="MeetDate"/> from a <see cref="DateOnly"/>.</summary>
    public MeetDate(DateOnly value) => Value = value;

    /// <inheritdoc/>
    public override string ToString() => Value.ToString("yyyy-MM-dd");
}
