using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProjectApprovalStepRepository
    {
        Task<long> Add(ProjectApprovalStep step);
        Task<List<ProjectApprovalStep>> GetAll();
        Task<ProjectApprovalStep?> GetById(long id);
        Task<bool> Update(ProjectApprovalStep step);
        Task<bool> Delete(long id);
    }
}
