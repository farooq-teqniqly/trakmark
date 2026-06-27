using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Trakmark.Extensions;

namespace Trakmark.Tests.Extensions;

/// <summary>Tests for <see cref="ServiceCollectionExtensions"/>.</summary>
public sealed class ServiceCollectionExtensionsTests
{
    private static IConfiguration BuildAuthConfig(string? clientId, string? clientSecret)
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

    private static IConfiguration BuildDbConfig(string? connectionString)
    {
        var values = new Dictionary<string, string?>();

        if (connectionString is not null)
        {
            values["ConnectionStrings:DefaultConnection"] = connectionString;
        }

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    private static IWebHostEnvironment BuildEnv(string environmentName = "Production")
    {
        var env = Substitute.For<IWebHostEnvironment>();
        env.EnvironmentName.Returns(environmentName);
        return env;
    }

    [Theory]
    [InlineData(null, "secret")]
    [InlineData("", "secret")]
    [InlineData("   ", "secret")]
    [InlineData("id", null)]
    [InlineData("id", "")]
    [InlineData("id", "   ")]
    [InlineData(null, null)]
    public void AddAppAuthentication_GoogleCredentialsMissing_ThrowsInvalidOperationException(
        string? clientId,
        string? clientSecret
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildAuthConfig(clientId, clientSecret);

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() => services.AddAppAuthentication(config));
    }

    [Fact]
    public void AddAppAuthentication_GoogleCredentialsPresent_Succeeds()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildAuthConfig("test-client-id", "test-client-secret");

        // Act / Assert
        Assert.NotNull(services.AddAppAuthentication(config));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddAppDatabase_ConnectionStringMissing_ThrowsInvalidOperationException(string? connectionString)
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildDbConfig(connectionString);
        var env = BuildEnv();

        // Act / Assert
        Assert.Throws<InvalidOperationException>(() => services.AddAppDatabase(config, env));
    }

    [Fact]
    public void AddAppDatabase_ConnectionStringPresent_Succeeds()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildDbConfig("Server=localhost;Database=Trakmark;Trusted_Connection=True;");
        var env = BuildEnv();

        // Act / Assert
        Assert.NotNull(services.AddAppDatabase(config, env));
    }
}
