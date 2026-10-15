using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Application.Dtos.Responses;
using Application.Exceptions;
using Domain.Interfaces;

namespace Application.Features.ProjectProposals.Queries.GetProjectProposalById
{
    public class GetProjectProposalByIdQueryHandler : IRequestHandler<GetProjectProposalByIdQuery, ProjectResponse>
    {
        private readonly IProjectProposalRepository _projectProposalRepository;
        private readonly IMapper _mapper;

        public GetProjectProposalByIdQueryHandler(IProjectProposalRepository projectProposalRepository, IMapper mapper)
        {
            _projectProposalRepository = projectProposalRepository;
            _mapper = mapper;
        }

        public async Task<ProjectResponse> Handle(GetProjectProposalByIdQuery query, CancellationToken cancellationToken)
        {
            if (query.Id == Guid.Empty)
            {
                throw new BadRequestException("El ID del proyecto no puede estar vacío.");
            }

            var project = await _projectProposalRepository.GetById(query.Id);
            if (project == null)
            {
                throw new NotFoundException("El proyecto no se encuentra registrado.");
            }

            return _mapper.Map<ProjectResponse>(project);
        }
    }
}
