using Trakmark.Domain.Ids;

namespace Trakmark.Services;

/// <summary>
/// Scoped implementation of <see cref="ICurrentUserContext"/>.
/// Each Blazor Server circuit gets its own instance, so setting
/// <see cref="UserId"/> in one component is visible to the
/// <see cref="Data.AuditInterceptor"/> within the same scope.
/// </summary>
public sealed class CurrentUserContext : ICurrentUserContext
{
    /// <inheritdoc/>
    public RegisteredUserId? UserId { get; set; }
}
