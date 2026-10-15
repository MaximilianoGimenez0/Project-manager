using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task Add(User user);
        Task<List<User>> GetAll();
        Task<User?> GetById(int id);
        Task<User?> GetByEmail(string email);
        Task<bool> Update(User user);
        Task<bool> Delete(int id);

    }
}
