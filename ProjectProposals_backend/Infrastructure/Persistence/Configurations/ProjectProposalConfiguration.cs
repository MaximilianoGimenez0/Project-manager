using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProjectProposalConfiguration : IEntityTypeConfiguration<ProjectProposal>
    {
        public void Configure(EntityTypeBuilder<ProjectProposal> builder)
        {
            builder.ToTable("ProjectProposal");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.Description)
                .IsRequired();

            builder.Property(p => p.EstimatedAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.EstimatedDuration)
                .IsRequired();

            builder.Property(p => p.CreateAt)
                .IsRequired();

            builder.HasOne(p => p.ProjectProposalArea)
                .WithMany(c => c.ProjectProposals)
                .HasForeignKey(p => p.Area)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ProjectProposalType)
                .WithMany(c => c.ProjectProposals)
                .HasForeignKey(p => p.Type)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ProjectProposalApprovalStatus)
                .WithMany(c => c.ProjectProposals)
                .HasForeignKey(p => p.Status)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ProjectProposalUser)
                .WithMany(c => c.ProjectProposals)
                .HasForeignKey(p => p.CreateBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}