namespace Trakmark.Data.Entities;

/// <summary>
/// EF Core persistence entity for a <see cref="Domain.ValueObjects.City"/>, carrying
/// audit metadata (<see cref="CreatedAt"/>, <see cref="CreatedByUserId"/>) that is
/// not part of the domain model.
/// </summary>
public sealed class CityEntity
{
    /// <summary>
    /// The DB-generated surrogate key used as the clustered primary key.
    /// This is a persistence-only implementation detail, never exposed outside
    /// the EF Core configuration/entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>The domain <c>CityId</c> value, mapped as a unique alternate key.</summary>
    public string CityId { get; set; } = null!;

    /// <summary>The city's name.</summary>
    public string Name { get; set; } = null!;

    /// <summary>The two-letter abbreviation of the state the city is located in.</summary>
    public string State { get; set; } = null!;

    /// <summary>The timestamp at which this city was persisted.</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>The <c>RegisteredUserId</c> value of the user who created this city.</summary>
    public string CreatedByUserId { get; set; } = null!;
}
