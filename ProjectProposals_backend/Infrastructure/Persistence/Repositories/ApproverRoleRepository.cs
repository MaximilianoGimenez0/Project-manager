using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ApproverRoleRepository : IApproverRoleRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public ApproverRoleRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task Add(ApproverRole role)
        {
            var efRole = new Infrastructure.Persistence.Entities.ApproverRole
            {
                Name = role.Name,
                HierarchyLevel = role.HierarchyLevel,
                IsSpecializedRole = role.IsSpecializedRole
            };

            await _context.Roles.AddAsync(efRole);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var efRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (efRole == null) return false;

            _context.Roles.Remove(efRole);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ApproverRole>> GetAll()
        {
            var efRoles = await _context.Roles
                .AsNoTracking()
                .ToListAsync();

            var domainRoles = new List<ApproverRole>();

            foreach (var efRole in efRoles)
            {
                var role = new ApproverRole
                {
                    Id = efRole.Id,
                    Name = efRole.Name,
                    HierarchyLevel = efRole.HierarchyLevel,
                    IsSpecializedRole = efRole.IsSpecializedRole
                };
                domainRoles.Add(role);
            }

            return domainRoles;
        }

        public async Task<ApproverRole?> GetById(int id)
        {
            var efRole = await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            return efRole == null ? null : new ApproverRole
            {
                Id = efRole.Id,
                Name = efRole.Name,
                HierarchyLevel = efRole.HierarchyLevel,
                IsSpecializedRole = efRole.IsSpecializedRole
            };
        }

        public async Task<bool> Update(ApproverRole role)
        {
            var efRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == role.Id);
            if (efRole == null) return false;

            efRole.Name = role.Name;
            efRole.HierarchyLevel = role.HierarchyLevel;
            efRole.IsSpecializedRole = role.IsSpecializedRole;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
