using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dtos
{
    public class ProjectProposalDto
    {
        public Guid? Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public decimal? EstimatedAmount { get; set; }

        public int? EstimatedDuration { get; set; }

        public DateTime? CreateAt { get; set; }

        //fk
        public int? Area { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
        public int? CreateBy { get; set; }

        public string? Message { get; set; }
        public bool Result { get; set; }

    }
}
