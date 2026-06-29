using Microsoft.Extensions.Logging;

namespace Trakmark.Components.Pages;

/// <summary>Source-generated log extensions for <see cref="AddCities"/>.</summary>
internal static partial class AddCitiesLog
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to look up registered user for account ID '{AccountId}'.")]
    public static partial void LogUserLookupFailed(
        this ILogger<AddCities> logger, string accountId, Exception exception);
}
