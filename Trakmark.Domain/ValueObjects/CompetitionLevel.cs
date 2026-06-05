namespace Trakmark.Domain.ValueObjects;

/// <summary>
/// A closed set representing the level of competition for a school or meet
/// (High School, Middle School, or Elementary).
/// </summary>
public sealed class CompetitionLevel : IEquatable<CompetitionLevel>
{
    /// <summary>The display name of this competition level.</summary>
    public string Name { get; }

    private CompetitionLevel(string name) => Name = name;

    /// <summary>High school competition level.</summary>
    public static readonly CompetitionLevel HighSchool = new("High School");

    /// <summary>Middle school competition level.</summary>
    public static readonly CompetitionLevel MiddleSchool = new("Middle School");

    /// <summary>Elementary school competition level.</summary>
    public static readonly CompetitionLevel Elementary = new("Elementary");

    /// <inheritdoc/>
    public bool Equals(CompetitionLevel? other) => ReferenceEquals(this, other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as CompetitionLevel);

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode(StringComparison.Ordinal);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
