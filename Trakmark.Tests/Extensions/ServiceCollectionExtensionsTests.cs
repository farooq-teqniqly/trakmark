using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trakmark.Extensions;

namespace Trakmark.Tests.Extensions;

/// <summary>Tests for <see cref="ServiceCollectionExtensions"/>.</summary>
public sealed class ServiceCollectionExtensionsTests
{
    private static IConfiguration BuildConfig(string? clientId, string? clientSecret)
    {
        var values = new Dictionary<string, string?>();

        if (clientId is not null)
        {
            values["Authentication:Google:ClientId"] = clientId;
        }

        if (clientSecret is not null)
        {
            values["Authentication:Google:ClientSecret"] = clientSecret;
        }

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    [Theory]
    [InlineData(null, "secret")]
    [InlineData("id", null)]
    [InlineData(null, null)]
    public void AddAppAuthentication_throws_when_google_credentials_missing(
        string? clientId,
        string? clientSecret
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildConfig(clientId, clientSecret);

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() => services.AddAppAuthentication(config));
    }

    [Fact]
    public void AddAppAuthentication_succeeds_when_google_credentials_present()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildConfig("test-client-id", "test-client-secret");

        // Act / Assert
        var result = services.AddAppAuthentication(config);
        Assert.NotNull(result);
    }
}
