using Microsoft.EntityFrameworkCore;
using Trakmark.Data.Entities;
using Trakmark.Domain.Ids;
using Trakmark.Domain.ValueObjects;
using Trakmark.Services;

namespace Trakmark.IntegrationTests.Services;

/// <summary>
/// Integration tests for <see cref="SaveCitiesBatchService"/> covering all batch-save
/// outcome scenarios: success, validation error, in-batch duplicate, cross-batch
/// duplicate, and CreatedByUserId propagation via <see cref="AuditInterceptor"/>.
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
    public async Task SaveAsync_BatchOutsideValidRange_ThrowsArgumentOutOfRangeException(int count)
    {
        // Arrange
        var rows = Enumerable
            .Range(0, count)
            .Select(i => new SaveCityRow($"City{i}", State.Illinois))
            .ToList();

        var userContext = new CurrentUserContext { UserId = RegisteredUserId.NewId() };
        await using var context = _fixture.CreateContext(userContext);
        var service = new SaveCitiesBatchService(context);

        // Act / Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.SaveAsync(rows));
    }

    [Fact]
    public async Task SaveAsync_AllValidRows_PersistsAllRows()
    {
        // Arrange
        var rows = new List<SaveCityRow>
        {
            new("Springfield", State.Illinois),
            new("Chicago", State.Illinois),
        };

        var userContext = new CurrentUserContext { UserId = RegisteredUserId.NewId() };
        await using var context = _fixture.CreateContext(userContext);
        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.Success>(result);
        await using var readContext = _fixture.CreateContext();
        var count = await readContext.Cities.CountAsync(c => c.State == "IL");
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task SaveAsync_OneInvalidRow_RejectsWholeBatch()
    {
        // Arrange
        var rows = new List<SaveCityRow>
        {
            new("Springfield", State.Illinois),
            new("   ", State.Illinois),
        };

        var userContext = new CurrentUserContext { UserId = RegisteredUserId.NewId() };
        await using var context = _fixture.CreateContext(userContext);
        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.ValidationError>(result);
        await using var readContext = _fixture.CreateContext();
        Assert.Equal(0, await readContext.Cities.CountAsync());
    }

    [Fact]
    public async Task SaveAsync_InBatchDuplicate_RejectsWholeBatch()
    {
        // Arrange
        var rows = new List<SaveCityRow>
        {
            new("Springfield", State.Illinois),
            new("springfield", State.Illinois),
        };

        var userContext = new CurrentUserContext { UserId = RegisteredUserId.NewId() };
        await using var context = _fixture.CreateContext(userContext);
        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.InBatchDuplicate>(result);
        await using var readContext = _fixture.CreateContext();
        Assert.Equal(0, await readContext.Cities.CountAsync());
    }

    [Fact]
    public async Task SaveAsync_CrossBatchDuplicate_RejectsWholeBatch()
    {
        // Arrange — pre-seed an existing city directly (bypasses interceptor, sets fields manually)
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

        var rows = new List<SaveCityRow> { new("Springfield", State.Illinois) };

        var userContext = new CurrentUserContext { UserId = RegisteredUserId.NewId() };
        await using var context = _fixture.CreateContext(userContext);
        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.CrossBatchDuplicate>(result);
        await using var readContext = _fixture.CreateContext();
        Assert.Equal(1, await readContext.Cities.CountAsync());
    }

    [Fact]
    public async Task SaveAsync_ValidBatch_AuditInterceptorStampsCreatedByUserId()
    {
        // Arrange
        var adminId = RegisteredUserId.NewId();
        var rows = new List<SaveCityRow> { new("Decatur", State.Illinois) };

        var userContext = new CurrentUserContext { UserId = adminId };
        await using var context = _fixture.CreateContext(userContext);
        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.Success>(result);
        await using var readContext = _fixture.CreateContext();
        var city = await readContext.Cities.SingleAsync(c => c.Name == "Decatur");
        Assert.Equal(adminId.Value, city.CreatedByUserId);
    }

    [Fact]
    public async Task SaveAsync_UniqueConstraintViolationAtPersist_ReturnsConcurrentDuplicate()
    {
        // Arrange
        var rows = new List<SaveCityRow> { new("Springfield", State.Illinois) };

        var userContext = new CurrentUserContext { UserId = RegisteredUserId.NewId() };
        await using var context = _fixture.CreateContext(userContext);

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
            }
        );

        var service = new SaveCitiesBatchService(context);

        // Act
        var result = await service.SaveAsync(rows);

        // Assert
        Assert.IsType<SaveCitiesBatchResult.ConcurrentDuplicate>(result);
    }
}
