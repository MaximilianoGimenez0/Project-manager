using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProjectTypeRepository
    {
        Task Add(ProjectType type);
        Task<List<ProjectType>> GetAll();
        Task<ProjectType?> GetById(int id);
        Task<bool> Update(ProjectType type);
        Task<bool> Delete(int id);
    
    }
}
