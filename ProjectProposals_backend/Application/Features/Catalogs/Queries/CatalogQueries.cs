using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Application.Dtos.Responses;
using Domain.Interfaces;

namespace Application.Features.Catalogs.Queries
{
    public record GetAreasQuery : IRequest<List<GenericResponse>>;
    public record GetProjectTypesQuery : IRequest<List<GenericResponse>>;
    public record GetApproverRolesQuery : IRequest<List<GenericResponse>>;
    public record GetApprovalStatusesQuery : IRequest<List<GenericResponse>>;
    public record GetUsersQuery : IRequest<List<UserResponse>>;

    public class CatalogQueryHandlers :
        IRequestHandler<GetAreasQuery, List<GenericResponse>>,
        IRequestHandler<GetProjectTypesQuery, List<GenericResponse>>,
        IRequestHandler<GetApproverRolesQuery, List<GenericResponse>>,
        IRequestHandler<GetApprovalStatusesQuery, List<GenericResponse>>,
        IRequestHandler<GetUsersQuery, List<UserResponse>>
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IProjectTypeRepository _projectTypeRepository;
        private readonly IApproverRoleRepository _approverRoleRepository;
        private readonly IApprovalStatusRepository _approvalStatusRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CatalogQueryHandlers(
            IAreaRepository areaRepository,
            IProjectTypeRepository projectTypeRepository,
            IApproverRoleRepository approverRoleRepository,
            IApprovalStatusRepository approvalStatusRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _areaRepository = areaRepository;
            _projectTypeRepository = projectTypeRepository;
            _approverRoleRepository = approverRoleRepository;
            _approvalStatusRepository = approvalStatusRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<GenericResponse>> Handle(GetAreasQuery request, CancellationToken cancellationToken)
        {
            var areas = await _areaRepository.GetAll();
            return _mapper.Map<List<GenericResponse>>(areas);
        }

        public async Task<List<GenericResponse>> Handle(GetProjectTypesQuery request, CancellationToken cancellationToken)
        {
            var types = await _projectTypeRepository.GetAll();
            return _mapper.Map<List<GenericResponse>>(types);
        }

        public async Task<List<GenericResponse>> Handle(GetApproverRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _approverRoleRepository.GetAll();
            return _mapper.Map<List<GenericResponse>>(roles);
        }

        public async Task<List<GenericResponse>> Handle(GetApprovalStatusesQuery request, CancellationToken cancellationToken)
        {
            var statuses = await _approvalStatusRepository.GetAll();
            return _mapper.Map<List<GenericResponse>>(statuses);
        }

        public async Task<List<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAll();
            return _mapper.Map<List<UserResponse>>(users);
        }
    }
}
