using Microsoft.EntityFrameworkCore;
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

    [Fact]
    public async Task CreateAsync_ValidIdentityUserId_PersistsRegisteredUserRow()
    {
        // Arrange
        var identityUserId = Guid.NewGuid().ToString();
        await using var context = _fixture.CreateContext();
        var service = new RegisteredUserMappingService(context);

        // Act
        await service.CreateAsync(identityUserId);

        // Assert
        await using var readContext = _fixture.CreateContext();
        var entity = await readContext.RegisteredUsers
            .SingleAsync(r => r.AccountId == identityUserId);
        Assert.Equal(identityUserId, entity.AccountId);
        Assert.False(string.IsNullOrEmpty(entity.RegisteredUserId));
    }
}
