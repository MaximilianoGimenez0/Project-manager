using Application.Dtos.Responses;
using MediatR;

namespace Application.Features.Auth.Queries.GetCurrentUser
{
    public record GetCurrentUserQuery(int UserId) : IRequest<UserResponse>;
}
