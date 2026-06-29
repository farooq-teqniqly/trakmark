using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Trakmark.Services;

namespace Trakmark.IntegrationTests.Services;

/// <summary>
/// Integration tests for <see cref="IRegisteredUserLookupService"/> covering
/// the registered-user-identity-mapping spec scenarios.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class RegisteredUserLookupServiceTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;

    /// <summary>Initializes a new instance of <see cref="RegisteredUserLookupServiceTests"/>.</summary>
    public RegisteredUserLookupServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync() => await _fixture.ResetAsync();

    /// <inheritdoc/>
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetByAccountIdAsync_KnownIdentityUserId_ReturnsRegisteredUserId()
    {
        // Arrange
        var identityUserId = Guid.NewGuid().ToString();
        await using var seedContext = _fixture.CreateContext();
        var mappingService = new RegisteredUserMappingService(
            seedContext, NullLogger<RegisteredUserMappingService>.Instance);
        await mappingService.CreateAsync(identityUserId);

        var seeded = await seedContext.RegisteredUsers
            .SingleAsync(r => r.AccountId == identityUserId);
        var expectedRegisteredUserId = seeded.RegisteredUserId;

        await using var lookupContext = _fixture.CreateContext();
        var lookupService = new RegisteredUserLookupService(
            lookupContext, NullLogger<RegisteredUserLookupService>.Instance);

        // Act
        var registeredUserId = await lookupService.GetByAccountIdAsync(identityUserId);

        // Assert
        Assert.Equal(expectedRegisteredUserId, registeredUserId.Value);
    }

    [Fact]
    public async Task GetByAccountIdAsync_UnknownIdentityUserId_ThrowsInvalidOperationException()
    {
        // Arrange
        var unknownIdentityUserId = Guid.NewGuid().ToString();
        await using var context = _fixture.CreateContext();
        var lookupService = new RegisteredUserLookupService(
            context, NullLogger<RegisteredUserLookupService>.Instance);

        // Act / Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => lookupService.GetByAccountIdAsync(unknownIdentityUserId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetByAccountIdAsync_NullOrEmptyIdentityUserId_ThrowsArgumentException(
        string? identityUserId)
    {
        // Arrange
        await using var context = _fixture.CreateContext();
        var sut = new RegisteredUserLookupService(
            context, NullLogger<RegisteredUserLookupService>.Instance);

        // Act
        var ex = await Record.ExceptionAsync(() => sut.GetByAccountIdAsync(identityUserId!));

        // Assert
        Assert.IsType<ArgumentException>(ex, exactMatch: false);
    }
}
