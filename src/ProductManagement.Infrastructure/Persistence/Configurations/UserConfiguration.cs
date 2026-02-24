using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.ValueObjects;

namespace ProductManagement.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // Value Object: Email
        builder.OwnsOne(u => u.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);

            emailBuilder.HasIndex(e => e.Value)
                .IsUnique();
        });

        // Value Object: PasswordHash
        builder.OwnsOne(u => u.PasswordHash, passwordBuilder =>
        {
            passwordBuilder.Property(p => p.Value)
                .HasColumnName("PasswordHash")
                .IsRequired()
                .HasMaxLength(500);
        });

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.LastLoginAt);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        // Collection: RefreshTokens (Owned Entity)
        builder.OwnsMany(u => u.RefreshTokens, rtBuilder =>
        {
            rtBuilder.ToTable("RefreshTokens");

            rtBuilder.WithOwner().HasForeignKey("UserId");

            rtBuilder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            rtBuilder.HasIndex(rt => rt.Token);

            rtBuilder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            rtBuilder.Property(rt => rt.CreatedAt)
                .IsRequired();

            rtBuilder.Property(rt => rt.IsRevoked)
                .IsRequired()
                .HasDefaultValue(false);

            rtBuilder.Property(rt => rt.RevokedAt);
        });

        builder.Navigation(u => u.Email).IsRequired();
        builder.Navigation(u => u.PasswordHash).IsRequired();
    }
}
