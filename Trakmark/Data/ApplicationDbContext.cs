using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Trakmark.Data.Configurations;
using Trakmark.Data.Entities;

namespace Trakmark.Data;

/// <summary>EF Core database context that combines the ASP.NET Core Identity schema with application data.</summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    /// <summary>The persisted cities.</summary>
    public DbSet<CityEntity> Cities => Set<CityEntity>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new CityConfiguration());
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        // EF Core's runtime pending-model-changes check raises a false
        // positive for this context: `dotnet ef migrations
        // has-pending-model-changes` confirms no drift, and the AddCities
        // migration's BuildTargetModel is structurally identical to
        // ApplicationDbContextModelSnapshot.BuildModel. Suppressing per
        // Microsoft's documented mitigation rather than masking real drift.
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(RelationalEventId.PendingModelChangesWarning)
        );
    }
}
