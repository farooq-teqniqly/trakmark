using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trakmark.Data.Entities;

namespace Trakmark.Data.Configurations;

/// <summary>
/// EF Core entity type configuration for <see cref="RegisteredUserEntity"/>.
/// </summary>
public sealed class RegisteredUserConfiguration : IEntityTypeConfiguration<RegisteredUserEntity>
{
    /// <summary>The maximum length of the <see cref="RegisteredUserEntity.RegisteredUserId"/> column (format <c>USR-XXXXXX</c>).</summary>
    private const int RegisteredUserIdMaxLength = 10;

    /// <summary>
    /// The maximum length of the <see cref="RegisteredUserEntity.AccountId"/> column,
    /// set to 450 for ASP.NET Identity GUID compatibility.
    /// </summary>
    private const int AccountIdMaxLength = 450;

    /// <summary>The maximum length of the <see cref="IAuditableEntity.CreatedByUserId"/> column.</summary>
    private const int CreatedByUserIdMaxLength = 20;

    /// <summary>
    /// Configures the <c>RegisteredUsers</c> table: a DB-generated <c>int IDENTITY</c> clustered
    /// primary key, with <see cref="RegisteredUserEntity.RegisteredUserId"/> mapped as a unique
    /// non-clustered alternate key column, and a unique index on <see cref="RegisteredUserEntity.AccountId"/>.
    /// </summary>
    /// <param name="builder">The entity type builder supplied by EF Core.</param>
    public void Configure(EntityTypeBuilder<RegisteredUserEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("RegisteredUsers");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder
            .Property(r => r.RegisteredUserId)
            .HasMaxLength(RegisteredUserIdMaxLength)
            .ValueGeneratedNever();

        builder.HasAlternateKey(r => r.RegisteredUserId);

        builder.Property(r => r.AccountId).HasMaxLength(AccountIdMaxLength);

        builder.HasIndex(r => r.AccountId).IsUnique();

        builder.Property(r => r.CreatedByUserId).HasMaxLength(CreatedByUserIdMaxLength);
        builder.Property(r => r.CreatedAt).IsRequired();
    }
}
