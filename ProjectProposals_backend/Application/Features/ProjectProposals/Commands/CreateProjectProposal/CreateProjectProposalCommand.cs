using MediatR;
using Application.Dtos.Requests;
using Application.Dtos.Responses;

namespace Application.Features.ProjectProposals.Commands.CreateProjectProposal
{
    public record CreateProjectProposalCommand(ProjectProposalCreateRequest Request) : IRequest<ProjectResponse>;
}
