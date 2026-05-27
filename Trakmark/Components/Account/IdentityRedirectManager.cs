using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Trakmark.Data;

namespace Trakmark.Components.Account;

/// <summary>Performs server-side redirects and attaches one-time status messages via a short-lived cookie.</summary>
internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    /// <summary>Name of the cookie used to transport a status message across a redirect.</summary>
    public const string StatusCookieName = "Identity.StatusMessage";

    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5),
    };

    private string CurrentPath =>
        navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

    /// <summary>Redirects to <paramref name="uri"/>, converting absolute URIs to base-relative paths to prevent open redirects.</summary>
    public void RedirectTo(string? uri)
    {
        uri ??= "";

        // Prevent open redirects. Also reject scheme-relative URLs (e.g. "//evil.com") which
        // Uri.IsWellFormedUriString incorrectly accepts as relative.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative) || uri.StartsWith("//"))
        {
            uri = navigationManager.ToBaseRelativePath(uri);
        }

        navigationManager.NavigateTo(uri);
    }

    /// <summary>Redirects to <paramref name="uri"/> with the given query parameters appended.</summary>
    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);
    }

    /// <summary>Redirects to the current page path without query string.</summary>
    public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

    /// <summary>Redirects to the current page and sets a status message cookie.</summary>
    public void RedirectToCurrentPageWithStatus(string message, HttpContext context) =>
        RedirectToWithStatus(CurrentPath, message, context);

    /// <summary>Redirects to the invalid-user page with an error status containing the current user ID.</summary>
    public void RedirectToInvalidUser(
        UserManager<ApplicationUser> userManager,
        HttpContext context
    ) =>
        RedirectToWithStatus(
            "Account/InvalidUser",
            $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.",
            context
        );

    /// <summary>Appends a status message cookie and redirects to <paramref name="uri"/>.</summary>
    public void RedirectToWithStatus(string uri, string message, HttpContext context)
    {
        context.Response.Cookies.Append(
            StatusCookieName,
            message,
            StatusCookieBuilder.Build(context)
        );
        RedirectTo(uri);
    }
}
