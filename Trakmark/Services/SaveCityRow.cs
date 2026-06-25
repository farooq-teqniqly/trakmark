using Trakmark.Domain.ValueObjects;

namespace Trakmark.Services;

/// <summary>
/// A single row submitted in a batch-save cities operation, consisting of a city
/// name and the state the city is located in.
/// </summary>
/// <param name="Name">The city name.</param>
/// <param name="State">The state the city is located in.</param>
public sealed record SaveCityRow(string Name, State State);
