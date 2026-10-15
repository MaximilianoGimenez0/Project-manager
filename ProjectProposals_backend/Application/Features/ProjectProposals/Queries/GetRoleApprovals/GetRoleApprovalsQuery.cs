using System.Collections.Generic;
using MediatR;
using Application.Dtos.Responses;

namespace Application.Features.ProjectProposals.Queries.GetRoleApprovals
{
    public record GetRoleApprovalsQuery(int Role) : IRequest<List<ProjectResponse>>;
}
