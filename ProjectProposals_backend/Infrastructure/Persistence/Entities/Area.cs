using System.Collections.Generic;

namespace Infrastructure.Persistence.Entities
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public IList<ApprovalRule> ApprovalRules { get; set; } = new List<ApprovalRule>();
        public IList<ProjectProposal> ProjectProposals { get; set; } = new List<ProjectProposal>();
    }
}
