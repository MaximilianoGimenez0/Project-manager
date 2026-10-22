using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Application.Common.Mappings;
using Application.Dtos.Responses;
using Application.Features.Catalogs.Queries;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Features.Catalogs
{
    public class CatalogQueryHandlersTests
    {
        private readonly Mock<IAreaRepository> _areaRepoMock;
        private readonly Mock<IProjectTypeRepository> _typeRepoMock;
        private readonly Mock<IApproverRoleRepository> _roleRepoMock;
        private readonly Mock<IApprovalStatusRepository> _statusRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly IMapper _mapper;
        private readonly CatalogQueryHandlers _handler;

        public CatalogQueryHandlersTests()
        {
            _areaRepoMock = new Mock<IAreaRepository>();
            _typeRepoMock = new Mock<IProjectTypeRepository>();
            _roleRepoMock = new Mock<IApproverRoleRepository>();
            _statusRepoMock = new Mock<IApprovalStatusRepository>();
            _userRepoMock = new Mock<IUserRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommonMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();

            _handler = new CatalogQueryHandlers(
                _areaRepoMock.Object,
                _typeRepoMock.Object,
                _roleRepoMock.Object,
                _statusRepoMock.Object,
                _userRepoMock.Object,
                _mapper);
        }

        [Fact]
        public async Task GetAreasQuery_ReturnsMappedAreas()
        {
            _areaRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Area>
            {
                new Area { Id = 1, Name = "Tecnología" },
                new Area { Id = 2, Name = "Finanzas" }
            });

            var result = await _handler.Handle(new GetAreasQuery(), CancellationToken.None);

            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Tecnología");
            result[1].Name.Should().Be("Finanzas");
        }

        [Fact]
        public async Task GetProjectTypesQuery_ReturnsMappedTypes()
        {
            _typeRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ProjectType>
            {
                new ProjectType { Id = 1, Name = "Innovación" }
            });

            var result = await _handler.Handle(new GetProjectTypesQuery(), CancellationToken.None);

            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Innovación");
        }

        [Fact]
        public async Task GetApproverRolesQuery_ReturnsMappedRoles()
        {
            _roleRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ApproverRole>
            {
                new ApproverRole { Id = 1, Name = "Líder de Área" },
                new ApproverRole { Id = 2, Name = "Gerente" }
            });

            var result = await _handler.Handle(new GetApproverRolesQuery(), CancellationToken.None);

            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Líder de Área");
        }

        [Fact]
        public async Task GetApprovalStatusesQuery_ReturnsMappedStatuses()
        {
            _statusRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ApprovalStatus>
            {
                new ApprovalStatus("Pending") { Id = 1 },
                new ApprovalStatus("Approved") { Id = 2 }
            });

            var result = await _handler.Handle(new GetApprovalStatusesQuery(), CancellationToken.None);

            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Pending");
            result[1].Name.Should().Be("Approved");
        }

        [Fact]
        public async Task GetUsersQuery_ReturnsMappedUsers()
        {
            _userRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User>
            {
                new User { Id = 1, Name = "Ana", Email = "ana@unaj.com", Role = 1 }
            });

            var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Ana");
            result[0].Email.Should().Be("ana@unaj.com");
        }
    }
}
