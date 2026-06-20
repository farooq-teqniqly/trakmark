namespace Trakmark.Domain.Ids;

/// <summary>Strongly-typed identifier for the <c>Student</c> aggregate root.</summary>
public readonly record struct StudentId
{
    private const string Prefix = "STU-";

    /// <summary>The raw string value of this identifier.</summary>
    public string Value { get; }

    private StudentId(string value) => Value = value;

    /// <summary>A sentinel empty identifier.</summary>
    public static readonly StudentId Empty = new(Prefix + new string('A', DomainId.BodyLength));

    /// <summary>Generates a new, effectively unique <see cref="StudentId"/>.</summary>
    public static StudentId NewId() => new(DomainId.NewValue(Prefix));

    /// <summary>Parses <paramref name="value"/> as a <see cref="StudentId"/>.</summary>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is ill-formed.</exception>
    public static StudentId Parse(string value) =>
        TryParse(value, out var id)
            ? id
            : throw new FormatException($"Invalid StudentId: '{value}'.");

    /// <summary>
    /// Attempts to parse <paramref name="value"/> as a <see cref="StudentId"/>.
    /// Returns <see langword="false"/> when the value is ill-formed.
    /// </summary>
    public static bool TryParse(string value, out StudentId id)
    {
        if (DomainId.IsValid(value, Prefix))
        {
            id = new StudentId(value);
            return true;
        }
        id = default;
        return false;
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
