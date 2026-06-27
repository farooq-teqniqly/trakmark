namespace Trakmark.Data.Entities;

/// <summary>
/// EF Core persistence entity for a <see cref="Trakmark.Domain.Aggregates.RegisteredUser"/>,
/// mapping the domain aggregate's identity and account linkage to a relational row.
/// </summary>
public sealed class RegisteredUserEntity
{
    /// <summary>
    /// The domain <c>RegisteredUserId</c> value (format <c>USR-XXXXXX</c>, 10 chars),
    /// used as the primary key.
    /// </summary>
    public string RegisteredUserId { get; set; } = null!;

    /// <summary>
    /// The ASP.NET Identity user ID (<c>AccountId</c>) linked to this registered user.
    /// Max length 450 for ASP.NET Identity GUID compatibility.
    /// </summary>
    public string AccountId { get; set; } = null!;
}
