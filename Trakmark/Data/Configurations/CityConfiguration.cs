using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trakmark.Data.Entities;

namespace Trakmark.Data.Configurations;

/// <summary>
/// EF Core entity type configuration for <see cref="CityEntity"/>.
/// </summary>
public sealed class CityConfiguration : IEntityTypeConfiguration<CityEntity>
{
    /// <summary>The maximum length of the <see cref="CityEntity.Name"/> column.</summary>
    private const int NameMaxLength = 100;

    /// <summary>
    /// Configures the <c>Cities</c> table: a DB-generated <c>int IDENTITY</c> clustered
    /// primary key, with <see cref="CityEntity.CityId"/> mapped as a unique non-clustered
    /// alternate key column.
    /// </summary>
    /// <param name="builder">The entity type builder supplied by EF Core.</param>
    public void Configure(EntityTypeBuilder<CityEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Cities");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.CityId).IsRequired().HasMaxLength(20);
        builder.HasAlternateKey(c => c.CityId);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(NameMaxLength);

        builder.Property(c => c.State).IsRequired().HasMaxLength(2);

        builder.Property(c => c.CreatedAt).IsRequired();

        builder.Property(c => c.CreatedByUserId).IsRequired().HasMaxLength(20);

        builder.HasIndex(c => new { c.Name, c.State }).IsUnique();
    }
}
