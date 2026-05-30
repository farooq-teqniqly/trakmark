namespace Trakmark.Components.Account.Pages.Manage;

/// <summary>Source-generated log messages for account management pages.</summary>
internal static partial class ManagePagesLoggingExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User with ID '{UserId}' deleted themselves."
    )]
    internal static partial void LogUserDeletedSelf(this ILogger logger, string userId);
}
