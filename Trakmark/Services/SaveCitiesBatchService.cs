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
    /// <param name="rows">The city rows to persist. Must not be null.</param>
    /// <param name="createdByUserId">The <see cref="RegisteredUserId"/> of the submitting Admin.</param>
    public async Task<SaveCitiesBatchResult> SaveAsync(
        IReadOnlyList<SaveCityRow> rows,
        RegisteredUserId createdByUserId)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var validationResult = ValidateRows(rows);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var cities = BuildCities(rows);

        var inBatchDuplicate = FindInBatchDuplicate(cities);
        if (inBatchDuplicate is not null)
        {
            return inBatchDuplicate;
        }

        var crossBatchDuplicate = await FindCrossBatchDuplicateAsync(cities);
        if (crossBatchDuplicate is not null)
        {
            return crossBatchDuplicate;
        }

        await PersistAsync(cities, createdByUserId);
        return new SaveCitiesBatchResult.Success(cities.Count);
    }

    private static SaveCitiesBatchResult.ValidationError? ValidateRows(IReadOnlyList<SaveCityRow> rows)
    {
        foreach (var row in rows)
        {
            try
            {
                City.Create(row.Name, row.State);
            }
            catch (ArgumentException ex)
            {
                return new SaveCitiesBatchResult.ValidationError(ex.Message);
            }
        }

        return null;
    }

    private static List<City> BuildCities(IReadOnlyList<SaveCityRow> rows)
    {
        var cities = new List<City>(rows.Count);

        foreach (var row in rows)
        {
            cities.Add(City.Create(row.Name, row.State));
        }

        return cities;
    }

    private static SaveCitiesBatchResult.InBatchDuplicate? FindInBatchDuplicate(List<City> cities)
    {
        var seen = new HashSet<City>();

        foreach (var city in cities)
        {
            if (!seen.Add(city))
            {
                return new SaveCitiesBatchResult.InBatchDuplicate(
                    city.Name,
                    city.State.Abbreviation
                );
            }
        }

        return null;
    }

    private async Task<SaveCitiesBatchResult.CrossBatchDuplicate?> FindCrossBatchDuplicateAsync(
        List<City> cities)
    {
        foreach (var city in cities)
        {
            var nameUpper = city.Name.ToUpperInvariant();
            var abbr = city.State.Abbreviation.ToUpperInvariant();

            var exists = await _context.Cities.AnyAsync(e =>
                e.Name.ToUpper() == nameUpper &&
                e.State.ToUpper() == abbr
            );

            if (exists)
            {
                return new SaveCitiesBatchResult.CrossBatchDuplicate(
                    city.Name,
                    city.State.Abbreviation
                );
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
