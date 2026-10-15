using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Entities
{
    public class ProjectProposal
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedAmount { get; set; }
        public int EstimatedDuration { get; set; }
        public DateTime CreateAt { get; set; }

        public int Area { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public int CreateBy { get; set; }

        public Area ProjectProposalArea { get; set; } = null!;
        public ProjectType ProjectProposalType { get; set; } = null!;
        public ApprovalStatus ProjectProposalApprovalStatus { get; set; } = null!;
        public User ProjectProposalUser { get; set; } = null!;

        public IList<ProjectApprovalStep> ProjectApprovalSteps { get; set; } = new List<ProjectApprovalStep>();
    }
}
