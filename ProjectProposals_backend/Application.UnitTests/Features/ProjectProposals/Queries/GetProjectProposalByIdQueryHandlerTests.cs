using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Application.Common.Mappings;
using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Features.ProjectProposals.Queries.GetProjectProposalById;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Features.ProjectProposals.Queries
{
    public class GetProjectProposalByIdQueryHandlerTests
    {
        private readonly Mock<IProjectProposalRepository> _repositoryMock;
        private readonly IMapper _mapper;
        private readonly GetProjectProposalByIdQueryHandler _handler;

        public GetProjectProposalByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IProjectProposalRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommonMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<ProjectApprovalStepMappingProfile>();
                cfg.AddProfile<ProjectProposalMappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();

            _handler = new GetProjectProposalByIdQueryHandler(_repositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_WithEmptyGuid_ThrowsBadRequestException()
        {
            // Arrange
            var query = new GetProjectProposalByIdQuery(Guid.Empty);

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El ID del proyecto no puede estar vacío.");
        }

        [Fact]
        public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync((ProjectProposal?)null);

            var query = new GetProjectProposalByIdQuery(projectId);

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("El proyecto no se encuentra registrado.");
        }

        [Fact]
        public async Task Handle_WhenProjectExists_ReturnsMappedProjectResponse()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new ProjectProposal
            {
                Id = projectId,
                Title = "Proyecto Innovación",
                Description = "Descripción",
                EstimatedAmount = 75000,
                EstimatedDuration = 12,
                ProjectProposalArea = new Area { Id = 1, Name = "Tecnología" },
                ProjectProposalType = new ProjectType { Id = 2, Name = "Innovación" },
                ProjectProposalApprovalStatus = new ApprovalStatus("Pending") { Id = 1 },
                ProjectProposalUser = new User { Id = 1, Name = "Ana Lucero", Email = "alucero@unaj.com" }
            };

            _repositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);

            var query = new GetProjectProposalByIdQuery(projectId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(projectId);
            result.Title.Should().Be("Proyecto Innovación");
            result.Area.Name.Should().Be("Tecnología");
            result.Type.Name.Should().Be("Innovación");
            result.Status.Name.Should().Be("Pending");
            result.User.Name.Should().Be("Ana Lucero");
        }
    }
}
