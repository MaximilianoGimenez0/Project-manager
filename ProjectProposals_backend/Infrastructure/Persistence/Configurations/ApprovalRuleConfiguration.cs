using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ApprovalRuleConfiguration : IEntityTypeConfiguration<ApprovalRule>
    {
        public void Configure(EntityTypeBuilder<ApprovalRule> builder)
        {
            builder.ToTable("ApprovalRule");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.MinAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.MaxAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.StepOrder)
                .IsRequired();

            builder.HasOne(p => p.ApprovalRuleApproverRole)
                .WithMany(c => c.ApprovalRules)
                .HasForeignKey(p => p.ApproverRoleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.ApprovalRuleType)
                .WithMany(c => c.ApprovalRules)
                .HasForeignKey(p => p.Type)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.ApprovalRuleArea)
                .WithMany(c => c.ApprovalRules)
                .HasForeignKey(p => p.Area)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}