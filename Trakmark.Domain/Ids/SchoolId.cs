namespace Trakmark.Domain.Ids;

/// <summary>Strongly-typed identifier for the <c>School</c> aggregate root.</summary>
public readonly record struct SchoolId
{
    private const string Prefix = "SCH-";

    /// <summary>The raw string value of this identifier.</summary>
    public string Value { get; }

    private SchoolId(string value) => Value = value;

    /// <summary>A sentinel empty identifier.</summary>
    public static readonly SchoolId Empty = new(Prefix + new string('A', DomainId.BodyLength));

    /// <summary>Generates a new, effectively unique <see cref="SchoolId"/>.</summary>
    public static SchoolId NewId() => new(DomainId.NewValue(Prefix));

    /// <summary>Parses <paramref name="value"/> as a <see cref="SchoolId"/>.</summary>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is ill-formed.</exception>
    public static SchoolId Parse(string value) =>
        TryParse(value, out var id) ? id : throw new FormatException($"Invalid SchoolId: '{value}'.");

    /// <summary>
    /// Attempts to parse <paramref name="value"/> as a <see cref="SchoolId"/>.
    /// Returns <see langword="false"/> when the value is ill-formed.
    /// </summary>
    public static bool TryParse(string value, out SchoolId id)
    {
        if (DomainId.IsValid(value, Prefix))
        {
            id = new SchoolId(value);
            return true;
        }
        id = default;
        return false;
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
