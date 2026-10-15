using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Application.Dtos.Responses;
using Application.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResponse> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetById(query.UserId);
            if (user == null)
            {
                throw new NotFoundException("Usuario no encontrado.");
            }

            return _mapper.Map<UserResponse>(user);
        }
    }
}
