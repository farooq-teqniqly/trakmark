namespace Trakmark.Data;

/// <summary>
/// Sentinel user ID values used when a row is written by the system rather than a logged-in user.
/// </summary>
public static class WellKnownUsers
{
    /// <summary>Sentinel value stamped on rows inserted by the system with no logged-in user.</summary>
    public const string SystemUserId = "SYSTEM";
}
