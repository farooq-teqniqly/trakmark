using Microsoft.EntityFrameworkCore;
using Trakmark.Data;
using Trakmark.Domain.Ids;
using Trakmark.Services;

namespace Trakmark.Tests.Data;

/// <summary>Unit tests for <see cref="AuditInterceptor"/>.</summary>
public sealed class AuditInterceptorTests
{
#pragma warning disable S1144 // Id is required by EF Core as the convention-based primary key
    private sealed class AuditableTestEntity : IAuditableEntity
    {
        public int Id { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
    }

    private sealed class NonAuditableTestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
#pragma warning restore S1144

    private sealed class TestAuditContext : DbContext
    {
        public TestAuditContext(DbContextOptions<TestAuditContext> options) : base(options) { }
        public DbSet<AuditableTestEntity> Auditable => Set<AuditableTestEntity>();
        public DbSet<NonAuditableTestEntity> NonAuditable => Set<NonAuditableTestEntity>();
    }

    private static TestAuditContext CreateContext(AuditInterceptor interceptor)
    {
        var options = new DbContextOptionsBuilder<TestAuditContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;
        return new TestAuditContext(options);
    }

    [Fact]
    public async Task SavingChangesAsync_AddedAuditableEntity_StampsCreatedByUserIdAndCreatedAt()
    {
        // Arrange
        var userId = RegisteredUserId.NewId();
        var userContext = new CurrentUserContext { UserId = userId };
        var interceptor = new AuditInterceptor(userContext);
        var before = DateTimeOffset.UtcNow;

        await using var context = CreateContext(interceptor);
        context.Auditable.Add(new AuditableTestEntity());

        // Act
        await context.SaveChangesAsync();

        // Assert
        var entity = await context.Auditable.SingleAsync();
        Assert.Equal(userId.Value, entity.CreatedByUserId);
        Assert.True(entity.CreatedAt >= before);
    }

    [Fact]
    public async Task SavingChangesAsync_UserIdNullWithAddedAuditableEntity_ThrowsInvalidOperationException()
    {
        // Arrange
        var userContext = new CurrentUserContext();
        var interceptor = new AuditInterceptor(userContext);

        await using var context = CreateContext(interceptor);
        context.Auditable.Add(new AuditableTestEntity());

        // Act / Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task SavingChangesAsync_AddedNonAuditableEntity_DoesNotThrow()
    {
        // Arrange
        var userContext = new CurrentUserContext();
        var interceptor = new AuditInterceptor(userContext);

        await using var context = CreateContext(interceptor);
        context.NonAuditable.Add(new NonAuditableTestEntity { Name = "test" });

        // Act
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, await context.NonAuditable.CountAsync());
    }

    [Fact]
    public void SavingChanges_AddedAuditableEntity_StampsCreatedByUserIdAndCreatedAt()
    {
        // Arrange
        var userId = RegisteredUserId.NewId();
        var userContext = new CurrentUserContext { UserId = userId };
        var interceptor = new AuditInterceptor(userContext);
        var before = DateTimeOffset.UtcNow;

        using var context = CreateContext(interceptor);
        context.Auditable.Add(new AuditableTestEntity());

        // Act
#pragma warning disable S6966
        context.SaveChanges();
#pragma warning restore S6966

        // Assert
        var entity = context.Auditable.Single();
        Assert.Equal(userId.Value, entity.CreatedByUserId);
        Assert.True(entity.CreatedAt >= before);
    }

    [Fact]
    public void SavingChanges_UserIdNullWithAddedAuditableEntity_ThrowsInvalidOperationException()
    {
        // Arrange
        var userContext = new CurrentUserContext();
        var interceptor = new AuditInterceptor(userContext);

        using var context = CreateContext(interceptor);
        context.Auditable.Add(new AuditableTestEntity());

        // Act / Assert
#pragma warning disable S6966
        Assert.Throws<InvalidOperationException>(() => context.SaveChanges());
#pragma warning restore S6966
    }
}
