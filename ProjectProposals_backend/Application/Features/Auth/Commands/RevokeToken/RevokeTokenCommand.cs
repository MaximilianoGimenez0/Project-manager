using Application.Dtos.Requests;
using MediatR;

namespace Application.Features.Auth.Commands.RevokeToken
{
    public record RevokeTokenCommand(RevokeTokenRequest Request, string? IpAddress = null) : IRequest<bool>;
}
