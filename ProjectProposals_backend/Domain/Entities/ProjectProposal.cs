using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProjectProposal
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal EstimatedAmount { get; set; }

        public int EstimatedDuration { get; set; }

        public DateTime CreateAt { get; set; }

        //fk
        public int Area { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public int CreateBy { get; set; }

        //objetos referenciados

        public Area? ProjectProposalArea { get; set; }
        public ProjectType? ProjectProposalType { get; set; }
        public ApprovalStatus? ProjectProposalApprovalStatus { get; set; }
        public User? ProjectProposalUser { get; set; }

        public List<ProjectApprovalStep> Steps { get; set; } = new();
        

    }
}
