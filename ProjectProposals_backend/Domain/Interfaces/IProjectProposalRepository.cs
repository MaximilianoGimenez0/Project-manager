using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProjectProposalRepository
    {
        Task<bool> Add(ProjectProposal project);
        Task<List<ProjectProposal>> GetAll();
        Task<ProjectProposal?> GetById(Guid id);
        Task<bool> Update(ProjectProposal project);
        Task<bool> Delete(Guid id);
    }
}
