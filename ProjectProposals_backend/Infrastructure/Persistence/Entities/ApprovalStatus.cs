using System.Collections.Generic;

namespace Infrastructure.Persistence.Entities
{
    public class ApprovalStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public IList<ProjectProposal> ProjectProposals { get; set; } = new List<ProjectProposal>();
        public IList<ProjectApprovalStep> ProjectApprovalSteps { get; set; } = new List<ProjectApprovalStep>();
    }
}
