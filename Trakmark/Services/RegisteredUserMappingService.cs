using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trakmark.Data;
using Trakmark.Data.Entities;
using Trakmark.Domain.Aggregates;
using Trakmark.Domain.Ids;

namespace Trakmark.Services;

/// <summary>
/// Persists a <c>RegisteredUser</c> aggregate to the <c>RegisteredUsers</c> table
/// on first Google OAuth registration, bridging the ASP.NET Core Identity user ID
/// to the domain <see cref="Domain.Ids.RegisteredUserId"/>.
/// </summary>
public sealed class RegisteredUserMappingService : IRegisteredUserMappingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RegisteredUserMappingService> _logger;

    /// <summary>Initializes a new instance of <see cref="RegisteredUserMappingService"/>.</summary>
    public RegisteredUserMappingService(ApplicationDbContext context, ILogger<RegisteredUserMappingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task CreateAsync(string identityUserId)
    {
        ArgumentNullException.ThrowIfNull(identityUserId);

        var registeredUser = RegisteredUser.Create(new UserAccountId(identityUserId));

        _context.RegisteredUsers.Add(new RegisteredUserEntity
        {
            RegisteredUserId = registeredUser.Id.Value,
            AccountId = registeredUser.AccountId.Value,
            CreatedByUserId = WellKnownUsers.SystemUserId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            _logger.LogDuplicateMappingIgnored(identityUserId);
        }
    }
}
