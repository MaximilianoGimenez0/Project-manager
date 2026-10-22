using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, bool>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RevokeTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<bool> Handle(RevokeTokenCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new BadRequestException("El token de actualización es obligatorio.");
            }

            var storedToken = await _refreshTokenRepository.GetByToken(request.RefreshToken);
            if (storedToken == null)
            {
                throw new NotFoundException("El token de actualización no fue encontrado.");
            }

            if (!storedToken.IsActive)
            {
                return true;
            }

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = command.IpAddress;
            storedToken.ReasonRevoked = "Cierre de sesión / Revocación manual";

            return await _refreshTokenRepository.Update(storedToken);
        }
    }
}
