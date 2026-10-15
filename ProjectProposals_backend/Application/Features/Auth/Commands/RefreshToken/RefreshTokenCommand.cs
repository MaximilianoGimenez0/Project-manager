using Application.Dtos.Requests;
using Application.Dtos.Responses;
using MediatR;

namespace Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequest Request, string? IpAddress = null) : IRequest<AuthResponse>;
}
