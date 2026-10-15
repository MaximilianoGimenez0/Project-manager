using Application.Dtos.Requests;
using Application.Dtos.Responses;
using MediatR;

namespace Application.Features.Auth.Commands.Login
{
    public record LoginCommand(LoginRequest Request, string? IpAddress = null) : IRequest<AuthResponse>;
}
