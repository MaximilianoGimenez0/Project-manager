using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IApprovalRuleRepository
    {
        Task Add(ApprovalRule approvalRule);
        Task<List<ApprovalRule>> GetAll();
        Task<ApprovalRule?> GetById(long id);
        Task<bool> Update(ApprovalRule approvalRule);
        Task<bool> Delete(long id);
    }
}
