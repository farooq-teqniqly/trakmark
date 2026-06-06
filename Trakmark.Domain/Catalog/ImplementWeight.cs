using System.Runtime.CompilerServices;

namespace Trakmark.Domain.Catalog;

/// <summary>
/// A closed set of implement weights used as a setup parameter for throwing disciplines.
/// Implement weight is part of discipline identity: a 4 kg shot put and a 6 kg shot put are
/// distinct disciplines so personal bests are segmented correctly.
/// </summary>
public sealed class ImplementWeight : IEquatable<ImplementWeight>
{
    /// <summary>The display name of this implement weight.</summary>
    public string Name { get; }

    private ImplementWeight(string name) => Name = name;

    /// <summary>4 kg implement (standard women's and youth).</summary>
    public static readonly ImplementWeight Kg4 = new("4 kg");

    /// <summary>6 kg implement (standard men's HS).</summary>
    public static readonly ImplementWeight Kg6 = new("6 kg");

    /// <summary>7.26 kg implement (standard men's college/open).</summary>
    public static readonly ImplementWeight Kg7_26 = new("7.26 kg");

    /// <inheritdoc/>
    public bool Equals(ImplementWeight? other) => ReferenceEquals(this, other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ImplementWeight);

    /// <inheritdoc/>
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
