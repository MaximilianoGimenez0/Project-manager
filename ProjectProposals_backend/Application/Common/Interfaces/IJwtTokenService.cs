using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(string? ipAddress = null);
    }
}
