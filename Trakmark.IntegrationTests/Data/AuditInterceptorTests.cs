using Microsoft.EntityFrameworkCore;
using Trakmark.Data;
using Trakmark.Data.Entities;
using Trakmark.Domain.Ids;
using Trakmark.Services;

namespace Trakmark.IntegrationTests.Data;

/// <summary>
/// Integration tests for <see cref="AuditInterceptor"/> using a real SQL Server container.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class AuditInterceptorTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;

    /// <summary>Initializes a new instance of <see cref="AuditInterceptorTests"/>.</summary>
    public AuditInterceptorTests(DatabaseFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync() => await _fixture.ResetAsync();

    /// <inheritdoc/>
    public Task DisposeAsync() => Task.CompletedTask;

    private ApplicationDbContext CreateContextWithInterceptor(ICurrentUserContext userContext)
    {
        var interceptor = new AuditInterceptor(userContext);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .AddInterceptors(interceptor)
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task SavingChangesAsync_AddedAuditableEntity_StampsCreatedByUserIdAndCreatedAt()
    {
        // Arrange
        var userId = RegisteredUserId.NewId();
        var userContext = new CurrentUserContext { UserId = userId };
        var before = DateTimeOffset.UtcNow;

        await using var context = CreateContextWithInterceptor(userContext);

        context.Cities.Add(new CityEntity
        {
            CityId = "CTY-AUDIT0001",
            Name = "AuditCity",
            State = "IL",
        });

        // Act
        await context.SaveChangesAsync();

        // Assert
        await using var readContext = _fixture.CreateContext();
        var city = await readContext.Cities.SingleAsync(c => c.Name == "AuditCity");
        Assert.Equal(userId.Value, city.CreatedByUserId);
        Assert.True(city.CreatedAt >= before);
    }

    [Fact]
    public async Task SavingChangesAsync_UserIdNullWithAddedAuditableEntity_ThrowsInvalidOperationException()
    {
        // Arrange
        var userContext = new CurrentUserContext();
        await using var context = CreateContextWithInterceptor(userContext);

        context.Cities.Add(new CityEntity
        {
            CityId = "CTY-AUDIT0002",
            Name = "NullUserCity",
            State = "IL",
        });

        // Act / Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task SavingChangesAsync_AddedNonAuditableEntity_DoesNotThrow()
    {
        // Arrange
        var userContext = new CurrentUserContext();
        await using var context = CreateContextWithInterceptor(userContext);

        context.RegisteredUsers.Add(new RegisteredUserEntity
        {
            RegisteredUserId = RegisteredUserId.NewId().Value,
            AccountId = Guid.NewGuid().ToString(),
        });

        // Act
        await context.SaveChangesAsync();

        // Assert
        await using var readContext = _fixture.CreateContext();
        Assert.Equal(1, await readContext.RegisteredUsers.CountAsync());
    }

    [Fact]
    public async Task SavingChanges_AddedAuditableEntity_StampsAuditFields()
    {
        // Arrange
        var userId = RegisteredUserId.NewId();
        var userContext = new CurrentUserContext { UserId = userId };
        var before = DateTimeOffset.UtcNow;

        await using var context = CreateContextWithInterceptor(userContext);

        context.Cities.Add(new CityEntity
        {
            CityId = "CTY-AUDIT0003",
            Name = "SyncAuditCity",
            State = "IL",
        });

        // Act
        context.SaveChanges();

        // Assert
        await using var readContext = _fixture.CreateContext();
        var city = await readContext.Cities.SingleAsync(c => c.Name == "SyncAuditCity");
        Assert.Equal(userId.Value, city.CreatedByUserId);
        Assert.True(city.CreatedAt >= before);
    }
}
