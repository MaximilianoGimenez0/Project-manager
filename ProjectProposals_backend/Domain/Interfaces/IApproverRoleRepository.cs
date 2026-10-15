using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IApproverRoleRepository
    {
        Task Add(ApproverRole role);
        Task<List<ApproverRole>> GetAll();
        Task<ApproverRole?> GetById(int id);
        Task<bool> Update(ApproverRole role);
        Task<bool> Delete(int id);
    }
}
