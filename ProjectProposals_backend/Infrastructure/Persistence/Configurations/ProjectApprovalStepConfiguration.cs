using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProjectApprovalStepConfiguration : IEntityTypeConfiguration<ProjectApprovalStep>
    {
        public void Configure(EntityTypeBuilder<ProjectApprovalStep> builder)
        {
            builder.ToTable("ProjectApprovalStep");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.StepOrder)
                .IsRequired();

            builder.Property(p => p.DecisionDate)
                .IsRequired(false);

            builder.Property(p => p.Observations)
                .IsRequired(false);

            builder.HasOne(p => p.StepProjectProposal)
                .WithMany(c => c.ProjectApprovalSteps)
                .HasForeignKey(p => p.ProjectProposalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.StepUser)
                .WithMany(c => c.ProjectApprovalSteps)
                .HasForeignKey(p => p.ApproverUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.StepApproverRole)
                .WithMany(c => c.ProjectApprovalSteps)
                .HasForeignKey(p => p.ApproverRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.StepApprovalStatus)
                .WithMany(c => c.ProjectApprovalSteps)
                .HasForeignKey(p => p.Status)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}