using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dtos
{
    public class ProjectProposalViewDto
    {
        public Guid? Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public decimal? EstimatedAmount { get; set; }

        public int? EstimatedDuration { get; set; }

        public DateTime? CreateAt { get; set; }

        //fk
        public string? Area { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? CreateBy { get; set; }

    }
}
