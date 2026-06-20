namespace Trakmark.Domain.ValueObjects;

/// <summary>
/// A closed set representing a student's grade level.
/// Use the static factory members; do not construct directly.
/// </summary>
public sealed class GradeLevel : IEquatable<GradeLevel>
{
    /// <summary>The display name of this grade level.</summary>
    public string Name { get; }

    private GradeLevel(string name) => Name = name;

    /// <summary>9th grade (high-school freshman).</summary>
    public static readonly GradeLevel Freshman = new("Freshman");

    /// <summary>10th grade.</summary>
    public static readonly GradeLevel Sophomore = new("Sophomore");

    /// <summary>11th grade.</summary>
    public static readonly GradeLevel Junior = new("Junior");

    /// <summary>12th grade.</summary>
    public static readonly GradeLevel Senior = new("Senior");

    /// <summary>8th grade (middle school).</summary>
    public static readonly GradeLevel MiddleSchool8th = new("8th Grade");

    /// <summary>7th grade (middle school).</summary>
    public static readonly GradeLevel MiddleSchool7th = new("7th Grade");

    /// <summary>6th grade (middle school / upper elementary).</summary>
    public static readonly GradeLevel Grade6 = new("6th Grade");

    /// <inheritdoc/>
    public bool Equals(GradeLevel? other) => other is not null && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as GradeLevel);

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns <see langword="true"/> when both grade levels are equal.</summary>
    public static bool operator ==(GradeLevel? left, GradeLevel? right) => left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the grade levels differ.</summary>
    public static bool operator !=(GradeLevel? left, GradeLevel? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
