using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Application.Dtos.Responses;
using Application.Exceptions;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Features.ProjectProposals.Queries.GetRoleApprovals
{
    public class GetRoleApprovalsQueryHandler : IRequestHandler<GetRoleApprovalsQuery, List<ProjectResponse>>
    {
        private readonly IProjectProposalRepository _projectProposalRepository;
        private readonly IProjectApprovalStepRepository _stepRepository;
        private readonly IMapper _mapper;

        public GetRoleApprovalsQueryHandler(
            IProjectProposalRepository projectProposalRepository,
            IProjectApprovalStepRepository stepRepository,
            IMapper mapper)
        {
            _projectProposalRepository = projectProposalRepository;
            _stepRepository = stepRepository;
            _mapper = mapper;
        }

        public async Task<List<ProjectResponse>> Handle(GetRoleApprovalsQuery query, CancellationToken cancellationToken)
        {
            if (query.Role < 0 || query.Role > 999)
            {
                throw new BadRequestException("El rol no puede ser menor a 0 ni mayor a 999.");
            }

            var allSteps = await _stepRepository.GetAll();
            var targetProjectIds = allSteps
                .Where(s => s.Status == (int)ApprovalStatusEnum.Pending || s.Status == (int)ApprovalStatusEnum.Observed)
                .GroupBy(s => s.ProjectProposalId)
                .Select(g => g.OrderBy(s => s.StepOrder).FirstOrDefault())
                .Where(s => s != null && s.ApproverRoleId == query.Role)
                .Select(s => s!.ProjectProposalId)
                .Distinct()
                .ToList();

            var results = new List<ProjectResponse>();
            foreach (var projectId in targetProjectIds)
            {
                var project = await _projectProposalRepository.GetById(projectId);
                if (project != null)
                {
                    results.Add(_mapper.Map<ProjectResponse>(project));
                }
            }

            return results;
        }
    }
}
