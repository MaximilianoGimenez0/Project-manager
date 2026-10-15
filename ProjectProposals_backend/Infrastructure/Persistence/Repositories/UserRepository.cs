using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public UserRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task Add(Domain.Entities.User user)
        {
            var efUser = new Infrastructure.Persistence.Entities.User
            {
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role
            };

            await _context.Users.AddAsync(efUser);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var efUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (efUser == null) return false;

            _context.Users.Remove(efUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Domain.Entities.User>> GetAll()
        {
            var efUsers = await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRole)
                .ToListAsync();

            var domainUsers = new List<Domain.Entities.User>();

            foreach (var efUser in efUsers)
            {
                var user = new Domain.Entities.User
                {
                    Id = efUser.Id,
                    Name = efUser.Name,
                    Email = efUser.Email,
                    PasswordHash = efUser.PasswordHash,
                    Role = efUser.Role,
                    UserRole = efUser.UserRole != null ? new Domain.Entities.ApproverRole
                    {
                        Id = efUser.UserRole.Id,
                        Name = efUser.UserRole.Name
                    } : null
                };

                domainUsers.Add(user);
            }

            return domainUsers;
        }

        public async Task<Domain.Entities.User?> GetById(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            return new Domain.Entities.User
            {
                Id = id,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role,
                UserRole = user.UserRole != null ? new Domain.Entities.ApproverRole
                {
                    Id = user.UserRole.Id,
                    Name = user.UserRole.Name
                } : null
            };
        }

        public async Task<Domain.Entities.User?> GetByEmail(string email)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());

            if (user == null) return null;

            return new Domain.Entities.User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role,
                UserRole = user.UserRole != null ? new Domain.Entities.ApproverRole
                {
                    Id = user.UserRole.Id,
                    Name = user.UserRole.Name
                } : null
            };
        }

        public async Task<bool> Update(Domain.Entities.User user)
        {
            var userDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (userDb == null) return false;

            userDb.Name = user.Name;
            userDb.Email = user.Email;
            userDb.PasswordHash = user.PasswordHash;
            userDb.Role = user.Role;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
