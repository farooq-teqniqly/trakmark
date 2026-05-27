namespace Trakmark.Components.Account;

internal static partial class IdentityComponentsEndpointRouteBuilderExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "User with ID '{UserId}' asked for their personal data.")]
    private static partial void LogPersonalDataDownloaded(ILogger logger, string userId);
}
