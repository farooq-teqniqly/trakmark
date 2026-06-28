using Microsoft.Extensions.Logging;

namespace Trakmark.Services;

/// <summary>Source-generated log extensions for <see cref="RegisteredUserMappingService"/>.</summary>
internal static partial class RegisteredUserMappingServiceLoggingExtensions
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "RegisteredUser mapping for Identity user {IdentityUserId} already exists; skipping.")]
    internal static partial void LogDuplicateMappingIgnored(
        this ILogger logger,
        string identityUserId);
}
