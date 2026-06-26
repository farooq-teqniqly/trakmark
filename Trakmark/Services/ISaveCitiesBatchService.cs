using Trakmark.Domain.Ids;

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
    /// </summary>
    /// <param name="rows">The city rows to persist. Must not be null; must contain 1–100 items.</param>
    /// <param name="createdByUserId">The <see cref="RegisteredUserId"/> of the submitting Admin.</param>
    Task<SaveCitiesBatchResult> SaveAsync(
        IReadOnlyList<SaveCityRow> rows,
        RegisteredUserId createdByUserId);
}
