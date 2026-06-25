using Microsoft.EntityFrameworkCore;
using Trakmark.Data;
using Trakmark.Data.Entities;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;

namespace Trakmark.Services;

/// <summary>
/// Validates and persists a batch of cities in a single all-or-nothing transaction.
/// Accepts the submitting Admin's <see cref="RegisteredUserId"/> as a parameter to
/// keep the service testable without an HTTP context dependency.
/// </summary>
public sealed class SaveCitiesBatchService
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new <see cref="SaveCitiesBatchService"/>.</summary>
    public SaveCitiesBatchService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Validates and persists <paramref name="rows"/> as an all-or-nothing batch.
    /// Returns a <see cref="SaveCitiesBatchResult"/> discriminating between success,
    /// validation failure, in-batch duplicate, and cross-batch (existing) duplicate.
    /// </summary>
    /// <param name="rows">The city rows to persist. Must not be null; must contain 1-100 items.</param>
    /// <param name="createdByUserId">The <see cref="RegisteredUserId"/> of the submitting Admin.</param>
    public async Task<SaveCitiesBatchResult> SaveAsync(
        IReadOnlyList<SaveCityRow> rows,
        RegisteredUserId createdByUserId)
    {
        ArgumentNullException.ThrowIfNull(rows);

        if (rows.Count == 0 || rows.Count > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(rows), "Batch must contain between 1 and 100 rows.");
        }

        var (cities, validationError) = BuildAndValidate(rows);
        if (validationError is not null)
        {
            return validationError;
        }

        var inBatchDuplicate = FindInBatchDuplicate(cities!);
        if (inBatchDuplicate is not null)
        {
            return inBatchDuplicate;
        }

        var crossBatchDuplicate = await FindCrossBatchDuplicateAsync(cities!);
        if (crossBatchDuplicate is not null)
        {
            return crossBatchDuplicate;
        }

        await PersistAsync(cities!, createdByUserId);
        return new SaveCitiesBatchResult.Success(cities!.Count);
    }

    private static (List<City>? Cities, SaveCitiesBatchResult.ValidationError? Error) BuildAndValidate(
        IReadOnlyList<SaveCityRow> rows)
    {
        var cities = new List<City>(rows.Count);

        foreach (var row in rows)
        {
            try
            {
                cities.Add(City.Create(row.Name, row.State));
            }
            catch (ArgumentException ex)
            {
                return (null, new SaveCitiesBatchResult.ValidationError(ex.Message));
            }
        }

        return (cities, null);
    }

    private static SaveCitiesBatchResult.InBatchDuplicate? FindInBatchDuplicate(List<City> cities)
    {
        var seen = new HashSet<City>();
        var duplicate = cities.FirstOrDefault(city => !seen.Add(city));

        return duplicate is null
            ? null
            : new SaveCitiesBatchResult.InBatchDuplicate(duplicate.Name, duplicate.State.Abbreviation);
    }

    private async Task<SaveCitiesBatchResult.CrossBatchDuplicate?> FindCrossBatchDuplicateAsync(
        List<City> cities)
    {
        var batchNames = new HashSet<string>(
            cities.Select(c => c.Name.ToUpperInvariant()),
            StringComparer.Ordinal);

        var batchAbbrs = new HashSet<string>(
            cities.Select(c => c.State.Abbreviation.ToUpperInvariant()),
            StringComparer.Ordinal);

        var existing = await _context.Cities
            .Where(e => batchNames.Contains(e.Name.ToUpper()) && batchAbbrs.Contains(e.State.ToUpper()))
            .ToListAsync();

        if (existing.Count == 0)
        {
            return null;
        }

        foreach (var city in cities)
        {
            var nameUpper = city.Name.ToUpperInvariant();
            var abbrUpper = city.State.Abbreviation.ToUpperInvariant();

            if (existing.Any(e =>
                    e.Name.ToUpperInvariant() == nameUpper &&
                    e.State.ToUpperInvariant() == abbrUpper))
            {
                return new SaveCitiesBatchResult.CrossBatchDuplicate(city.Name, city.State.Abbreviation);
            }
        }

        return null;
    }

    private async Task PersistAsync(List<City> cities, RegisteredUserId createdByUserId)
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var city in cities)
        {
            _context.Cities.Add(new CityEntity
            {
                CityId = city.Id.Value,
                Name = city.Name,
                State = city.State.Abbreviation,
                CreatedAt = now,
                CreatedByUserId = createdByUserId.Value,
            });
        }

        await _context.SaveChangesAsync();
    }
}
