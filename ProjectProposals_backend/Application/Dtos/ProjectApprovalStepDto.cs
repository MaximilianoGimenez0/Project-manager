using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dtos
{
    public class ProjectApprovalStepDto
    {
        public long? Id { get; set; }
        public int? StepOrder { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string? Observations { get; set; }

        public Guid? ProjectProposalId { get; set; }
        public int? ApproverUserId { get; set; }
        public int? ApproverRoleId { get; set; }
        public int? Status { get; set; }

    }
}
