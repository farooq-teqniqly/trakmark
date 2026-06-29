using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Trakmark.Services;

namespace Trakmark.IntegrationTests.Services;

/// <summary>
/// Integration tests for <see cref="RegisteredUserMappingService"/> covering
/// the registered-user-identity-mapping spec scenarios.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class RegisteredUserMappingServiceTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;

    /// <summary>Initializes a new instance of <see cref="RegisteredUserMappingServiceTests"/>.</summary>
    public RegisteredUserMappingServiceTests(DatabaseFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync() => await _fixture.ResetAsync();

    /// <inheritdoc/>
    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateAsync_NullOrEmptyIdentityUserId_ThrowsArgumentException(string? identityUserId)
    {
        // Arrange
        await using var context = _fixture.CreateContext();
        var service = new RegisteredUserMappingService(context, NullLogger<RegisteredUserMappingService>.Instance);

        // Act / Assert
        await Assert.ThrowsAnyAsync<ArgumentException>(() => service.CreateAsync(identityUserId!));
    }

    [Fact]
    public async Task CreateAsync_ValidIdentityUserId_PersistsRegisteredUserRow()
    {
        // Arrange
        var identityUserId = Guid.NewGuid().ToString();
        await using var context = _fixture.CreateContext();
        var service = new RegisteredUserMappingService(context, NullLogger<RegisteredUserMappingService>.Instance);

        // Act
        await service.CreateAsync(identityUserId);

        // Assert
        await using var readContext = _fixture.CreateContext();
        var entity = await readContext.RegisteredUsers
            .SingleAsync(r => r.AccountId == identityUserId);
        Assert.False(string.IsNullOrEmpty(entity.RegisteredUserId));
    }

    [Fact]
    public async Task CreateAsync_ValidIdentityUserId_StampsSystemSentinelAuditValues()
    {
        // Arrange
        var identityUserId = Guid.NewGuid().ToString();
        var before = DateTimeOffset.UtcNow;
        await using var context = _fixture.CreateContext();
        var service = new RegisteredUserMappingService(context, NullLogger<RegisteredUserMappingService>.Instance);

        // Act
        await service.CreateAsync(identityUserId);

        // Assert
        await using var readContext = _fixture.CreateContext();
        var entity = await readContext.RegisteredUsers.SingleAsync(r => r.AccountId == identityUserId);
        Assert.Equal("SYSTEM", entity.CreatedByUserId);
        Assert.True(entity.CreatedAt >= before);
    }

    [Fact]
    public async Task CreateAsync_SameIdentityUserIdCalledTwice_IsIdempotentAndPersistsSingleRow()
    {
        // Arrange
        var identityUserId = Guid.NewGuid().ToString();
        await using var context1 = _fixture.CreateContext();
        var service1 = new RegisteredUserMappingService(context1, NullLogger<RegisteredUserMappingService>.Instance);
        await service1.CreateAsync(identityUserId);

        // Act
        await using var context2 = _fixture.CreateContext();
        var service2 = new RegisteredUserMappingService(context2, NullLogger<RegisteredUserMappingService>.Instance);
        var exception = await Record.ExceptionAsync(() => service2.CreateAsync(identityUserId));

        // Assert
        Assert.Null(exception);
        await using var readContext = _fixture.CreateContext();
        Assert.Equal(1, await readContext.RegisteredUsers.CountAsync(r => r.AccountId == identityUserId));
    }
}
