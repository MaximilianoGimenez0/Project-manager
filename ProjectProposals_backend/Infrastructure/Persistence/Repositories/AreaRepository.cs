using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public AreaRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task Add(Domain.Entities.Area area)
        {
            var efArea = new Infrastructure.Persistence.Entities.Area
            {
                Name = area.Name
            };

            await _context.Areas.AddAsync(efArea);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var efArea = await _context.Areas.FirstOrDefaultAsync(a => a.Id == id);
            if (efArea == null) return false;

            _context.Areas.Remove(efArea);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Domain.Entities.Area>> GetAll()
        {
            var efAreas = await _context.Areas
                .AsNoTracking()
                .ToListAsync();

            var domainAreas = new List<Domain.Entities.Area>();

            foreach (var efArea in efAreas)
            {
                var area = new Domain.Entities.Area
                {
                    Id = efArea.Id,
                    Name = efArea.Name
                };
                domainAreas.Add(area);
            }

            return domainAreas;
        }

        public async Task<Domain.Entities.Area?> GetById(int id)
        {
            var efArea = await _context.Areas
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            return efArea == null ? null : new Domain.Entities.Area { Id = efArea.Id, Name = efArea.Name };
        }

        public async Task<bool> Update(Domain.Entities.Area area)
        {
            var efArea = await _context.Areas.FirstOrDefaultAsync(a => a.Id == area.Id);
            if (efArea == null) return false;

            efArea.Name = area.Name;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
