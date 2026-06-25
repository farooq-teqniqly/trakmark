using Trakmark.Domain.ValueObjects;

namespace Trakmark.Services;

/// <summary>
/// A single row submitted in a batch-save cities operation, consisting of a city
/// name and the state the city is located in.
/// </summary>
public sealed record SaveCityRow
{
    /// <summary>The city name.</summary>
    public string Name { get; }

    /// <summary>The state the city is located in.</summary>
    public State State { get; }

    /// <summary>Initializes a new <see cref="SaveCityRow"/>.</summary>
    /// <param name="name">The city name. Must not be null.</param>
    /// <param name="state">The state the city is located in. Must not be null.</param>
    public SaveCityRow(string name, State state)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(state);
        Name = name;
        State = state;
    }
}
