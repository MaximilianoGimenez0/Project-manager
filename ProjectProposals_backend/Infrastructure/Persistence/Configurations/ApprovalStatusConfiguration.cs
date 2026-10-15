using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ApprovalStatusConfiguration : IEntityTypeConfiguration<Entities.ApprovalStatus>
    {
        public void Configure(EntityTypeBuilder<Entities.ApprovalStatus> builder)
        {
            builder.ToTable("ApprovalStatus");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(25);
        }
    }
}