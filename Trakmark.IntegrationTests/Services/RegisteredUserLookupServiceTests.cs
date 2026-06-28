using Microsoft.Extensions.Logging.Abstractions;
using Trakmark.Domain.Ids;
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
        ArgumentNullException.ThrowIfNull(fixture);
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

        await using var lookupContext = _fixture.CreateContext();
        var lookupService = new RegisteredUserLookupService(lookupContext);

        // Act
        var registeredUserId = await lookupService.GetByAccountIdAsync(identityUserId);

        // Assert
        Assert.True(RegisteredUserId.TryParse(registeredUserId.Value, out _));
    }

    [Fact]
    public async Task GetByAccountIdAsync_UnknownIdentityUserId_ThrowsInvalidOperationException()
    {
        // Arrange
        var unknownIdentityUserId = Guid.NewGuid().ToString();
        await using var context = _fixture.CreateContext();
        var lookupService = new RegisteredUserLookupService(context);

        // Act / Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => lookupService.GetByAccountIdAsync(unknownIdentityUserId));
    }
}
