using Trakmark.Domain.Ids;

namespace Trakmark.Services;

/// <summary>
/// Holds the resolved <see cref="RegisteredUserId"/> for the current DI scope.
/// Authorized Blazor components set <see cref="UserId"/> in
/// <c>OnInitializedAsync</c> after resolving the user via
/// <c>IRegisteredUserLookupService</c>. <see cref="Data.AuditInterceptor"/>
/// reads this value when stamping <see cref="Data.IAuditableEntity"/> entries.
/// </summary>
public interface ICurrentUserContext
{
    /// <summary>
    /// Gets or sets the <see cref="RegisteredUserId"/> for the current scope,
    /// or <see langword="null"/> before a component has set it.
    /// </summary>
    RegisteredUserId? UserId { get; set; }
}
