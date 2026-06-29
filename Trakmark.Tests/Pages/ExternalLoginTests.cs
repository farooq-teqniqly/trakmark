using System.Security.Claims;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Trakmark.Components.Account.Pages;
using Trakmark.Data;
using Trakmark.Services;

namespace Trakmark.Tests.Pages;

/// <summary>
/// bUnit tests for <c>ExternalLogin</c> covering the registration-failure
/// redirect behavior from the registered-user-identity-mapping spec.
/// </summary>
public sealed class ExternalLoginTests : BunitContext
{
    [Fact]
    public async Task ExternalLoginCallback_MappingServiceThrows_RedirectsToLoginWithoutSignIn()
    {
        // Arrange
        var userStore = Substitute.For<
            IUserStore<ApplicationUser>,
            IUserEmailStore<ApplicationUser>
        >();
        var userManager = Substitute.For<UserManager<ApplicationUser>>(
            userStore,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );
        userManager.SupportsUserEmail.Returns(true);
        userManager.CreateAsync(Arg.Any<ApplicationUser>()).Returns(IdentityResult.Success);
        userManager
            .AddLoginAsync(Arg.Any<ApplicationUser>(), Arg.Any<UserLoginInfo>())
            .Returns(IdentityResult.Success);

        var signInManager = Substitute.For<SignInManager<ApplicationUser>>(
            userManager,
            Substitute.For<IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            Options.Create(new IdentityOptions()),
            Substitute.For<ILogger<SignInManager<ApplicationUser>>>(),
            Substitute.For<IAuthenticationSchemeProvider>(),
            Substitute.For<IUserConfirmation<ApplicationUser>>()
        );

        // ExternalLoginSignInAsync returns failed (new user, not yet registered locally)
        signInManager
            .ExternalLoginSignInAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<bool>(),
                Arg.Any<bool>()
            )
            .Returns(SignInResult.Failed);

        // GetExternalLoginInfoAsync returns valid info with email claim
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Email, "test@example.com"),
                    new Claim(ClaimTypes.NameIdentifier, "google-key"),
                ],
                "Google"
            )
        );
        var loginInfo = new ExternalLoginInfo(principal, "Google", "google-key", "Google");
        signInManager.GetExternalLoginInfoAsync().Returns(loginInfo);

        var mappingService = Substitute.For<IRegisteredUserMappingService>();
        mappingService
            .CreateAsync(Arg.Any<string>())
            .Returns<Task>(_ => throw new InvalidOperationException("database error"));

        // Register services
        Services.AddSingleton<IUserStore<ApplicationUser>>(userStore);
        Services.AddSingleton(userManager);
        Services.AddSingleton(signInManager);
        Services.AddSingleton(mappingService);
        Services.AddSingleton(Substitute.For<ILogger<ExternalLogin>>());

        // Register IdentityRedirectManager (internal type — access via reflection)
        var redirectManagerType =
            typeof(ExternalLogin).Assembly.GetType(
                "Trakmark.Components.Account.IdentityRedirectManager"
            )
            ?? throw new InvalidOperationException(
                "IdentityRedirectManager type not found — was it renamed or moved?"
            );
        Services.AddSingleton(redirectManagerType);

        // Navigate to the ExternalLogin URL so [SupplyParameterFromQuery] picks up action
        var navManager = Services.GetRequiredService<NavigationManager>() as BunitNavigationManager;
        navManager!.NavigateTo("http://localhost/Account/ExternalLogin?action=LoginCallback");

        // Provide HttpContext as the required cascading parameter
        HttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "GET";

        // Act
        Render<ExternalLogin>(p => p.AddCascadingValue(httpContext));

        // Assert
        Assert.Contains(
            navManager.History,
            h => h.Uri.Contains("Account/Login") && !h.Uri.Contains("ExternalLogin")
        );

        await signInManager
            .DidNotReceive()
            .SignInAsync(Arg.Any<ApplicationUser>(), Arg.Any<bool>(), Arg.Any<string?>());
    }
}
