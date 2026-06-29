namespace Trakmark.Services;

/// <summary>
/// Creates the <c>RegisteredUser</c> mapping that links an ASP.NET Core Identity
/// user ID to a domain <c>RegisteredUserId</c> on first OAuth registration.
/// </summary>
public interface IRegisteredUserMappingService
{
    /// <summary>
    /// Creates and persists a <c>RegisteredUser</c> aggregate for the given
    /// Identity user ID.
    /// </summary>
    /// <param name="identityUserId">
    /// The ASP.NET Core Identity user ID (GUID string) to map.
    /// </param>
    Task CreateAsync(string identityUserId);
}
