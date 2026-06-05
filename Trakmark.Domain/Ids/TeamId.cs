namespace Trakmark.Domain.Ids;

/// <summary>Strongly-typed identifier for the <c>Team</c> entity.</summary>
public readonly record struct TeamId
{
    private const string Prefix = "TEAM-";

    /// <summary>The raw string value of this identifier.</summary>
    public string Value { get; }

    private TeamId(string value) => Value = value;

    /// <summary>A sentinel empty identifier.</summary>
    public static readonly TeamId Empty = new(Prefix + new string('A', DomainId.BodyLength));

    /// <summary>Generates a new, effectively unique <see cref="TeamId"/>.</summary>
    public static TeamId NewId() => new(DomainId.NewValue(Prefix));

    /// <summary>Parses <paramref name="value"/> as a <see cref="TeamId"/>.</summary>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is ill-formed.</exception>
    public static TeamId Parse(string value) =>
        TryParse(value, out var id) ? id : throw new FormatException($"Invalid TeamId: '{value}'.");

    /// <summary>
    /// Attempts to parse <paramref name="value"/> as a <see cref="TeamId"/>.
    /// Returns <see langword="false"/> when the value is ill-formed.
    /// </summary>
    public static bool TryParse(string value, out TeamId id)
    {
        if (DomainId.IsValid(value, Prefix))
        {
            id = new TeamId(value);
            return true;
        }
        id = default;
        return false;
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
