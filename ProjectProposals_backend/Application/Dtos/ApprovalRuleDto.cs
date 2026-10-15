using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dtos
{
    public class ApprovalRuleDto
    {
        public long? Id { get; set; }

        public decimal? MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

        public int? StepOrder { get; set; }
        public int? ApproverRoleId { get; set; }


        public int? Area { get; set; }
        public int? Type { get; set; }
                       
    }
}
