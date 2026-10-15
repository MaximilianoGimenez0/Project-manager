using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(r => r.Token)
                .IsUnique();

            builder.Property(r => r.ExpiresAt)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.CreatedByIp)
                .HasMaxLength(100);

            builder.Property(r => r.RevokedByIp)
                .HasMaxLength(100);

            builder.Property(r => r.ReplacedByToken)
                .HasMaxLength(255);

            builder.Property(r => r.ReasonRevoked)
                .HasMaxLength(255);

            builder.HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
