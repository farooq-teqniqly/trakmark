using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Trakmark.Services;

namespace Trakmark.Data;

/// <summary>
/// EF Core <see cref="SaveChangesInterceptor"/> that stamps <see cref="IAuditableEntity.CreatedByUserId"/>
/// and <see cref="IAuditableEntity.CreatedAt"/> on every <c>Added</c> entity that implements
/// <see cref="IAuditableEntity"/>, using the resolved user from <see cref="ICurrentUserContext"/>.
/// Throws <see cref="InvalidOperationException"/> when <see cref="ICurrentUserContext.UserId"/>
/// is <see langword="null"/> and there are auditable entities to stamp.
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserContext _userContext;

    /// <summary>Initializes a new instance of <see cref="AuditInterceptor"/>.</summary>
    public AuditInterceptor(ICurrentUserContext userContext)
    {
        _userContext = userContext;
    }

    /// <inheritdoc/>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        ArgumentNullException.ThrowIfNull(eventData);
        StampAuditableEntries(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(eventData);
        StampAuditableEntries(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void StampAuditableEntries(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var auditableEntries = context
            .ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added && e.Entity is IAuditableEntity)
            .ToList();

        if (auditableEntries.Count is 0)
        {
            return;
        }

        var partiallyStamped = auditableEntries
            .Select(e => (IAuditableEntity)e.Entity)
            .Where(e => string.IsNullOrEmpty(e.CreatedByUserId) != (e.CreatedAt == default))
            .ToList();

        if (partiallyStamped.Count > 0)
        {
            throw new InvalidOperationException(
                "One or more auditable entities have only one audit field pre-stamped. Both CreatedByUserId and CreatedAt must be set together or left unset."
            );
        }

        var unstampedEntries = auditableEntries
            .Where(e => string.IsNullOrEmpty(((IAuditableEntity)e.Entity).CreatedByUserId))
            .ToList();

        if (unstampedEntries.Count is 0)
        {
            return;
        }

        if (_userContext.UserId is null)
        {
            throw new InvalidOperationException(
                "ICurrentUserContext.UserId is null. An authorized component must set UserId before SaveChanges is called."
            );
        }

        var userId = _userContext.UserId.Value.Value;
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in unstampedEntries)
        {
            var entity = (IAuditableEntity)entry.Entity;
            entity.CreatedByUserId = userId;
            entity.CreatedAt = now;
        }
    }
}
