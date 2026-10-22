using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Application.Dtos.Responses;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Features.ProjectProposals.Commands.DecideProjectStep
{
    public class DecideProjectStepCommandHandler : IRequestHandler<DecideProjectStepCommand, ProjectResponse>
    {
        private readonly IProjectProposalRepository _projectProposalRepository;
        private readonly IProjectApprovalStepRepository _stepRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public DecideProjectStepCommandHandler(
            IProjectProposalRepository projectProposalRepository,
            IProjectApprovalStepRepository stepRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _projectProposalRepository = projectProposalRepository;
            _stepRepository = stepRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ProjectResponse> Handle(DecideProjectStepCommand command, CancellationToken cancellationToken)
        {
            if (command.ProjectId == Guid.Empty)
            {
                throw new BadRequestException("El ID del proyecto no puede estar vacío.");
            }

            var project = await _projectProposalRepository.GetById(command.ProjectId);
            if (project == null)
            {
                throw new BadRequestException("El proyecto ingresado no existe.");
            }

            if (project.Status == (int)ApprovalStatusEnum.Approved || project.Status == (int)ApprovalStatusEnum.Rejected)
            {
                throw new ConflictException("El proyecto ya no se encuentra en un estado que permite modificaciones.");
            }

            var request = command.Request;
            var user = await _userRepository.GetById(request.User);
            if (user == null)
            {
                throw new BadRequestException("El usuario ingresado no existe.");
            }

            if (project.CreateBy != 0 && project.CreateBy == user.Id)
            {
                throw new ConflictException("El usuario creador de la propuesta no puede actuar como aprobador de la misma.");
            }

            var nextStep = project.Steps?
                .Where(s => s.Status == (int)ApprovalStatusEnum.Pending || s.Status == (int)ApprovalStatusEnum.Observed)
                .OrderBy(s => s.StepOrder)
                .FirstOrDefault();

            if (nextStep == null)
            {
                throw new BadRequestException("El proyecto no tiene un paso pendiente de aprobación.");
            }

            if (nextStep.Id != request.Id)
            {
                throw new BadRequestException("El id del paso no corresponde con el próximo paso pendiente de aprobación.");
            }

            if (nextStep.ApproverRoleId != user.Role)
            {
                throw new BadRequestException("El rol del usuario no corresponde, no puede aprobar este paso.");
            }

            nextStep.Status = request.Status;
            nextStep.DecisionDate = DateTime.UtcNow;
            nextStep.ApproverUserId = request.User;
            nextStep.Observations = request.Observation;

            var updatedStep = await _stepRepository.Update(nextStep);
            if (!updatedStep)
            {
                throw new BadRequestException("Datos de decisión inválidos.");
            }

            var steps = project.Steps ?? new List<ProjectApprovalStep>();
            if (steps.Any(s => s.Status == (int)ApprovalStatusEnum.Rejected))
            {
                project.Status = (int)ApprovalStatusEnum.Rejected;
            }
            else if (steps.Count > 0 && steps.All(s => s.Status == (int)ApprovalStatusEnum.Approved))
            {
                project.Status = (int)ApprovalStatusEnum.Approved;
            }
            else if (steps.Any(s => s.Status == (int)ApprovalStatusEnum.Observed))
            {
                project.Status = (int)ApprovalStatusEnum.Observed;
            }
            else
            {
                project.Status = (int)ApprovalStatusEnum.Pending;
            }

            await _projectProposalRepository.Update(project);

            var result = await _projectProposalRepository.GetById(command.ProjectId);
            return _mapper.Map<ProjectResponse>(result ?? project);
        }
    }
}
