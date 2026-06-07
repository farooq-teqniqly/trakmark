namespace Trakmark.Domain.Ids;

/// <summary>Strongly-typed identifier for the <c>Meet</c> aggregate root.</summary>
public readonly record struct MeetId
{
    private const string Prefix = "MEET-";

    /// <summary>The raw string value of this identifier.</summary>
    public string Value { get; }

    private MeetId(string value) => Value = value;

    /// <summary>A sentinel empty identifier.</summary>
    public static readonly MeetId Empty = new(Prefix + new string('A', DomainId.BodyLength));

    /// <summary>Generates a new, effectively unique <see cref="MeetId"/>.</summary>
    public static MeetId NewId() => new(DomainId.NewValue(Prefix));

    /// <summary>Parses <paramref name="value"/> as a <see cref="MeetId"/>.</summary>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is ill-formed.</exception>
    public static MeetId Parse(string value) =>
        TryParse(value, out var id) ? id : throw new FormatException($"Invalid MeetId: '{value}'.");

    /// <summary>
    /// Attempts to parse <paramref name="value"/> as a <see cref="MeetId"/>.
    /// Returns <see langword="false"/> when the value is ill-formed.
    /// </summary>
    public static bool TryParse(string value, out MeetId id)
    {
        if (DomainId.IsValid(value, Prefix))
        {
            id = new MeetId(value);
            return true;
        }
        id = default;
        return false;
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
