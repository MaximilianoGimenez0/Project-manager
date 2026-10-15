using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Application.Common.Interfaces;
using Application.Dtos.Responses;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Features.ProjectProposals.Commands.CreateProjectProposal
{
    public class CreateProjectProposalCommandHandler : IRequestHandler<CreateProjectProposalCommand, ProjectResponse>
    {
        private readonly IProjectProposalRepository _projectProposalRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IProjectTypeRepository _projectTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IApprovalStepEngine _approvalStepEngine;
        private readonly IMapper _mapper;

        public CreateProjectProposalCommandHandler(
            IProjectProposalRepository projectProposalRepository,
            IAreaRepository areaRepository,
            IProjectTypeRepository projectTypeRepository,
            IUserRepository userRepository,
            IApprovalStepEngine approvalStepEngine,
            IMapper mapper)
        {
            _projectProposalRepository = projectProposalRepository;
            _areaRepository = areaRepository;
            _projectTypeRepository = projectTypeRepository;
            _userRepository = userRepository;
            _approvalStepEngine = approvalStepEngine;
            _mapper = mapper;
        }

        public async Task<ProjectResponse> Handle(CreateProjectProposalCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (string.IsNullOrWhiteSpace(request.Title) ||
                string.IsNullOrWhiteSpace(request.Description) ||
                request.Amount == null ||
                request.Duration == null ||
                request.Area == null ||
                request.Type == null ||
                request.User == null)
            {
                throw new BadRequestException("Todos los campos obligatorios deben completarse.");
            }

            // Validar existencia de clave foránea con Área
            var area = await _areaRepository.GetById(request.Area.Value);
            if (area == null)
            {
                throw new BadRequestException("El área ingresada no existe.");
            }

            // Validar existencia de clave foránea con Tipo de Proyecto
            var type = await _projectTypeRepository.GetById(request.Type.Value);
            if (type == null)
            {
                throw new BadRequestException("El tipo de proyecto ingresado no existe.");
            }

            // Validar existencia de clave foránea con Usuario
            var user = await _userRepository.GetById(request.User.Value);
            if (user == null)
            {
                throw new BadRequestException("El usuario creador ingresado no existe.");
            }

            // Comprobar unicidad de título
            var allProjects = await _projectProposalRepository.GetAll();
            if (allProjects.Any(p => string.Equals(p.Title, request.Title, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BadRequestException("Ya existe un proyecto con el mismo título.");
            }

            var proposalId = Guid.NewGuid();
            var proposal = new ProjectProposal
            {
                Id = proposalId,
                Title = request.Title,
                Description = request.Description,
                EstimatedAmount = request.Amount.Value,
                EstimatedDuration = request.Duration.Value,
                CreateAt = DateTime.UtcNow,
                Status = (int)ApprovalStatusEnum.Pending,
                Area = request.Area.Value,
                Type = request.Type.Value,
                CreateBy = request.User.Value
            };

            // Calcular pasos según las reglas de negocio
            var steps = await _approvalStepEngine.CalculateStepsAsync(proposal);
            if (steps == null || steps.Count == 0)
            {
                throw new BadRequestException("No se encontraron reglas de aprobación aplicables para los parámetros del proyecto.");
            }

            proposal.Steps = steps;

            foreach (var step in steps)
            {
                step.StepProjectProposal = proposal;
                step.ProjectProposalId = proposalId;
            }

            await _projectProposalRepository.Add(proposal);

            var created = await _projectProposalRepository.GetById(proposalId);
            if (created == null)
            {
                throw new BadRequestException("Error al recuperar la propuesta creada.");
            }

            return _mapper.Map<ProjectResponse>(created);
        }
    }
}
