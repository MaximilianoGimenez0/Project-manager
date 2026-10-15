using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public RefreshTokenRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task Add(Domain.Entities.RefreshToken refreshToken)
        {
            var efToken = new Infrastructure.Persistence.Entities.RefreshToken
            {
                Id = refreshToken.Id,
                UserId = refreshToken.UserId,
                Token = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiresAt,
                CreatedAt = refreshToken.CreatedAt,
                CreatedByIp = refreshToken.CreatedByIp,
                RevokedAt = refreshToken.RevokedAt,
                RevokedByIp = refreshToken.RevokedByIp,
                ReplacedByToken = refreshToken.ReplacedByToken,
                ReasonRevoked = refreshToken.ReasonRevoked
            };

            await _context.RefreshTokens.AddAsync(efToken);
            await _context.SaveChangesAsync();
        }

        public async Task<Domain.Entities.RefreshToken?> GetByToken(string token)
        {
            var efToken = await _context.RefreshTokens
                .Include(r => r.User)
                    .ThenInclude(u => u.UserRole)
                .FirstOrDefaultAsync(r => r.Token == token);

            if (efToken == null) return null;

            return new Domain.Entities.RefreshToken
            {
                Id = efToken.Id,
                UserId = efToken.UserId,
                User = efToken.User != null ? new Domain.Entities.User
                {
                    Id = efToken.User.Id,
                    Name = efToken.User.Name,
                    Email = efToken.User.Email,
                    PasswordHash = efToken.User.PasswordHash,
                    Role = efToken.User.Role,
                    UserRole = efToken.User.UserRole != null ? new Domain.Entities.ApproverRole
                    {
                        Id = efToken.User.UserRole.Id,
                        Name = efToken.User.UserRole.Name
                    } : null
                } : null,
                Token = efToken.Token,
                ExpiresAt = efToken.ExpiresAt,
                CreatedAt = efToken.CreatedAt,
                CreatedByIp = efToken.CreatedByIp,
                RevokedAt = efToken.RevokedAt,
                RevokedByIp = efToken.RevokedByIp,
                ReplacedByToken = efToken.ReplacedByToken,
                ReasonRevoked = efToken.ReasonRevoked
            };
        }

        public async Task<bool> Update(Domain.Entities.RefreshToken refreshToken)
        {
            var tokenDb = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Id == refreshToken.Id);
            if (tokenDb == null) return false;

            tokenDb.RevokedAt = refreshToken.RevokedAt;
            tokenDb.RevokedByIp = refreshToken.RevokedByIp;
            tokenDb.ReplacedByToken = refreshToken.ReplacedByToken;
            tokenDb.ReasonRevoked = refreshToken.ReasonRevoked;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RevokeUserTokens(int userId, string reason, string? ipAddress = null)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            if (activeTokens.Count > 0)
            {
                foreach (var token in activeTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                    token.ReasonRevoked = reason;
                    token.RevokedByIp = ipAddress;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
