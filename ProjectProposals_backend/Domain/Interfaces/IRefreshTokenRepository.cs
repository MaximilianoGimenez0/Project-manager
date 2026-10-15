using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task Add(RefreshToken refreshToken);
        Task<RefreshToken?> GetByToken(string token);
        Task<bool> Update(RefreshToken refreshToken);
        Task RevokeUserTokens(int userId, string reason, string? ipAddress = null);
    }
}
