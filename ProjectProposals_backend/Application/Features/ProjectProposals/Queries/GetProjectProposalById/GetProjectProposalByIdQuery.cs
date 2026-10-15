using System;
using MediatR;
using Application.Dtos.Responses;

namespace Application.Features.ProjectProposals.Queries.GetProjectProposalById
{
    public record GetProjectProposalByIdQuery(Guid Id) : IRequest<ProjectResponse>;
}
