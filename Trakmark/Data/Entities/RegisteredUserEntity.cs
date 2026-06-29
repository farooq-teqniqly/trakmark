using Trakmark.Data;

namespace Trakmark.Data.Entities;

/// <summary>
/// EF Core persistence entity for a <see cref="Trakmark.Domain.Aggregates.RegisteredUser"/>,
/// mapping the domain aggregate's identity and account linkage to a relational row.
/// </summary>
public sealed class RegisteredUserEntity : IAuditableEntity
{
    /// <summary>
    /// The DB-generated surrogate key used as the clustered primary key.
    /// This is a persistence-only implementation detail, never exposed outside
    /// the EF Core configuration/entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The domain <c>RegisteredUserId</c> value (format <c>USR-XXXXXX</c>, 10 chars),
    /// mapped as a unique alternate key.
    /// </summary>
    public string RegisteredUserId { get; set; } = null!;

    /// <summary>
    /// The ASP.NET Identity user ID (<c>AccountId</c>) linked to this registered user.
    /// Max length 450 for ASP.NET Identity GUID compatibility.
    /// </summary>
    public string AccountId { get; set; } = null!;

    /// <inheritdoc/>
    public string CreatedByUserId { get; set; } = null!;

    /// <inheritdoc/>
    public DateTimeOffset CreatedAt { get; set; }
}
