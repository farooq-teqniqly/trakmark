using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

    private readonly ILogger<RegisteredUserLookupService> _logger;

    /// <summary>Initializes a new instance of <see cref="RegisteredUserLookupService"/>.</summary>
    public RegisteredUserLookupService(
        ApplicationDbContext context,
        ILogger<RegisteredUserLookupService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<RegisteredUserId> GetByAccountIdAsync(string identityUserId)
    {
        ArgumentException.ThrowIfNullOrEmpty(identityUserId);

        var entity = await _context.RegisteredUsers
            .SingleOrDefaultAsync(r => r.AccountId == identityUserId);

        if (entity is null)
        {
            _logger.LogMappingNotFound(identityUserId);
            throw new InvalidOperationException(
                $"No RegisteredUser found for Identity user ID '{identityUserId}'.");
        }

        return RegisteredUserId.Parse(entity.RegisteredUserId);
    }
}
