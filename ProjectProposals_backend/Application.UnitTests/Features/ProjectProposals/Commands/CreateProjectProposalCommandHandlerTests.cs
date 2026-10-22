using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Dtos.Requests;
using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Features.ProjectProposals.Commands.CreateProjectProposal;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Features.ProjectProposals.Commands
{
    public class CreateProjectProposalCommandHandlerTests
    {
        private readonly Mock<IProjectProposalRepository> _projectRepositoryMock;
        private readonly Mock<IAreaRepository> _areaRepositoryMock;
        private readonly Mock<IProjectTypeRepository> _typeRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IApprovalStepEngine> _engineMock;
        private readonly IMapper _mapper;
        private readonly CreateProjectProposalCommandHandler _handler;

        public CreateProjectProposalCommandHandlerTests()
        {
            _projectRepositoryMock = new Mock<IProjectProposalRepository>();
            _areaRepositoryMock = new Mock<IAreaRepository>();
            _typeRepositoryMock = new Mock<IProjectTypeRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _engineMock = new Mock<IApprovalStepEngine>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommonMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<ProjectApprovalStepMappingProfile>();
                cfg.AddProfile<ProjectProposalMappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();

            _handler = new CreateProjectProposalCommandHandler(
                _projectRepositoryMock.Object,
                _areaRepositoryMock.Object,
                _typeRepositoryMock.Object,
                _userRepositoryMock.Object,
                _engineMock.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_WithValidRequest_CreatesProposalAndReturnsProjectResponse()
        {
            var request = new ProjectProposalCreateRequest
            {
                Title = "Proyecto Test",
                Description = "Descripción del proyecto",
                Amount = 25000,
                Duration = 6,
                Area = 1,
                Type = 1,
                User = 1
            };

            _areaRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new Area { Id = 1, Name = "Finanzas" });
            _typeRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new ProjectType { Id = 1, Name = "Mejora" });
            _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User { Id = 1, Name = "José", Email = "j@u.com", Role = 1 });
            _projectRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectProposal>());

            var calculatedSteps = new List<ProjectApprovalStep>
            {
                new ProjectApprovalStep
                {
                    StepOrder = 1,
                    ApproverRoleId = (int)ApproverRoleEnum.AreaLeader,
                    Status = (int)ApprovalStatusEnum.Pending
                }
            };

            _engineMock.Setup(e => e.CalculateStepsAsync(It.IsAny<ProjectProposal>()))
                .ReturnsAsync(calculatedSteps);

            _projectRepositoryMock.Setup(r => r.Add(It.IsAny<ProjectProposal>()))
                .ReturnsAsync(true);

            var createdProject = new ProjectProposal
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                EstimatedAmount = request.Amount!.Value,
                EstimatedDuration = request.Duration!.Value,
                Area = 1,
                Type = 1,
                Status = (int)ApprovalStatusEnum.Pending,
                CreateBy = 1,
                ProjectProposalArea = new Area { Id = 1, Name = "Finanzas" },
                ProjectProposalType = new ProjectType { Id = 1, Name = "Mejora" },
                ProjectProposalApprovalStatus = new ApprovalStatus("Pending") { Id = 1 },
                ProjectProposalUser = new User { Id = 1, Name = "José", Email = "j@u.com" },
                Steps = calculatedSteps
            };

            _projectRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>()))
                .ReturnsAsync(createdProject);

            var command = new CreateProjectProposalCommand(request);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Title.Should().Be("Proyecto Test");
            result.Status.Name.Should().Be("Pending");
            result.Steps.Should().HaveCount(1);
            result.Steps![0].StepOrder.Should().Be(1);

            _projectRepositoryMock.Verify(r => r.Add(It.Is<ProjectProposal>(p =>
                p.Title == request.Title &&
                p.EstimatedAmount == request.Amount &&
                p.Steps.Count == 1
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAreaDoesNotExist_ThrowsBadRequestException()
        {
            var request = new ProjectProposalCreateRequest
            {
                Title = "Proyecto Válido",
                Description = "Descripción",
                Amount = 1000,
                Duration = 1,
                Area = 99,
                Type = 1,
                User = 1
            };

            _areaRepositoryMock.Setup(r => r.GetById(99)).ReturnsAsync((Area?)null);

            var command = new CreateProjectProposalCommand(request);

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El área ingresada no existe.");
        }

        [Fact]
        public async Task Handle_WhenDuplicateTitleExists_ThrowsBadRequestException()
        {
            var request = new ProjectProposalCreateRequest
            {
                Title = "Proyecto Duplicado",
                Description = "Descripción",
                Amount = 1000,
                Duration = 1,
                Area = 1,
                Type = 1,
                User = 1
            };

            _areaRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new Area { Id = 1, Name = "Finanzas" });
            _typeRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new ProjectType { Id = 1, Name = "Mejora" });
            _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User { Id = 1, Name = "José", Email = "j@u.com" });
            _projectRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectProposal>
            {
                new ProjectProposal { Title = "proyecto duplicado" }
            });

            var command = new CreateProjectProposalCommand(request);

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Ya existe un proyecto con el mismo título.");
        }

        [Fact]
        public async Task Handle_WhenNoApprovalStepsGenerated_ThrowsBadRequestException()
        {
            var request = new ProjectProposalCreateRequest
            {
                Title = "Proyecto Sin Reglas",
                Description = "Descripción",
                Amount = 999999,
                Duration = 1,
                Area = 1,
                Type = 1,
                User = 1
            };

            _areaRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new Area { Id = 1, Name = "Finanzas" });
            _typeRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new ProjectType { Id = 1, Name = "Mejora" });
            _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User { Id = 1, Name = "José", Email = "j@u.com" });
            _projectRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectProposal>());
            _engineMock.Setup(e => e.CalculateStepsAsync(It.IsAny<ProjectProposal>()))
                .ReturnsAsync(new List<ProjectApprovalStep>());

            var command = new CreateProjectProposalCommand(request);

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("No se encontraron reglas de aprobación aplicables para los parámetros del proyecto.");
        }

        [Theory]
        [InlineData(null, "Desc", 1000.0, 6, 1, 1, 1)]
        [InlineData("Title", null, 1000.0, 6, 1, 1, 1)]
        [InlineData("Title", "Desc", null, 6, 1, 1, 1)]
        [InlineData("Title", "Desc", 1000.0, null, 1, 1, 1)]
        [InlineData("Title", "Desc", 1000.0, 6, null, 1, 1)]
        [InlineData("Title", "Desc", 1000.0, 6, 1, null, 1)]
        [InlineData("Title", "Desc", 1000.0, 6, 1, 1, null)]
        public async Task Handle_WhenRequiredFieldIsMissing_ThrowsBadRequestException(
            string? title, string? desc, double? amount, int? duration, int? area, int? type, int? user)
        {
            var request = new ProjectProposalCreateRequest
            {
                Title = title,
                Description = desc,
                Amount = amount.HasValue ? (decimal)amount.Value : null,
                Duration = duration,
                Area = area,
                Type = type,
                User = user
            };
            var command = new CreateProjectProposalCommand(request);

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Todos los campos obligatorios deben completarse.");
        }

        [Fact]
        public async Task Handle_WhenTypeDoesNotExist_ThrowsBadRequestException()
        {
            var request = new ProjectProposalCreateRequest
            {
                Title = "Proyecto Válido",
                Description = "Descripción",
                Amount = 1000,
                Duration = 1,
                Area = 1,
                Type = 999,
                User = 1
            };

            _areaRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new Area { Id = 1, Name = "Finanzas" });
            _typeRepositoryMock.Setup(r => r.GetById(999)).ReturnsAsync((ProjectType?)null);

            var command = new CreateProjectProposalCommand(request);

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El tipo de proyecto ingresado no existe.");
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ThrowsBadRequestException()
        {
            var request = new ProjectProposalCreateRequest
            {
                Title = "Proyecto Válido",
                Description = "Descripción",
                Amount = 1000,
                Duration = 1,
                Area = 1,
                Type = 1,
                User = 999
            };

            _areaRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new Area { Id = 1, Name = "Finanzas" });
            _typeRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new ProjectType { Id = 1, Name = "Mejora" });
            _userRepositoryMock.Setup(r => r.GetById(999)).ReturnsAsync((User?)null);

            var command = new CreateProjectProposalCommand(request);

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("El usuario creador ingresado no existe.");
        }

        [Fact]
        public async Task Handle_WhenCreatedEntityCannotBeRetrieved_ThrowsBadRequestException()
        {
            var request = new ProjectProposalCreateRequest
            {
                Title = "Proyecto Recuperación Fallida",
                Description = "Descripción",
                Amount = 10000,
                Duration = 2,
                Area = 1,
                Type = 1,
                User = 1
            };

            _areaRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new Area { Id = 1, Name = "Finanzas" });
            _typeRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new ProjectType { Id = 1, Name = "Mejora" });
            _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User { Id = 1, Name = "José" });
            _projectRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectProposal>());
            _engineMock.Setup(e => e.CalculateStepsAsync(It.IsAny<ProjectProposal>()))
                .ReturnsAsync(new List<ProjectApprovalStep> { new ProjectApprovalStep { StepOrder = 1, ApproverRoleId = 10 } });
            _projectRepositoryMock.Setup(r => r.Add(It.IsAny<ProjectProposal>())).ReturnsAsync(true);
            _projectRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((ProjectProposal?)null);

            var command = new CreateProjectProposalCommand(request);

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Error al recuperar la propuesta creada.");
        }
    }
}
