using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAreaRepository
    {
        Task Add(Area area);
        Task<List<Area>> GetAll();
        Task<Area?> GetById(int id);
        Task<bool> Update(Area area);
        Task<bool> Delete(int id);
    }
}
