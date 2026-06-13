using Microsoft.AspNetCore.Identity;

namespace Trakmark.Data;

/// <summary>Application-specific Identity user. Add profile properties here to extend the default Identity schema.</summary>
// S2094: intentionally empty — standard ASP.NET Core Identity extension point for future profile properties.
#pragma warning disable S2094
public class ApplicationUser : IdentityUser { }
#pragma warning restore S2094
