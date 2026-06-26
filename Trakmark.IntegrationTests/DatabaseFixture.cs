using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.MsSql;
using Trakmark.Data;

namespace Trakmark.IntegrationTests;

/// <summary>
/// xUnit collection fixture that starts a single SQL Server Testcontainers
/// container for the entire test run and exposes helpers to create a
/// <see cref="ApplicationDbContext"/> and to reset the database between tests.
/// </summary>
public sealed class DatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder().Build();
    private Respawner _respawner = null!;

    /// <summary>Gets the connection string for the shared <c>Trakmark</c> database.</summary>
    public string ConnectionString { get; private set; } = null!;

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var builder = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "Trakmark",
        };

        ConnectionString = builder.ConnectionString;

        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        await using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
        });
    }

    /// <inheritdoc/>
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    /// <summary>Resets all data in the shared database, leaving the schema intact.</summary>
    public async Task ResetAsync()
    {
        await using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
    }

    /// <summary>Creates a new <see cref="ApplicationDbContext"/> backed by the shared container.</summary>
    public ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new ApplicationDbContext(options);
    }
}
