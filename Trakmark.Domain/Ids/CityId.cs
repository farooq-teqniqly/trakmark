namespace Trakmark.Domain.Ids;

/// <summary>Strongly-typed identifier for the <c>City</c> aggregate root.</summary>
public readonly record struct CityId
{
    private const string Prefix = "CTY-";

    /// <summary>The raw string value of this identifier.</summary>
    public string Value { get; }

    private CityId(string value) => Value = value;

    /// <summary>A sentinel empty identifier.</summary>
    public static readonly CityId Empty = new(Prefix + new string('A', DomainId.BodyLength));

    /// <summary>Generates a new, effectively unique <see cref="CityId"/>.</summary>
    public static CityId NewId() => new(DomainId.NewValue(Prefix));

    /// <summary>Parses <paramref name="value"/> as a <see cref="CityId"/>.</summary>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is ill-formed.</exception>
    public static CityId Parse(string value) =>
        TryParse(value, out var id) ? id : throw new FormatException($"Invalid CityId: '{value}'.");

    /// <summary>
    /// Attempts to parse <paramref name="value"/> as a <see cref="CityId"/>.
    /// Returns <see langword="false"/> when the value is ill-formed.
    /// </summary>
    public static bool TryParse(string value, out CityId id)
    {
        if (DomainId.IsValid(value, Prefix))
        {
            id = new CityId(value);
            return true;
        }
        id = default;
        return false;
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
