namespace Trakmark.Domain.Catalog;

/// <summary>
/// A structured controlled-vocabulary entry describing a specific competitive event.
/// Identity includes all setup parameters (e.g., hurdle height, implement weight)
/// so that "110m hurdles @ 39"" and "110m hurdles @ 33"" are distinct disciplines
/// and personal bests are segmented correctly.
/// </summary>
public sealed class Discipline : IEquatable<Discipline>
{
    /// <summary>The human-readable name of this discipline (e.g., "100m Hurdles @ 39\"").</summary>
    public string Name { get; }

    /// <summary>The kind of mark this discipline produces.</summary>
    public MarkKind MarkKind { get; }

    /// <summary>Whether this is a relay event.</summary>
    public bool IsRelay { get; }

    private readonly string _identityKey;

    private Discipline(string name, MarkKind markKind, bool isRelay, string identityKey)
    {
        Name = name;
        MarkKind = markKind;
        IsRelay = isRelay;
        _identityKey = identityKey;
    }

    /// <summary>
    /// Creates a plain running discipline (no setup parameters beyond distance).
    /// </summary>
    /// <param name="distanceMeters">The race distance in meters.</param>
    public static Discipline Run(int distanceMeters) =>
        new($"{distanceMeters}m", MarkKind.Time, false, $"run:{distanceMeters}");

    /// <summary>
    /// Creates a relay running discipline.
    /// </summary>
    /// <param name="distanceMeters">The relay distance in meters.</param>
    public static Discipline RelayRun(int distanceMeters) =>
        new($"{distanceMeters}m Relay", MarkKind.Time, true, $"relay:{distanceMeters}");

    /// <summary>
    /// Creates a hurdle-run discipline. Hurdle height is part of identity.
    /// </summary>
    /// <param name="distanceMeters">The race distance in meters.</param>
    /// <param name="height">The hurdle height setup parameter.</param>
    public static Discipline HurdleRun(int distanceMeters, HurdleHeight height)
    {
        ArgumentNullException.ThrowIfNull(height);
        return new(
            $"{distanceMeters}m Hurdles @ {height}",
            MarkKind.Time,
            false,
            $"hurdle:{distanceMeters}:{height.Name}"
        );
    }

    /// <summary>
    /// Creates a field-throw discipline. Implement weight is part of identity.
    /// </summary>
    /// <param name="eventName">The throw event name (e.g., "Shot Put", "Discus").</param>
    /// <param name="weight">The implement weight setup parameter.</param>
    public static Discipline ImplementThrow(string eventName, ImplementWeight weight)
    {
        ArgumentNullException.ThrowIfNull(eventName);
        ArgumentNullException.ThrowIfNull(weight);
        return new(
            $"{eventName} @ {weight}",
            MarkKind.Distance,
            false,
            $"throw:{eventName}:{weight.Name}"
        );
    }

    /// <summary>
    /// Creates a plain field-jump discipline (e.g., Long Jump, High Jump).
    /// </summary>
    /// <param name="eventName">The jump event name.</param>
    public static Discipline Jump(string eventName)
    {
        ArgumentNullException.ThrowIfNull(eventName);
        return new(eventName, MarkKind.Distance, false, $"jump:{eventName}");
    }

    /// <summary>
    /// Creates a place-only discipline (e.g., a combined-points event where only placement is recorded).
    /// </summary>
    /// <param name="eventName">The event name.</param>
    public static Discipline PlaceOnly(string eventName)
    {
        ArgumentNullException.ThrowIfNull(eventName);
        return new(eventName, MarkKind.PlaceOnly, false, $"placeonly:{eventName}");
    }

    /// <inheritdoc/>
    public bool Equals(Discipline? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(_identityKey, other._identityKey, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Discipline);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        _identityKey.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns <see langword="true"/> when both disciplines are equal.</summary>
    public static bool operator ==(Discipline? left, Discipline? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the disciplines differ.</summary>
    public static bool operator !=(Discipline? left, Discipline? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
