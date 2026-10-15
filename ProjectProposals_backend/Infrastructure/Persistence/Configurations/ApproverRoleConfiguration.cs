using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence
{
    public class ApproverRoleConfiguration : IEntityTypeConfiguration<ApproverRole>
    {
        public void Configure(EntityTypeBuilder<ApproverRole> builder)
        {
            builder.ToTable("ApproverRole");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(p => p.HierarchyLevel)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.IsSpecializedRole)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}