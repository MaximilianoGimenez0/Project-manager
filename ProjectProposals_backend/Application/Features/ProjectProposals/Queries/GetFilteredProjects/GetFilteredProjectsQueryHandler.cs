using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Dtos.Responses;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Features.ProjectProposals.Queries.GetFilteredProjects
{
    public class GetFilteredProjectsQueryHandler : IRequestHandler<GetFilteredProjectsQuery, List<ProjectShortResponse>>
    {
        private readonly IProjectProposalRepository _projectProposalRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectApprovalStepRepository _stepRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IApprovalStatusRepository _statusRepository;
        private readonly IProjectTypeRepository _typeRepository;

        public GetFilteredProjectsQueryHandler(
            IProjectProposalRepository projectProposalRepository,
            IUserRepository userRepository,
            IProjectApprovalStepRepository stepRepository,
            IAreaRepository areaRepository,
            IApprovalStatusRepository statusRepository,
            IProjectTypeRepository typeRepository)
        {
            _projectProposalRepository = projectProposalRepository;
            _userRepository = userRepository;
            _stepRepository = stepRepository;
            _areaRepository = areaRepository;
            _statusRepository = statusRepository;
            _typeRepository = typeRepository;
        }

        public async Task<List<ProjectShortResponse>> Handle(GetFilteredProjectsQuery query, CancellationToken cancellationToken)
        {
            var projects = await _projectProposalRepository.GetAll();

            if (query.ApprovalUser != null)
            {
                var user = await _userRepository.GetById(query.ApprovalUser.Value);
                if (user == null)
                {
                    throw new BadRequestException("El usuario no existe.");
                }

                var allSteps = await _stepRepository.GetAll();
                var pendingSteps = allSteps
                    .Where(s => s.Status == (int)ApprovalStatusEnum.Pending || s.Status == (int)ApprovalStatusEnum.Observed)
                    .GroupBy(s => s.ProjectProposalId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderBy(s => s.StepOrder).FirstOrDefault()
                    );

                projects = projects.Where(p =>
                    pendingSteps.TryGetValue(p.Id, out var nextStep) &&
                    nextStep != null &&
                    nextStep.ApproverRoleId == user.Role
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                projects = projects
                    .Where(p => p.Title.Contains(query.Title, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (query.Status.HasValue)
            {
                projects = projects
                    .Where(p => p.Status == query.Status.Value)
                    .ToList();
            }

            if (query.Applicant.HasValue)
            {
                projects = projects
                    .Where(p => p.CreateBy == query.Applicant.Value)
                    .ToList();
            }

            var areaDict = (await _areaRepository.GetAll()).ToDictionary(a => a.Id, a => a.Name);
            var statusDict = (await _statusRepository.GetAll()).ToDictionary(s => s.Id, s => s.Name);
            var typeDict = (await _typeRepository.GetAll()).ToDictionary(t => t.Id, t => t.Name);

            var response = projects.Select(p => new ProjectShortResponse
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Amount = p.EstimatedAmount,
                Duration = p.EstimatedDuration,
                Area = areaDict.TryGetValue(p.Area, out var areaName) ? areaName : string.Empty,
                Status = statusDict.TryGetValue(p.Status, out var statusName) ? statusName : string.Empty,
                Type = typeDict.TryGetValue(p.Type, out var typeName) ? typeName : string.Empty
            }).ToList();

            return response;
        }
    }
}
