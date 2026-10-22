using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Application.Common.Mappings;
using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Features.ProjectProposals.Queries.GetRoleApprovals;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Features.ProjectProposals.Queries
{
    public class GetRoleApprovalsQueryHandlerTests
    {
        private readonly Mock<IProjectProposalRepository> _projectRepoMock;
        private readonly Mock<IProjectApprovalStepRepository> _stepRepoMock;
        private readonly IMapper _mapper;
        private readonly GetRoleApprovalsQueryHandler _handler;

        public GetRoleApprovalsQueryHandlerTests()
        {
            _projectRepoMock = new Mock<IProjectProposalRepository>();
            _stepRepoMock = new Mock<IProjectApprovalStepRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommonMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<ProjectApprovalStepMappingProfile>();
                cfg.AddProfile<ProjectProposalMappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();

            _handler = new GetRoleApprovalsQueryHandler(_projectRepoMock.Object, _stepRepoMock.Object, _mapper);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1000)]
        public async Task Handle_WhenRoleOutOfRange_ThrowsBadRequestException(int invalidRole)
        {
            var query = new GetRoleApprovalsQuery(invalidRole);

            var act = () => _handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El rol no puede ser menor a 0 ni mayor a 999.");
        }

        [Fact]
        public async Task Handle_WhenMatchingPendingStepsExist_ReturnsMappedProjects()
        {
            var project1Id = Guid.NewGuid();
            var project2Id = Guid.NewGuid();

            var project1 = new ProjectProposal
            {
                Id = project1Id,
                Title = "Proyecto para Gerencia",
                Status = (int)ApprovalStatusEnum.Pending,
                EstimatedAmount = 25000,
                ProjectProposalArea = new Area { Id = 1, Name = "Finanzas" },
                ProjectProposalType = new ProjectType { Id = 1, Name = "Mejora" },
                ProjectProposalApprovalStatus = new ApprovalStatus("Pending") { Id = 1 },
                ProjectProposalUser = new User { Id = 1, Name = "José" }
            };

            var steps = new List<ProjectApprovalStep>
            {
                // Project 1 step 1 was approved, step 2 is pending Manager (Role 20) -> MATCH
                new ProjectApprovalStep { ProjectProposalId = project1Id, StepOrder = 1, Status = (int)ApprovalStatusEnum.Approved, ApproverRoleId = (int)ApproverRoleEnum.AreaLeader },
                new ProjectApprovalStep { ProjectProposalId = project1Id, StepOrder = 2, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = (int)ApproverRoleEnum.Manager },

                // Project 2 step 1 is pending AreaLeader (Role 10) -> NOT A MATCH for Manager
                new ProjectApprovalStep { ProjectProposalId = project2Id, StepOrder = 1, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = (int)ApproverRoleEnum.AreaLeader },
                new ProjectApprovalStep { ProjectProposalId = project2Id, StepOrder = 2, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = (int)ApproverRoleEnum.Manager }
            };

            _stepRepoMock.Setup(r => r.GetAll()).ReturnsAsync(steps);
            _projectRepoMock.Setup(r => r.GetById(project1Id)).ReturnsAsync(project1);

            var query = new GetRoleApprovalsQuery((int)ApproverRoleEnum.Manager);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().HaveCount(1);
            result[0].Id.Should().Be(project1Id);
            result[0].Title.Should().Be("Proyecto para Gerencia");
            result[0].Status.Name.Should().Be("Pending");
        }
    }
}
