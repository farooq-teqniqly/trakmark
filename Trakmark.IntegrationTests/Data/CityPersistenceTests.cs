using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Trakmark.Data;
using Trakmark.Data.Entities;

namespace Trakmark.IntegrationTests.Data;

/// <summary>
/// Integration tests verifying that a city round-trips through the real database,
/// including persistence-only audit metadata (<c>CreatedAt</c>, <c>CreatedByUserId</c>).
/// </summary>
public sealed class CityPersistenceTests : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

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

        await using (var writeContext = CreateContext())
        {
            writeContext.Cities.Add(entity);
            await writeContext.SaveChangesAsync();
        }

        // Act
        await using var readContext = CreateContext();
        var loaded = await readContext.Cities.SingleAsync(c => c.CityId == "CTY-ABCDEF");

        // Assert
        Assert.Equal("Springfield", loaded.Name);
        Assert.Equal("IL", loaded.State);
        Assert.Equal(createdAt, loaded.CreatedAt);
        Assert.Equal("USR-ABCDEF", loaded.CreatedByUserId);
    }

    private ApplicationDbContext CreateContext()
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "Trakmark",
        };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionStringBuilder.ConnectionString)
            .Options;

        return new ApplicationDbContext(options);
    }
}
