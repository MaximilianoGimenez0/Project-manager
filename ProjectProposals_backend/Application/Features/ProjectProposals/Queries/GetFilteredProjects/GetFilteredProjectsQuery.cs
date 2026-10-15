using System.Collections.Generic;
using MediatR;
using Application.Dtos.Responses;

namespace Application.Features.ProjectProposals.Queries.GetFilteredProjects
{
    public record GetFilteredProjectsQuery(
        string? Title,
        int? Status,
        int? Applicant,
        int? ApprovalUser) : IRequest<List<ProjectShortResponse>>;
}
