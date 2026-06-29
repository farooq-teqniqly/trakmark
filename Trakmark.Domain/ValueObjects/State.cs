namespace Trakmark.Domain.ValueObjects;

/// <summary>
/// A closed set of U.S. states plus the District of Columbia, identified by
/// two-letter abbreviation (e.g. <c>"CA"</c>). Equality is by abbreviation,
/// case-insensitive.
/// </summary>
public sealed class State : IEquatable<State>
{
    /// <summary>The two-letter abbreviation of this state (e.g. <c>"CA"</c>).</summary>
    public string Abbreviation { get; }

    /// <summary>The full name of this state (e.g. <c>"California"</c>).</summary>
    public string Name { get; }

    private State(string abbreviation, string name)
    {
        Abbreviation = abbreviation;
        Name = name;
    }

    /// <summary>Alabama.</summary>
    public static readonly State Alabama = new("AL", "Alabama");

    /// <summary>Alaska.</summary>
    public static readonly State Alaska = new("AK", "Alaska");

    /// <summary>Arizona.</summary>
    public static readonly State Arizona = new("AZ", "Arizona");

    /// <summary>Arkansas.</summary>
    public static readonly State Arkansas = new("AR", "Arkansas");

    /// <summary>California.</summary>
    public static readonly State California = new("CA", "California");

    /// <summary>Colorado.</summary>
    public static readonly State Colorado = new("CO", "Colorado");

    /// <summary>Connecticut.</summary>
    public static readonly State Connecticut = new("CT", "Connecticut");

    /// <summary>Delaware.</summary>
    public static readonly State Delaware = new("DE", "Delaware");

    /// <summary>District of Columbia.</summary>
    public static readonly State DistrictOfColumbia = new("DC", "District of Columbia");

    /// <summary>Florida.</summary>
    public static readonly State Florida = new("FL", "Florida");

    /// <summary>Georgia.</summary>
    public static readonly State Georgia = new("GA", "Georgia");

    /// <summary>Hawaii.</summary>
    public static readonly State Hawaii = new("HI", "Hawaii");

    /// <summary>Idaho.</summary>
    public static readonly State Idaho = new("ID", "Idaho");

    /// <summary>Illinois.</summary>
    public static readonly State Illinois = new("IL", "Illinois");

    /// <summary>Indiana.</summary>
    public static readonly State Indiana = new("IN", "Indiana");

    /// <summary>Iowa.</summary>
    public static readonly State Iowa = new("IA", "Iowa");

    /// <summary>Kansas.</summary>
    public static readonly State Kansas = new("KS", "Kansas");

    /// <summary>Kentucky.</summary>
    public static readonly State Kentucky = new("KY", "Kentucky");

    /// <summary>Louisiana.</summary>
    public static readonly State Louisiana = new("LA", "Louisiana");

    /// <summary>Maine.</summary>
    public static readonly State Maine = new("ME", "Maine");

    /// <summary>Maryland.</summary>
    public static readonly State Maryland = new("MD", "Maryland");

    /// <summary>Massachusetts.</summary>
    public static readonly State Massachusetts = new("MA", "Massachusetts");

    /// <summary>Michigan.</summary>
    public static readonly State Michigan = new("MI", "Michigan");

    /// <summary>Minnesota.</summary>
    public static readonly State Minnesota = new("MN", "Minnesota");

    /// <summary>Mississippi.</summary>
    public static readonly State Mississippi = new("MS", "Mississippi");

    /// <summary>Missouri.</summary>
    public static readonly State Missouri = new("MO", "Missouri");

    /// <summary>Montana.</summary>
    public static readonly State Montana = new("MT", "Montana");

    /// <summary>Nebraska.</summary>
    public static readonly State Nebraska = new("NE", "Nebraska");

    /// <summary>Nevada.</summary>
    public static readonly State Nevada = new("NV", "Nevada");

    /// <summary>New Hampshire.</summary>
    public static readonly State NewHampshire = new("NH", "New Hampshire");

    /// <summary>New Jersey.</summary>
    public static readonly State NewJersey = new("NJ", "New Jersey");

    /// <summary>New Mexico.</summary>
    public static readonly State NewMexico = new("NM", "New Mexico");

    /// <summary>New York.</summary>
    public static readonly State NewYork = new("NY", "New York");

    /// <summary>North Carolina.</summary>
    public static readonly State NorthCarolina = new("NC", "North Carolina");

    /// <summary>North Dakota.</summary>
    public static readonly State NorthDakota = new("ND", "North Dakota");

    /// <summary>Ohio.</summary>
    public static readonly State Ohio = new("OH", "Ohio");

    /// <summary>Oklahoma.</summary>
    public static readonly State Oklahoma = new("OK", "Oklahoma");

    /// <summary>Oregon.</summary>
    public static readonly State Oregon = new("OR", "Oregon");

    /// <summary>Pennsylvania.</summary>
    public static readonly State Pennsylvania = new("PA", "Pennsylvania");

    /// <summary>Rhode Island.</summary>
    public static readonly State RhodeIsland = new("RI", "Rhode Island");

    /// <summary>South Carolina.</summary>
    public static readonly State SouthCarolina = new("SC", "South Carolina");

    /// <summary>South Dakota.</summary>
    public static readonly State SouthDakota = new("SD", "South Dakota");

    /// <summary>Tennessee.</summary>
    public static readonly State Tennessee = new("TN", "Tennessee");

    /// <summary>Texas.</summary>
    public static readonly State Texas = new("TX", "Texas");

    /// <summary>Utah.</summary>
    public static readonly State Utah = new("UT", "Utah");

    /// <summary>Vermont.</summary>
    public static readonly State Vermont = new("VT", "Vermont");

    /// <summary>Virginia.</summary>
    public static readonly State Virginia = new("VA", "Virginia");

    /// <summary>Washington.</summary>
    public static readonly State Washington = new("WA", "Washington");

    /// <summary>West Virginia.</summary>
    public static readonly State WestVirginia = new("WV", "West Virginia");

    /// <summary>Wisconsin.</summary>
    public static readonly State Wisconsin = new("WI", "Wisconsin");

    /// <summary>Wyoming.</summary>
    public static readonly State Wyoming = new("WY", "Wyoming");

    /// <inheritdoc/>
    public bool Equals(State? other) =>
        other is not null
        && string.Equals(Abbreviation, other.Abbreviation, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as State);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        Abbreviation.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns <see langword="true"/> when both states are equal.</summary>
    public static bool operator ==(State? left, State? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>Returns <see langword="true"/> when the states differ.</summary>
    public static bool operator !=(State? left, State? right) => !(left == right);

    /// <inheritdoc/>
    public override string ToString() => Abbreviation;
}
