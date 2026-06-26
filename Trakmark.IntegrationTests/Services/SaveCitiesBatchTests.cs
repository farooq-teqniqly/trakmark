using Microsoft.EntityFrameworkCore;
using Trakmark.Data.Entities;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;
using Trakmark.Services;

namespace Trakmark.IntegrationTests.Services;

/// <summary>
/// Integration tests for <see cref="SaveCitiesBatchService"/> covering all batch-save
/// outcome scenarios: success, validation error, in-batch duplicate, cross-batch
/// duplicate, and CreatedByUserId propagation.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class SaveCitiesBatchTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;

    /// <summary>Initializes a new instance of <see cref="SaveCitiesBatchTests"/>.</summary>
    public SaveCitiesBatchTests(DatabaseFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync() => await _fixture.ResetAsync();

    /// <inheritdoc/>
    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task Batch_outside_1_to_100_rows_throws(int count)
    {
        // Arrange
        var adminId = RegisteredUserId.NewId();
        var rows = Enumerable
            .Range(0, count)
            .Select(i => new SaveCityRow($"City{i}", State.Illinois))
            .ToList();

        await using var context = _fixture.CreateContext();
        var service = new SaveCitiesBatchService(context);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.SaveAsync(rows, adminId)
        );
    }

    [Fact]
    public async Task All_valid_batch_persists_all_rows()
    {
        // Arrange
        var adminId = RegisteredUserId.NewId();
        var rows = new List<SaveCityRow>
        {
            new("Springfield", State.Illinois),
            new("Chicago", State.Illinois),
        };

        await using var context = _fixture.CreateContext();
        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows, adminId);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.Success>(result);
        await using var readContext = _fixture.CreateContext();
        var count = await readContext.Cities.CountAsync(c => c.State == "IL");
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task One_invalid_row_rejects_whole_batch()
    {
        // Arrange
        var adminId = RegisteredUserId.NewId();
        var rows = new List<SaveCityRow>
        {
            new("Springfield", State.Illinois),
            new("   ", State.Illinois),
        };

        await using var context = _fixture.CreateContext();
        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows, adminId);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.ValidationError>(result);
        await using var readContext = _fixture.CreateContext();
        Assert.Equal(0, await readContext.Cities.CountAsync());
    }

    [Fact]
    public async Task InBatch_duplicate_rejects_whole_batch()
    {
        // Arrange
        var adminId = RegisteredUserId.NewId();
        var rows = new List<SaveCityRow>
        {
            new("Springfield", State.Illinois),
            new("springfield", State.Illinois),
        };

        await using var context = _fixture.CreateContext();
        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows, adminId);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.InBatchDuplicate>(result);
        await using var readContext = _fixture.CreateContext();
        Assert.Equal(0, await readContext.Cities.CountAsync());
    }

    [Fact]
    public async Task CrossBatch_duplicate_rejects_whole_batch()
    {
        // Arrange — pre-seed an existing city
        await using (var seedContext = _fixture.CreateContext())
        {
            seedContext.Cities.Add(
                new CityEntity
                {
                    CityId = "CTY-BATCHSEED1",
                    Name = "Springfield",
                    State = "IL",
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedByUserId = "USR-BATCHSEED1",
                }
            );

            await seedContext.SaveChangesAsync();
        }

        var adminId = RegisteredUserId.NewId();
        var rows = new List<SaveCityRow> { new("Springfield", State.Illinois) };

        await using var context = _fixture.CreateContext();
        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows, adminId);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.CrossBatchDuplicate>(result);
        await using var readContext = _fixture.CreateContext();
        Assert.Equal(1, await readContext.Cities.CountAsync());
    }

    [Fact]
    public async Task Persisted_rows_carry_submitting_admin_registered_user_id_as_created_by_user_id()
    {
        // Arrange
        var adminId = RegisteredUserId.NewId();
        var rows = new List<SaveCityRow> { new("Decatur", State.Illinois) };

        await using var context = _fixture.CreateContext();
        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows, adminId);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.Success>(result);
        await using var readContext = _fixture.CreateContext();
        var city = await readContext.Cities.SingleAsync(c => c.Name == "Decatur");
        Assert.Equal(adminId.Value, city.CreatedByUserId);
    }

    [Fact]
    public async Task Unique_constraint_violation_at_persist_returns_ConcurrentDuplicate()
    {
        // Arrange
        var adminId = RegisteredUserId.NewId();
        var rows = new List<SaveCityRow> { new("Springfield", State.Illinois) };

        await using var context = _fixture.CreateContext();

        // Pre-stage a city entity that duplicates the batch row, but do NOT save it.
        // This simulates a concurrent insert: FindCrossBatchDuplicateAsync queries
        // the database (sees nothing — the staged entity is only in the local change
        // tracker), but SaveChangesAsync flushes both the staged entity and the
        // service's entity and hits the (Name, State) unique index.
        context.Cities.Add(
            new CityEntity
            {
                CityId = "CTY-RACE1",
                Name = "Springfield",
                State = "IL",
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedByUserId = adminId.Value,
            }
        );

        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows, adminId);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.ConcurrentDuplicate>(result);
    }
}
