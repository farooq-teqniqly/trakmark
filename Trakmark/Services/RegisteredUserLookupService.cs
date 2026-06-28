using Microsoft.EntityFrameworkCore;
using Trakmark.Data;
using Trakmark.Domain.Ids;

namespace Trakmark.Services;

/// <summary>
/// Queries the <c>RegisteredUsers</c> table to resolve the domain
/// <see cref="RegisteredUserId"/> for a given ASP.NET Core Identity user ID.
/// </summary>
public sealed class RegisteredUserLookupService : IRegisteredUserLookupService
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of <see cref="RegisteredUserLookupService"/>.</summary>
    public RegisteredUserLookupService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<RegisteredUserId> GetByAccountIdAsync(string identityUserId)
    {
        ArgumentNullException.ThrowIfNull(identityUserId);

        var entity = await _context.RegisteredUsers
            .SingleOrDefaultAsync(r => r.AccountId == identityUserId);

        if (entity is null)
        {
            throw new InvalidOperationException(
                $"No RegisteredUser found for Identity user ID '{identityUserId}'.");
        }

        return RegisteredUserId.Parse(entity.RegisteredUserId);
    }
}
