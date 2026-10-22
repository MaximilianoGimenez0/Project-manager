using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Features.ProjectProposals.Queries.GetFilteredProjects;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Features.ProjectProposals.Queries
{
    public class GetFilteredProjectsQueryHandlerTests
    {
        private readonly Mock<IProjectProposalRepository> _projectRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IProjectApprovalStepRepository> _stepRepoMock;
        private readonly Mock<IAreaRepository> _areaRepoMock;
        private readonly Mock<IApprovalStatusRepository> _statusRepoMock;
        private readonly Mock<IProjectTypeRepository> _typeRepoMock;
        private readonly GetFilteredProjectsQueryHandler _handler;

        public GetFilteredProjectsQueryHandlerTests()
        {
            _projectRepoMock = new Mock<IProjectProposalRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _stepRepoMock = new Mock<IProjectApprovalStepRepository>();
            _areaRepoMock = new Mock<IAreaRepository>();
            _statusRepoMock = new Mock<IApprovalStatusRepository>();
            _typeRepoMock = new Mock<IProjectTypeRepository>();

            // Setup default catalogs
            _areaRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Area>
            {
                new Area { Id = 1, Name = "Tecnología" },
                new Area { Id = 2, Name = "Finanzas" }
            });
            _statusRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ApprovalStatus>
            {
                new ApprovalStatus("Pending") { Id = 1 },
                new ApprovalStatus("Approved") { Id = 2 },
                new ApprovalStatus("Rejected") { Id = 3 },
                new ApprovalStatus("Observed") { Id = 4 }
            });
            _typeRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectType>
            {
                new ProjectType { Id = 1, Name = "Innovación" },
                new ProjectType { Id = 2, Name = "Mantenimiento" }
            });

            _handler = new GetFilteredProjectsQueryHandler(
                _projectRepoMock.Object,
                _userRepoMock.Object,
                _stepRepoMock.Object,
                _areaRepoMock.Object,
                _statusRepoMock.Object,
                _typeRepoMock.Object);
        }

        [Fact]
        public async Task Handle_WhenApprovalUserDoesNotExist_ThrowsBadRequestException()
        {
            // Arrange
            _projectRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectProposal>());
            _userRepoMock.Setup(r => r.GetById(99)).ReturnsAsync((User?)null);

            var query = new GetFilteredProjectsQuery(Title: null, Status: null, Applicant: null, ApprovalUser: 99);

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El usuario no existe.");
        }

        [Fact]
        public async Task Handle_WhenApprovalUserProvided_FiltersProjectsAwaitingUserRole()
        {
            // Arrange
            var user = new User { Id = 5, Name = "Gerente Test", Role = (int)ApproverRoleEnum.Manager };
            _userRepoMock.Setup(r => r.GetById(5)).ReturnsAsync(user);

            var project1Id = Guid.NewGuid();
            var project2Id = Guid.NewGuid();

            var project1 = new ProjectProposal { Id = project1Id, Title = "Proyecto Alpha", Status = 1, Area = 1, Type = 1 };
            var project2 = new ProjectProposal { Id = project2Id, Title = "Proyecto Beta", Status = 1, Area = 1, Type = 1 };

            _projectRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectProposal> { project1, project2 });

            // Project 1 next pending step is Manager (Role 20) -> should match
            // Project 2 next pending step is Director (Role 30) -> should NOT match
            var steps = new List<ProjectApprovalStep>
            {
                new ProjectApprovalStep { ProjectProposalId = project1Id, StepOrder = 1, Status = (int)ApprovalStatusEnum.Approved, ApproverRoleId = (int)ApproverRoleEnum.AreaLeader },
                new ProjectApprovalStep { ProjectProposalId = project1Id, StepOrder = 2, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = (int)ApproverRoleEnum.Manager },
                new ProjectApprovalStep { ProjectProposalId = project2Id, StepOrder = 1, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = (int)ApproverRoleEnum.Director }
            };

            _stepRepoMock.Setup(r => r.GetAll()).ReturnsAsync(steps);

            var query = new GetFilteredProjectsQuery(Title: null, Status: null, Applicant: null, ApprovalUser: 5);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(project1Id);
            result[0].Title.Should().Be("Proyecto Alpha");
            result[0].Area.Should().Be("Tecnología");
            result[0].Status.Should().Be("Pending");
            result[0].Type.Should().Be("Innovación");
        }

        [Fact]
        public async Task Handle_WhenFiltersByTitleStatusAndApplicant_ReturnsMatchingProjects()
        {
            // Arrange
            var project1 = new ProjectProposal { Id = Guid.NewGuid(), Title = "Sistema Contable", Status = 1, CreateBy = 10, Area = 2, Type = 1 };
            var project2 = new ProjectProposal { Id = Guid.NewGuid(), Title = "Sistema RRHH", Status = 2, CreateBy = 10, Area = 1, Type = 2 };
            var project3 = new ProjectProposal { Id = Guid.NewGuid(), Title = "Migración Cloud", Status = 1, CreateBy = 20, Area = 1, Type = 1 };

            _projectRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectProposal> { project1, project2, project3 });

            var query = new GetFilteredProjectsQuery(Title: "sistema", Status: 1, Applicant: 10, ApprovalUser: null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result[0].Title.Should().Be("Sistema Contable");
            result[0].Area.Should().Be("Finanzas");
        }

        [Fact]
        public async Task Handle_WhenNoFiltersApplied_ReturnsAllProjectsMapped()
        {
            // Arrange
            var project1 = new ProjectProposal { Id = Guid.NewGuid(), Title = "P1", Status = 1, Area = 1, Type = 1, EstimatedAmount = 10000, EstimatedDuration = 2 };
            var project2 = new ProjectProposal { Id = Guid.NewGuid(), Title = "P2", Status = 2, Area = 2, Type = 2, EstimatedAmount = 20000, EstimatedDuration = 4 };

            _projectRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectProposal> { project1, project2 });

            var query = new GetFilteredProjectsQuery(Title: null, Status: null, Applicant: null, ApprovalUser: null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result[0].Title.Should().Be("P1");
            result[0].Amount.Should().Be(10000);
            result[1].Title.Should().Be("P2");
            result[1].Duration.Should().Be(4);
        }
    }
}
