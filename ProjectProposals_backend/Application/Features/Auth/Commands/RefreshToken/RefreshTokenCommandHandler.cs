using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Application.Common.Interfaces;
using Application.Dtos.Responses;
using Application.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService,
            IMapper mapper)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }

        public async Task<AuthResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new BadRequestException("El token de actualización es obligatorio.");
            }

            var storedToken = await _refreshTokenRepository.GetByToken(request.RefreshToken);
            if (storedToken == null)
            {
                throw new UnauthorizedException("Token de actualización inválido.");
            }

            // Detección de reúso de token comprometido
            if (storedToken.IsRevoked)
            {
                await _refreshTokenRepository.RevokeUserTokens(
                    storedToken.UserId,
                    "Posible compromiso de seguridad: Intento de reutilización de token revocado",
                    command.IpAddress);

                throw new UnauthorizedException("El token de actualización ha sido revocado. Todas las sesiones activas han sido cerradas por seguridad.");
            }

            if (storedToken.IsExpired)
            {
                throw new UnauthorizedException("El token de actualización ha expirado. Por favor, inicie sesión nuevamente.");
            }

            var user = storedToken.User ?? await _userRepository.GetById(storedToken.UserId);
            if (user == null)
            {
                throw new UnauthorizedException("El usuario asociado al token no existe.");
            }

            // Generar nuevo par de tokens (Rotación)
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken(command.IpAddress);
            newRefreshToken.UserId = user.Id;

            // Revocar el token anterior reemplazándolo por el nuevo
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = command.IpAddress;
            storedToken.ReplacedByToken = newRefreshToken.Token;
            storedToken.ReasonRevoked = "Rotación de token";

            await _refreshTokenRepository.Update(storedToken);
            await _refreshTokenRepository.Add(newRefreshToken);

            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);

            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                TokenType = "Bearer",
                ExpiresIn = 900,
                User = _mapper.Map<UserResponse>(user)
            };
        }
    }
}
