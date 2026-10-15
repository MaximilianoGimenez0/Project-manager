using System;
using MediatR;
using Application.Dtos.Requests;
using Application.Dtos.Responses;

namespace Application.Features.ProjectProposals.Commands.DecideProjectStep
{
    public record DecideProjectStepCommand(Guid ProjectId, DecisionStepUpdateRequest Request) : IRequest<ProjectResponse>;
}
