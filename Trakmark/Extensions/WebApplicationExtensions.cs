using Microsoft.AspNetCore.Identity;

namespace Trakmark.Extensions;

/// <summary>Extension methods for <see cref="WebApplication"/> used during application startup.</summary>
public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        /// <summary>Seeds the <c>Admin</c> and <c>User</c> Identity roles if they do not already exist.</summary>
        public async Task SeedRolesAsync()
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in new[] { "Admin", "User" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        /// <summary>Registers standard middleware: migrations endpoint or exception handler, HSTS, status-code pages, HTTPS redirect, and antiforgery.</summary>
        public WebApplication UseAppMiddleware()
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            return app;
        }
    }
}
