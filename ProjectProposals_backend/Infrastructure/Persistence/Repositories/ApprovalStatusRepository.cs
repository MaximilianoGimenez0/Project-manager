using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ApprovalStatusRepository : IApprovalStatusRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public ApprovalStatusRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task Add(ApprovalStatus status)
        {
            var efStatus = new Infrastructure.Persistence.Entities.ApprovalStatus
            {
                Name = status.Name
            };

            await _context.Status.AddAsync(efStatus);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var efStatus = await _context.Status.FirstOrDefaultAsync(s => s.Id == id);
            if (efStatus == null) return false;

            _context.Status.Remove(efStatus);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ApprovalStatus>> GetAll()
        {
            var efStatuses = await _context.Status
                .AsNoTracking()
                .ToListAsync();

            var domainStatuses = new List<ApprovalStatus>();

            foreach (var efStatus in efStatuses)
            {
                var status = new ApprovalStatus(efStatus.Name)
                {
                    Id = efStatus.Id
                };
                domainStatuses.Add(status);
            }

            return domainStatuses;
        }

        public async Task<ApprovalStatus?> GetById(int id)
        {
            var efStatus = await _context.Status
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            return efStatus == null ? null : new ApprovalStatus(efStatus.Name) { Id = efStatus.Id };
        }

        public async Task<bool> Update(ApprovalStatus status)
        {
            var efStatus = await _context.Status.FirstOrDefaultAsync(r => r.Id == status.Id);
            if (efStatus == null) return false;

            efStatus.Name = status.Name;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
