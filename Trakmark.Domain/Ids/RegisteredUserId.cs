namespace Trakmark.Domain.Ids;

/// <summary>Strongly-typed identifier for the <c>RegisteredUser</c> aggregate root.</summary>
public readonly record struct RegisteredUserId
{
    private const string Prefix = "USR-";

    /// <summary>The raw string value of this identifier.</summary>
    public string Value { get; }

    private RegisteredUserId(string value) => Value = value;

    /// <summary>A sentinel empty identifier.</summary>
    public static readonly RegisteredUserId Empty = new(Prefix + new string('A', DomainId.BodyLength));

    /// <summary>Generates a new, effectively unique <see cref="RegisteredUserId"/>.</summary>
    public static RegisteredUserId NewId() => new(DomainId.NewValue(Prefix));

    /// <summary>Parses <paramref name="value"/> as a <see cref="RegisteredUserId"/>.</summary>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is ill-formed.</exception>
    public static RegisteredUserId Parse(string value) =>
        TryParse(value, out var id) ? id : throw new FormatException($"Invalid RegisteredUserId: '{value}'.");

    /// <summary>
    /// Attempts to parse <paramref name="value"/> as a <see cref="RegisteredUserId"/>.
    /// Returns <see langword="false"/> when the value is ill-formed.
    /// </summary>
    public static bool TryParse(string value, out RegisteredUserId id)
    {
        if (DomainId.IsValid(value, Prefix))
        {
            id = new RegisteredUserId(value);
            return true;
        }
        id = default;
        return false;
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
