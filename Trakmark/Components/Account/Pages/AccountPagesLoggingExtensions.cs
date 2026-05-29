namespace Trakmark.Components.Account.Pages;

/// <summary>Source-generated log messages for account pages.</summary>
internal static partial class AccountPagesLoggingExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "{Name} logged in with {LoginProvider} provider."
    )]
    internal static partial void LogExternalLoginSuccess(
        this ILogger logger,
        string? name,
        string loginProvider
    );

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User created an account using {LoginProvider} provider."
    )]
    internal static partial void LogUserCreatedWithExternalProvider(
        this ILogger logger,
        string loginProvider
    );
}
