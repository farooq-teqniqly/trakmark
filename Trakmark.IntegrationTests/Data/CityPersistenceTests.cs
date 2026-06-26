using Microsoft.EntityFrameworkCore;
using Trakmark.Data.Entities;

namespace Trakmark.IntegrationTests.Data;

/// <summary>
/// Integration tests verifying that a city round-trips through the real database,
/// including persistence-only audit metadata (<c>CreatedAt</c>, <c>CreatedByUserId</c>).
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class CityPersistenceTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;

    /// <summary>Initializes a new instance of <see cref="CityPersistenceTests"/>.</summary>
    public CityPersistenceTests(DatabaseFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync() => await _fixture.ResetAsync();

    /// <inheritdoc/>
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task City_round_trips_with_created_at_and_created_by_user_id()
    {
        // Arrange
        var createdAt = new DateTimeOffset(2026, 6, 21, 12, 30, 0, TimeSpan.Zero);
        var entity = new CityEntity
        {
            CityId = "CTY-ABCDEF",
            Name = "Springfield",
            State = "IL",
            CreatedAt = createdAt,
            CreatedByUserId = "USR-ABCDEF",
        };

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.Cities.Add(entity);
            await writeContext.SaveChangesAsync();
        }

        // Act
        await using var readContext = _fixture.CreateContext();
        var loaded = await readContext.Cities.SingleAsync(c => c.CityId == "CTY-ABCDEF");

        // Assert
        Assert.Equal("Springfield", loaded.Name);
        Assert.Equal("IL", loaded.State);
        Assert.Equal(createdAt, loaded.CreatedAt);
        Assert.Equal("USR-ABCDEF", loaded.CreatedByUserId);
    }
}
