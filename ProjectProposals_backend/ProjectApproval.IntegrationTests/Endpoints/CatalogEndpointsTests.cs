using System.Net;
using System.Net.Http.Json;
using Application.Dtos.Responses;
using FluentAssertions;
using ProjectApproval.IntegrationTests.Common;
using Xunit;

namespace ProjectApproval.IntegrationTests.Endpoints;

public class CatalogEndpointsTests : BaseIntegrationTest
{
    public CatalogEndpointsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task AreaGetAll_WhenAuthenticated_ReturnsAreasList()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync("jferreyra@unaj.com", "Password123!");

        // Act
        var response = await authClient.GetAsync("/api/Area");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var areas = await response.Content.ReadFromJsonAsync<List<GenericResponse>>();
        areas.Should().NotBeNull();
        areas!.Should().NotBeEmpty();
        areas.Should().Contain(a => a.Name == "Finanzas");
    }

    [Fact]
    public async Task ProjectTypeGetAll_WhenAuthenticated_ReturnsTypesList()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync("jferreyra@unaj.com", "Password123!");

        // Act
        var response = await authClient.GetAsync("/api/ProjectType");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var types = await response.Content.ReadFromJsonAsync<List<GenericResponse>>();
        types.Should().NotBeNull();
        types!.Should().NotBeEmpty();
        types.Should().Contain(t => t.Name == "Mejora de Procesos");
    }

    [Fact]
    public async Task RoleGetAll_WhenAuthenticated_ReturnsRolesList()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync("jferreyra@unaj.com", "Password123!");

        // Act
        var response = await authClient.GetAsync("/api/Role");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var roles = await response.Content.ReadFromJsonAsync<List<GenericResponse>>();
        roles.Should().NotBeNull();
        roles!.Should().NotBeEmpty();
        roles.Should().Contain(r => r.Name == "Gerente");
    }

    [Fact]
    public async Task StatusGetAll_WhenAuthenticated_ReturnsApprovalStatusesList()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync("jferreyra@unaj.com", "Password123!");

        // Act
        var response = await authClient.GetAsync("/api/ApprovalStatus");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var statuses = await response.Content.ReadFromJsonAsync<List<GenericResponse>>();
        statuses.Should().NotBeNull();
        statuses!.Should().NotBeEmpty();
        statuses.Should().Contain(s => s.Name == "Pending");
    }

    [Fact]
    public async Task UserGetAll_WhenAuthenticated_ReturnsUsersList()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync("jferreyra@unaj.com", "Password123!");

        // Act
        var response = await authClient.GetAsync("/api/User");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        users.Should().NotBeNull();
        users!.Should().NotBeEmpty();
        users.Should().Contain(u => u.Email == "jferreyra@unaj.com");
    }

    [Fact]
    public async Task Catalogs_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/Area");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
