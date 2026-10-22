using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Application.Common.Mappings;
using Application.Dtos.Requests;
using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Features.ProjectProposals.Commands.UpdateProjectProposal;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Features.ProjectProposals.Commands
{
    public class UpdateProjectProposalCommandHandlerTests
    {
        private readonly Mock<IProjectProposalRepository> _projectRepositoryMock;
        private readonly IMapper _mapper;
        private readonly UpdateProjectProposalCommandHandler _handler;

        public UpdateProjectProposalCommandHandlerTests()
        {
            _projectRepositoryMock = new Mock<IProjectProposalRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommonMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<ProjectApprovalStepMappingProfile>();
                cfg.AddProfile<ProjectProposalMappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();

            _handler = new UpdateProjectProposalCommandHandler(_projectRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_WhenIdIsEmpty_ThrowsBadRequestException()
        {
            var command = new UpdateProjectProposalCommand(Guid.Empty, new ProjectUpdateRequest());

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El ID del proyecto no puede estar vacío.");
        }

        [Fact]
        public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
        {
            var projectId = Guid.NewGuid();
            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync((ProjectProposal?)null);

            var command = new UpdateProjectProposalCommand(projectId, new ProjectUpdateRequest
            {
                Title = "Nuevo Titulo"
            });

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("El proyecto no se encuentra.");
        }

        [Fact]
        public async Task Handle_WhenNewTitleAlreadyExistsOnAnotherProject_ThrowsBadRequestException()
        {
            var projectId = Guid.NewGuid();
            var anotherProjectId = Guid.NewGuid();

            var existingProject = new ProjectProposal
            {
                Id = projectId,
                Title = "Titulo Original",
                Status = (int)ApprovalStatusEnum.Observed
            };

            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(existingProject);
            _projectRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectProposal>
            {
                new ProjectProposal { Id = anotherProjectId, Title = "Titulo Duplicado" },
                existingProject
            });

            var command = new UpdateProjectProposalCommand(projectId, new ProjectUpdateRequest
            {
                Title = "Titulo Duplicado"
            });

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Ya existe un proyecto con el mismo título.");
        }

        [Fact]
        public async Task Handle_WhenProjectIsNotInObservedStatus_ThrowsBadRequestException()
        {
            var projectId = Guid.NewGuid();
            var existingProject = new ProjectProposal
            {
                Id = projectId,
                Title = "Proyecto en Revision",
                Status = (int)ApprovalStatusEnum.Pending
            };

            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(existingProject);

            var command = new UpdateProjectProposalCommand(projectId, new ProjectUpdateRequest
            {
                Title = "Proyecto en Revision",
                Description = "Nueva descripcion"
            });

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El proyecto se encuentra en un estado que no permite modificaciones.");
        }

        [Fact]
        public async Task Handle_WhenProjectIsObservedAndDataIsValid_UpdatesSuccessfully()
        {
            var projectId = Guid.NewGuid();
            var project = new ProjectProposal
            {
                Id = projectId,
                Title = "Proyecto Original",
                Description = "Descripcion Antigua",
                EstimatedDuration = 3,
                Status = (int)ApprovalStatusEnum.Observed,
                ProjectProposalArea = new Area { Id = 1, Name = "Sistemas" },
                ProjectProposalType = new ProjectType { Id = 1, Name = "Desarrollo" },
                ProjectProposalApprovalStatus = new ApprovalStatus("Observed") { Id = 4 },
                ProjectProposalUser = new User { Id = 1, Name = "Admin" }
            };

            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            _projectRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectProposal> { project });
            _projectRepositoryMock.Setup(r => r.Update(It.IsAny<ProjectProposal>())).ReturnsAsync(true);

            var command = new UpdateProjectProposalCommand(projectId, new ProjectUpdateRequest
            {
                Title = "Proyecto Modificado",
                Description = "Nueva Descripcion",
                Duration = 6
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            project.Title.Should().Be("Proyecto Modificado");
            project.Description.Should().Be("Nueva Descripcion");
            project.EstimatedDuration.Should().Be(6);

            _projectRepositoryMock.Verify(r => r.Update(project), Times.Once);
        }
    }
}
