using System;
using MediatR;
using Application.Dtos.Requests;
using Application.Dtos.Responses;

namespace Application.Features.ProjectProposals.Commands.UpdateProjectProposal
{
    public record UpdateProjectProposalCommand(Guid Id, ProjectUpdateRequest Request) : IRequest<ProjectResponse>;
}
