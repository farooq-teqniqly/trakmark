namespace Trakmark.Data;

/// <summary>
/// Marks an EF Core entity as carrying creation audit metadata.
/// <see cref="AuditInterceptor"/> stamps <see cref="CreatedByUserId"/> and
/// <see cref="CreatedAt"/> automatically on every <c>Added</c> entity that
/// implements this interface, eliminating manual stamping in services.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>The <c>RegisteredUserId</c> value of the user who created this entity.</summary>
    string CreatedByUserId { get; set; }

    /// <summary>The UTC timestamp at which this entity was first persisted.</summary>
    DateTimeOffset CreatedAt { get; set; }
}
