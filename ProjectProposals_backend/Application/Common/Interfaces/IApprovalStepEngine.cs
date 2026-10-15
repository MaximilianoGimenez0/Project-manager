using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IApprovalStepEngine
    {
        Task<List<ProjectApprovalStep>> CalculateStepsAsync(ProjectProposal project);
    }
}
