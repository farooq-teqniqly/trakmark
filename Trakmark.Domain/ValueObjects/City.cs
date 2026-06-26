using Trakmark.Domain.Ids;

namespace Trakmark.Domain.ValueObjects;

/// <summary>
/// A city with a required name and state. Equality is by <see cref="Name"/>
/// (case-insensitive) and <see cref="State"/>.
/// </summary>
public sealed class City : IEquatable<City>
{
    private const int MaxNameLength = 100;

    /// <summary>The unique identifier for this city.</summary>
    public CityId Id { get; }

    /// <summary>The trimmed name of this city.</summary>
    public string Name { get; }

    /// <summary>The state this city is located in.</summary>
    public State State { get; }

    private City(CityId id, string name, State state)
    {
        Id = id;
        Name = name;
        State = state;
    }

    /// <summary>
    /// Creates a new <see cref="City"/> with a generated <see cref="CityId"/>.
    /// </summary>
    /// <param name="name">The city's name. Must be non-empty and at most 100 characters after trimming.</param>
    /// <param name="state">The state the city is located in.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="state"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is empty, whitespace, or exceeds 100 characters.</exception>
    public static City Create(string name, State state)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(state);

        var trimmed = name.Trim();
        if (trimmed.Length == 0)
        {
            throw new ArgumentException(
                "City name must not be empty or whitespace.",
                nameof(name)
            );
        }

        if (trimmed.Length > MaxNameLength)
        {
            throw new ArgumentException(
                $"City name must not exceed {MaxNameLength} characters.",
                nameof(name)
            );
        }

        return new(CityId.NewId(), trimmed, state);
    }

    /// <inheritdoc/>
    public bool Equals(City? other) =>
        other is not null
        && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
        && State == other.State;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as City);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(Name.GetHashCode(StringComparison.OrdinalIgnoreCase), State);

    /// <inheritdoc/>
    public override string ToString() => $"{Name}, {State}";

    /// <summary>Returns true when both instances are equal.</summary>
    public static bool operator ==(City? left, City? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>Returns true when both instances are not equal.</summary>
    public static bool operator !=(City? left, City? right) => !(left == right);
}
