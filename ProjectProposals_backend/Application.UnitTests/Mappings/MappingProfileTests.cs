using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Application.Common.Mappings;
using Application.Dtos.Responses;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Application.UnitTests.Mappings
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommonMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<ProjectApprovalStepMappingProfile>();
                cfg.AddProfile<ProjectProposalMappingProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void AutoMapper_Configuration_ShouldBeValid()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommonMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<ProjectApprovalStepMappingProfile>();
                cfg.AddProfile<ProjectProposalMappingProfile>();
            }, NullLoggerFactory.Instance);

            config.AssertConfigurationIsValid();
        }

        [Fact]
        public void AutoMapper_Profiles_ShouldMapProjectProposalToProjectResponseCorrectly()
        {
            var entity = new ProjectProposal
            {
                Title = "Título Proyecto",
                Description = "Descripción",
                ProjectProposalArea = new Area { Name = "Finanzas" },
                ProjectProposalType = new ProjectType { Name = "Infraestructura" },
                ProjectProposalApprovalStatus = new ApprovalStatus("Approved") { Id = 2 },
                ProjectProposalUser = new User { Name = "José Ferreyra" }
            };

            var dto = _mapper.Map<ProjectResponse>(entity);

            dto.Title.Should().Be("Título Proyecto");
            dto.Area.Name.Should().Be("Finanzas");
            dto.Type.Name.Should().Be("Infraestructura");
            dto.Status.Name.Should().Be("Approved");
            dto.User.Name.Should().Be("José Ferreyra");
        }

        [Fact]
        public void AutoMapper_Profiles_ShouldMapApprovalStepToApprovalStepResponseCorrectly()
        {
            var step = new ProjectApprovalStep
            {
                StepOrder = 1,
                StepApproverRole = new ApproverRole { Name = "Gerente" },
                StepApprovalStatus = new ApprovalStatus("Approved") { Id = 2 },
                StepUser = new User { Name = "Ana Lucero" },
                Observations = "Sin observaciones"
            };

            var dto = _mapper.Map<ApprovalStepResponse>(step);

            dto.StepOrder.Should().Be(1);
            dto.ApproverRole.Name.Should().Be("Gerente");
            dto.Status.Name.Should().Be("Approved");
            dto.ApproverUser!.Name.Should().Be("Ana Lucero");
            dto.Observations.Should().Be("Sin observaciones");
        }
    }
}
