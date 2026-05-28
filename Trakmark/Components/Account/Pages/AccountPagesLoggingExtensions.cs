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

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User with ID '{UserId}' logged in with 2fa."
    )]
    internal static partial void LogUserLoggedInWith2fa(this ILogger logger, string userId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "User with ID '{UserId}' account locked out."
    )]
    internal static partial void LogUserLockedOut2fa(this ILogger logger, string userId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Invalid authenticator code entered for user with ID '{UserId}'."
    )]
    internal static partial void LogInvalidAuthenticatorCode(this ILogger logger, string userId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User with ID '{UserId}' logged in with a recovery code."
    )]
    internal static partial void LogUserLoggedInWithRecoveryCode(
        this ILogger logger,
        string userId
    );

    [LoggerMessage(Level = LogLevel.Warning, Message = "User account locked out.")]
    internal static partial void LogUserLockedOutRecovery(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Invalid recovery code entered for user with ID '{UserId}'."
    )]
    internal static partial void LogInvalidRecoveryCode(this ILogger logger, string userId);
}
