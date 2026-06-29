using Microsoft.Extensions.Logging;

namespace Trakmark.Services;

/// <summary>Source-generated log extensions for <see cref="RegisteredUserLookupService"/>.</summary>
internal static partial class RegisteredUserLookupServiceLog
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "No RegisteredUser mapping found for Identity user ID '{IdentityUserId}'."
    )]
    public static partial void LogMappingNotFound(
        this ILogger<RegisteredUserLookupService> logger,
        string identityUserId
    );
}
