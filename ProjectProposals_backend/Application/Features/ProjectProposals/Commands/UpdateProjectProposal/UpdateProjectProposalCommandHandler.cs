using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Application.Dtos.Responses;
using Application.Exceptions;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Features.ProjectProposals.Commands.UpdateProjectProposal
{
    public class UpdateProjectProposalCommandHandler : IRequestHandler<UpdateProjectProposalCommand, ProjectResponse>
    {
        private readonly IProjectProposalRepository _projectProposalRepository;
        private readonly IMapper _mapper;

        public UpdateProjectProposalCommandHandler(IProjectProposalRepository projectProposalRepository, IMapper mapper)
        {
            _projectProposalRepository = projectProposalRepository;
            _mapper = mapper;
        }

        public async Task<ProjectResponse> Handle(UpdateProjectProposalCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                throw new BadRequestException("El ID del proyecto no puede estar vacío.");
            }

            var project = await _projectProposalRepository.GetById(command.Id);
            if (project == null)
            {
                throw new NotFoundException("El proyecto no se encuentra.");
            }

            var request = command.Request;

            // Comprobar unicidad de título si se modifica
            if (!string.IsNullOrWhiteSpace(request.Title) && !string.Equals(project.Title, request.Title, StringComparison.OrdinalIgnoreCase))
            {
                var allProjects = await _projectProposalRepository.GetAll();
                if (allProjects.Any(p => p.Id != command.Id && string.Equals(p.Title, request.Title, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new BadRequestException("Ya existe un proyecto con el mismo título.");
                }
            }

            // Solo se permite modificación si el proyecto está en estado Observado
            if (project.Status != (int)ApprovalStatusEnum.Observed)
            {
                throw new BadRequestException("El proyecto se encuentra en un estado que no permite modificaciones.");
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                project.Title = request.Title;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                project.Description = request.Description;
            }

            if (request.Duration.HasValue)
            {
                project.EstimatedDuration = request.Duration.Value;
            }

            await _projectProposalRepository.Update(project);

            var updated = await _projectProposalRepository.GetById(project.Id);
            return _mapper.Map<ProjectResponse>(updated ?? project);
        }
    }
}
