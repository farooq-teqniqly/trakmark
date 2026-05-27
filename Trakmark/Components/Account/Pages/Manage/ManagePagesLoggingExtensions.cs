namespace Trakmark.Components.Account.Pages.Manage;

/// <summary>Source-generated log messages for account management pages.</summary>
internal static partial class ManagePagesLoggingExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "User with ID '{UserId}' has disabled 2fa.")]
    internal static partial void LogDisabled2fa(this ILogger logger, string userId);

[LoggerMessage(Level = LogLevel.Information, Message = "User with ID '{UserId}' has enabled 2FA with an authenticator app.")]
    internal static partial void LogEnabled2fa(this ILogger logger, string userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "User with ID '{UserId}' deleted themselves.")]
    internal static partial void LogUserDeletedSelf(this ILogger logger, string userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "User with ID '{UserId}' has generated new 2FA recovery codes.")]
    internal static partial void LogGeneratedRecoveryCodes(this ILogger logger, string userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "User with ID '{UserId}' has reset their authentication app key.")]
    internal static partial void LogResetAuthenticatorKey(this ILogger logger, string userId);
}
