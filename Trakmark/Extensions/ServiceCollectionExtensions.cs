using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Trakmark.Data;

namespace Trakmark.Extensions;

/// <summary>Extension methods for <see cref="IServiceCollection"/> that register application-level services.</summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers authentication with Identity cookies and optionally Google OAuth when credentials are configured.</summary>
        public IServiceCollection AddAppAuthentication(IConfiguration config)
        {
            var auth = services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });

            auth.AddIdentityCookies();
            services.AddAuthorization();

            // Set credentials via user secrets (dev) or environment variables (prod):
            //   dotnet user-secrets set "Authentication:Google:ClientId" "<id>"
            //   dotnet user-secrets set "Authentication:Google:ClientSecret" "<secret>"
            var clientId =
                config["Authentication:Google:ClientId"]
                ?? throw new InvalidOperationException(
                    "Google OAuth ClientId is not configured. Set 'Authentication:Google:ClientId' in user secrets or environment variables."
                );

            var clientSecret =
                config["Authentication:Google:ClientSecret"]
                ?? throw new InvalidOperationException(
                    "Google OAuth ClientSecret is not configured. Set 'Authentication:Google:ClientSecret' in user secrets or environment variables."
                );

            auth.AddGoogle(options =>
            {
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
            });

            return services;
        }

        /// <summary>Registers the EF Core <see cref="ApplicationDbContext"/> backed by SQL Server, with detailed errors enabled in development.</summary>
        public IServiceCollection AddAppDatabase(IConfiguration config, IWebHostEnvironment env)
        {
            var connectionString =
                config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found."
                );

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);

                if (env.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging().EnableDetailedErrors();
                }
            });

            if (env.IsDevelopment())
            {
                services.AddDatabaseDeveloperPageExceptionFilter();
            }

            return services;
        }

        /// <summary>Registers ASP.NET Core Identity with roles, EF Core stores, and sign-in manager.</summary>
        public IServiceCollection AddAppIdentity()
        {
            services
                .AddIdentityCore<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
