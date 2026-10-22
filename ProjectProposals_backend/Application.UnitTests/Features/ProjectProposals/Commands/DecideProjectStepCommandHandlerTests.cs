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
using Application.Features.ProjectProposals.Commands.DecideProjectStep;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Features.ProjectProposals.Commands
{
    public class DecideProjectStepCommandHandlerTests
    {
        private readonly Mock<IProjectProposalRepository> _projectRepositoryMock;
        private readonly Mock<IProjectApprovalStepRepository> _stepRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IMapper _mapper;
        private readonly DecideProjectStepCommandHandler _handler;

        public DecideProjectStepCommandHandlerTests()
        {
            _projectRepositoryMock = new Mock<IProjectProposalRepository>();
            _stepRepositoryMock = new Mock<IProjectApprovalStepRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommonMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<ProjectApprovalStepMappingProfile>();
                cfg.AddProfile<ProjectProposalMappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();

            _handler = new DecideProjectStepCommandHandler(
                _projectRepositoryMock.Object,
                _stepRepositoryMock.Object,
                _userRepositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_WhenProjectDoesNotExist_ThrowsBadRequestException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync((ProjectProposal?)null);

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 1,
                User = 1,
                Status = (int)ApprovalStatusEnum.Approved
            });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El proyecto ingresado no existe.");
        }

        [Fact]
        public async Task Handle_WhenProjectIsAlreadyApproved_ThrowsConflictException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new ProjectProposal
            {
                Id = projectId,
                Status = (int)ApprovalStatusEnum.Approved
            };
            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 1,
                User = 1,
                Status = (int)ApprovalStatusEnum.Approved
            });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("El proyecto ya no se encuentra en un estado que permite modificaciones.");
        }

        [Fact]
        public async Task Handle_WhenRejectingStep_SetsProjectStatusToRejected()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var step1 = new ProjectApprovalStep
            {
                Id = 10,
                StepOrder = 1,
                ApproverRoleId = (int)ApproverRoleEnum.AreaLeader,
                Status = (int)ApprovalStatusEnum.Pending
            };
            var project = new ProjectProposal
            {
                Id = projectId,
                Status = (int)ApprovalStatusEnum.Pending,
                Steps = new List<ProjectApprovalStep> { step1 }
            };

            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            _userRepositoryMock.Setup(r => r.GetById(2)).ReturnsAsync(new User
            {
                Id = 2,
                Name = "Ana",
                Role = (int)ApproverRoleEnum.AreaLeader
            });
            _stepRepositoryMock.Setup(r => r.Update(It.IsAny<ProjectApprovalStep>())).ReturnsAsync(true);
            _projectRepositoryMock.Setup(r => r.Update(It.IsAny<ProjectProposal>())).ReturnsAsync(true);

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 10,
                User = 2,
                Status = (int)ApprovalStatusEnum.Rejected,
                Observation = "Presupuesto inviable"
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            step1.Status.Should().Be((int)ApprovalStatusEnum.Rejected);
            step1.ApproverUserId.Should().Be(2);
            step1.Observations.Should().Be("Presupuesto inviable");
            step1.DecisionDate.Should().NotBeNull();

            project.Status.Should().Be((int)ApprovalStatusEnum.Rejected);

            _stepRepositoryMock.Verify(r => r.Update(step1), Times.Once);
            _projectRepositoryMock.Verify(r => r.Update(project), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenApprovingLastStep_SetsProjectStatusToApproved()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var step1 = new ProjectApprovalStep
            {
                Id = 1,
                StepOrder = 1,
                ApproverRoleId = (int)ApproverRoleEnum.AreaLeader,
                Status = (int)ApprovalStatusEnum.Approved
            };
            var step2 = new ProjectApprovalStep
            {
                Id = 2,
                StepOrder = 2,
                ApproverRoleId = (int)ApproverRoleEnum.Manager,
                Status = (int)ApprovalStatusEnum.Pending
            };
            var project = new ProjectProposal
            {
                Id = projectId,
                Status = (int)ApprovalStatusEnum.Pending,
                Steps = new List<ProjectApprovalStep> { step1, step2 }
            };

            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            _userRepositoryMock.Setup(r => r.GetById(3)).ReturnsAsync(new User
            {
                Id = 3,
                Name = "Gonzalo",
                Role = (int)ApproverRoleEnum.Manager
            });
            _stepRepositoryMock.Setup(r => r.Update(It.IsAny<ProjectApprovalStep>())).ReturnsAsync(true);
            _projectRepositoryMock.Setup(r => r.Update(It.IsAny<ProjectProposal>())).ReturnsAsync(true);

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 2,
                User = 3,
                Status = (int)ApprovalStatusEnum.Approved,
                Observation = "OK"
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            step2.Status.Should().Be((int)ApprovalStatusEnum.Approved);
            project.Status.Should().Be((int)ApprovalStatusEnum.Approved);
        }

        [Fact]
        public async Task Handle_WhenCreatorAttemptsToApproveOwnStep_ThrowsConflictException()
        {
            // Arrange: Project created by User 2, and User 2 tries to decide step
            var projectId = Guid.NewGuid();
            var step1 = new ProjectApprovalStep
            {
                Id = 1,
                StepOrder = 1,
                ApproverRoleId = (int)ApproverRoleEnum.AreaLeader,
                Status = (int)ApprovalStatusEnum.Pending
            };
            var project = new ProjectProposal
            {
                Id = projectId,
                Status = (int)ApprovalStatusEnum.Pending,
                CreateBy = 2,
                Steps = new List<ProjectApprovalStep> { step1 }
            };

            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            _userRepositoryMock.Setup(r => r.GetById(2)).ReturnsAsync(new User
            {
                Id = 2,
                Name = "Ana",
                Role = (int)ApproverRoleEnum.AreaLeader
            });

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 1,
                User = 2,
                Status = (int)ApprovalStatusEnum.Approved
            });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("El usuario creador de la propuesta no puede actuar como aprobador de la misma.");
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsEmpty_ThrowsBadRequestException()
        {
            // Arrange
            var command = new DecideProjectStepCommand(Guid.Empty, new DecisionStepUpdateRequest());

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El ID del proyecto no puede estar vacío.");
        }

        [Fact]
        public async Task Handle_WhenDecidingUserDoesNotExist_ThrowsBadRequestException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new ProjectProposal { Id = projectId, Status = (int)ApprovalStatusEnum.Pending };
            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            _userRepositoryMock.Setup(r => r.GetById(99)).ReturnsAsync((User?)null);

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 1,
                User = 99,
                Status = (int)ApprovalStatusEnum.Approved
            });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El usuario ingresado no existe.");
        }

        [Fact]
        public async Task Handle_WhenNoPendingStepsExist_ThrowsBadRequestException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new ProjectProposal
            {
                Id = projectId,
                Status = (int)ApprovalStatusEnum.Pending,
                Steps = new List<ProjectApprovalStep>() // Sin pasos pendientes
            };
            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User { Id = 1, Role = (int)ApproverRoleEnum.AreaLeader });

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 1,
                User = 1,
                Status = (int)ApprovalStatusEnum.Approved
            });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El proyecto no tiene un paso pendiente de aprobación.");
        }

        [Fact]
        public async Task Handle_WhenStepIdDoesNotMatchNextPendingStep_ThrowsBadRequestException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var step1 = new ProjectApprovalStep { Id = 10, StepOrder = 1, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = 10 };
            var step2 = new ProjectApprovalStep { Id = 20, StepOrder = 2, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = 20 };
            var project = new ProjectProposal
            {
                Id = projectId,
                Status = (int)ApprovalStatusEnum.Pending,
                Steps = new List<ProjectApprovalStep> { step1, step2 }
            };
            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User { Id = 1, Role = 20 });

            // User tries to approve step 20 while step 10 is the next pending step
            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 20,
                User = 1,
                Status = (int)ApprovalStatusEnum.Approved
            });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El id del paso no corresponde con el próximo paso pendiente de aprobación.");
        }

        [Fact]
        public async Task Handle_WhenUserRoleDoesNotMatchRequiredApproverRole_ThrowsBadRequestException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var step1 = new ProjectApprovalStep { Id = 10, StepOrder = 1, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = (int)ApproverRoleEnum.Director };
            var project = new ProjectProposal
            {
                Id = projectId,
                Status = (int)ApprovalStatusEnum.Pending,
                Steps = new List<ProjectApprovalStep> { step1 }
            };
            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            // User is AreaLeader (10), but step requires Director (30)
            _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User { Id = 1, Role = (int)ApproverRoleEnum.AreaLeader });

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 10,
                User = 1,
                Status = (int)ApprovalStatusEnum.Approved
            });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El rol del usuario no corresponde, no puede aprobar este paso.");
        }

        [Fact]
        public async Task Handle_WhenStepUpdateFails_ThrowsBadRequestException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var step1 = new ProjectApprovalStep { Id = 10, StepOrder = 1, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = 10 };
            var project = new ProjectProposal
            {
                Id = projectId,
                Status = (int)ApprovalStatusEnum.Pending,
                Steps = new List<ProjectApprovalStep> { step1 }
            };
            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User { Id = 1, Role = 10 });
            _stepRepositoryMock.Setup(r => r.Update(It.IsAny<ProjectApprovalStep>())).ReturnsAsync(false); // Update fails

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 10,
                User = 1,
                Status = (int)ApprovalStatusEnum.Approved
            });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Datos de decisión inválidos.");
        }

        [Fact]
        public async Task Handle_WhenObservingStep_SetsProjectStatusToObserved()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var step1 = new ProjectApprovalStep { Id = 10, StepOrder = 1, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = 10 };
            var project = new ProjectProposal
            {
                Id = projectId,
                Status = (int)ApprovalStatusEnum.Pending,
                Steps = new List<ProjectApprovalStep> { step1 }
            };
            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User { Id = 1, Role = 10 });
            _stepRepositoryMock.Setup(r => r.Update(It.IsAny<ProjectApprovalStep>())).ReturnsAsync(true);
            _projectRepositoryMock.Setup(r => r.Update(It.IsAny<ProjectProposal>())).ReturnsAsync(true);

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 10,
                User = 1,
                Status = (int)ApprovalStatusEnum.Observed,
                Observation = "Requiere ajustar presupuesto"
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            step1.Status.Should().Be((int)ApprovalStatusEnum.Observed);
            project.Status.Should().Be((int)ApprovalStatusEnum.Observed);
        }

        [Fact]
        public async Task Handle_WhenApprovingIntermediateStep_KeepsProjectStatusPending()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var step1 = new ProjectApprovalStep { Id = 10, StepOrder = 1, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = 10 };
            var step2 = new ProjectApprovalStep { Id = 20, StepOrder = 2, Status = (int)ApprovalStatusEnum.Pending, ApproverRoleId = 20 };
            var project = new ProjectProposal
            {
                Id = projectId,
                Status = (int)ApprovalStatusEnum.Pending,
                Steps = new List<ProjectApprovalStep> { step1, step2 }
            };
            _projectRepositoryMock.Setup(r => r.GetById(projectId)).ReturnsAsync(project);
            _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User { Id = 1, Role = 10 });
            _stepRepositoryMock.Setup(r => r.Update(It.IsAny<ProjectApprovalStep>())).ReturnsAsync(true);
            _projectRepositoryMock.Setup(r => r.Update(It.IsAny<ProjectProposal>())).ReturnsAsync(true);

            var command = new DecideProjectStepCommand(projectId, new DecisionStepUpdateRequest
            {
                Id = 10,
                User = 1,
                Status = (int)ApprovalStatusEnum.Approved,
                Observation = "Paso 1 aprobado"
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            step1.Status.Should().Be((int)ApprovalStatusEnum.Approved);
            step2.Status.Should().Be((int)ApprovalStatusEnum.Pending);
            project.Status.Should().Be((int)ApprovalStatusEnum.Pending); // Still pending because step 2 is pending
        }
    }
}
