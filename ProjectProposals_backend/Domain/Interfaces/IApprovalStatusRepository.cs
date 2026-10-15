using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IApprovalStatusRepository
    {
        Task Add(ApprovalStatus status);
        Task<List<ApprovalStatus>> GetAll();
        Task<ApprovalStatus?> GetById(int id);
        Task<bool> Update(ApprovalStatus status);
        Task<bool> Delete(int id);
    }
}
