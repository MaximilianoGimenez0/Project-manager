using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ApprovalRule
    {
        public long Id { get; set; }

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public int StepOrder { get; set; }
        public int ApproverRoleId { get; set; }


        public int? Area { get; set; }
        public int? Type { get; set; }
        public Area? ApprovalRuleArea { get; set; }
        public ProjectType? ApprovalRuleType { get; set; }
        public ApproverRole? ApprovalRuleApproverRole { get; set; }

        public ApprovalRule(decimal minAmount, decimal maxAmount, int stepOrder, int approverRoleId) 
        {
            MinAmount = minAmount;
            MaxAmount = maxAmount; 
            StepOrder = stepOrder;
            ApproverRoleId = approverRoleId;
        }
    }
}
