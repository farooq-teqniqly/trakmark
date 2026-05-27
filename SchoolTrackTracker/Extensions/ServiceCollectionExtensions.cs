using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolTrackTracker.Components.Account;
using SchoolTrackTracker.Data;

namespace SchoolTrackTracker.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAppAuthentication(IConfiguration config)
        {
            var auth = services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });

            auth.AddIdentityCookies();

            // Set credentials via user secrets (dev) or environment variables (prod):
            //   dotnet user-secrets set "Authentication:Google:ClientId" "<id>"
            //   dotnet user-secrets set "Authentication:Google:ClientSecret" "<secret>"
            var clientId = config["Authentication:Google:ClientId"];
            var clientSecret = config["Authentication:Google:ClientSecret"];
            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
            {
                auth.AddGoogle(options =>
                {
                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                });
            }

            return services;
        }

        public IServiceCollection AddAppDatabase(IConfiguration config,
            IWebHostEnvironment env)
        {
            var connectionString = config.GetConnectionString("DefaultConnection")
                                   ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

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

            services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            return services;
        }
    }
}
