using Trakmark.Domain.Ids;

namespace Trakmark.Services;

/// <summary>
/// Resolves the domain <see cref="RegisteredUserId"/> for a given
/// ASP.NET Core Identity user ID.
/// </summary>
public interface IRegisteredUserLookupService
{
    /// <summary>
    /// Returns the <see cref="RegisteredUserId"/> associated with the given
    /// Identity user ID.
    /// </summary>
    /// <param name="identityUserId">
    /// The ASP.NET Core Identity user ID (GUID string) to look up.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no <c>RegisteredUser</c> row exists for <paramref name="identityUserId"/>.
    /// </exception>
    Task<RegisteredUserId> GetByAccountIdAsync(string identityUserId);
}
