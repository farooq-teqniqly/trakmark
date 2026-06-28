namespace Trakmark.Services;

/// <summary>
/// Validates and persists a batch of cities in a single all-or-nothing transaction.
/// </summary>
public interface ISaveCitiesBatchService
{
    /// <summary>
    /// Validates and persists <paramref name="rows"/> as an all-or-nothing batch.
    /// Returns a <see cref="SaveCitiesBatchResult"/> discriminating between success,
    /// validation failure, in-batch duplicate, and cross-batch (existing) duplicate.
    /// Creation metadata is stamped by <see cref="Data.AuditInterceptor"/> via
    /// <see cref="ICurrentUserContext"/> — do not pass or stamp a user ID manually.
    /// </summary>
    /// <param name="rows">The city rows to persist. Must not be null; must contain 1–100 items.</param>
    Task<SaveCitiesBatchResult> SaveAsync(IReadOnlyList<SaveCityRow> rows);
}
