using System.Net;
using System.Net.Http.Json;
using Application.Dtos.Requests;
using Application.Dtos.Responses;
using FluentAssertions;
using ProjectApproval.IntegrationTests.Common;
using Xunit;

namespace ProjectApproval.IntegrationTests.Endpoints;

public class ProjectProposalEndpointsTests : BaseIntegrationTest
{
    public ProjectProposalEndpointsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateProjectProposal_WhenAuthenticated_ReturnsCreatedAndGeneratesSteps()
    {
        // Arrange: Login as User 2 (alucero@unaj.com, Líder de Área)
        var (authClient, _) = await CreateAuthenticatedClientAsync("alucero@unaj.com", "Password123!");
        var uniqueTitle = $"Integration Test Project {Guid.NewGuid():N}";
        var request = new ProjectProposalCreateRequest
        {
            Title = uniqueTitle,
            Description = "Automated integration test proposal description",
            Amount = 15000,
            Duration = 6,
            Area = 1, // Finanzas
            Type = 1, // Mejora de Procesos
            User = 2
        };

        // Act
        var response = await authClient.PostAsJsonAsync("/api/project", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        created.Should().NotBeNull();
        created!.Id.Should().NotBeEmpty();
        created.Title.Should().Be(uniqueTitle);
        created.Amount.Should().Be(15000);
        created.Steps.Should().NotBeNull();
        created.Steps!.Count.Should().BeGreaterThan(0, "Approval rules must generate steps for valid proposals");
    }

    [Fact]
    public async Task CreateProjectProposal_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Act: without token
        var request = new ProjectProposalCreateRequest
        {
            Title = "Unauthorized Project",
            Description = "Should be rejected",
            Amount = 10000,
            Duration = 3,
            Area = 1,
            Type = 1,
            User = 1
        };

        var response = await Client.PostAsJsonAsync("/api/project", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WhenProposalExists_ReturnsOkWithDetails()
    {
        // Arrange: Create proposal first
        var (authClient, _) = await CreateAuthenticatedClientAsync("alucero@unaj.com", "Password123!");
        var uniqueTitle = $"GetById Test Project {Guid.NewGuid():N}";
        var createResponse = await authClient.PostAsJsonAsync("/api/project", new ProjectProposalCreateRequest
        {
            Title = uniqueTitle,
            Description = "Detailed description",
            Amount = 8000,
            Duration = 4,
            Area = 1,
            Type = 1,
            User = 2
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = (await createResponse.Content.ReadFromJsonAsync<ProjectResponse>())!;

        // Act
        var getResponse = await authClient.GetAsync($"/api/project/{created.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ProjectResponse>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Title.Should().Be(uniqueTitle);
    }

    [Fact]
    public async Task GetById_WhenProposalDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync("alucero@unaj.com", "Password123!");
        var randomId = Guid.NewGuid();

        // Act
        var response = await authClient.GetAsync($"/api/project/{randomId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFiltered_WhenQueryMatches_ReturnsFilteredList()
    {
        // Arrange: Create a project with distinct title
        var (authClient, _) = await CreateAuthenticatedClientAsync("alucero@unaj.com", "Password123!");
        var prefix = $"FilterKey_{Guid.NewGuid():N}";
        await authClient.PostAsJsonAsync("/api/project", new ProjectProposalCreateRequest
        {
            Title = $"{prefix}_Alpha",
            Description = "Filter test",
            Amount = 5000,
            Duration = 2,
            Area = 1,
            Type = 1,
            User = 2
        });

        // Act
        var response = await authClient.GetAsync($"/api/project?title={prefix}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<ProjectShortResponse>>();
        results.Should().NotBeNull();
        results!.Should().Contain(p => p.Title.Contains(prefix));
    }

    [Fact]
    public async Task UpdateProjectInformation_WhenValid_UpdatesFieldsSuccessfully()
    {
        // Arrange: Create proposal
        var (authClient, _) = await CreateAuthenticatedClientAsync("alucero@unaj.com", "Password123!");
        var createResponse = await authClient.PostAsJsonAsync("/api/project", new ProjectProposalCreateRequest
        {
            Title = $"BeforeUpdate_{Guid.NewGuid():N}",
            Description = "Original description",
            Amount = 5000,
            Duration = 3,
            Area = 1,
            Type = 1,
            User = 2
        });
        var created = (await createResponse.Content.ReadFromJsonAsync<ProjectResponse>())!;

        var updateRequest = new ProjectUpdateRequest
        {
            Title = $"AfterUpdate_{Guid.NewGuid():N}",
            Description = "Updated description text",
            Duration = 5
        };

        // Act
        var patchResponse = await authClient.PatchAsJsonAsync($"/api/project/{created.Id}", updateRequest);

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<ProjectResponse>();
        updated.Should().NotBeNull();
        updated!.Title.Should().Be(updateRequest.Title);
        updated.Description.Should().Be("Updated description text");
        updated.Duration.Should().Be(5);
    }

    [Fact]
    public async Task DecisionFlow_WhenAuthorizedRoleApproves_UpdatesStepAndProjectStatus()
    {
        // Step 1 for Area 1, Type 1 with Amount = 15000:
        // Rule 1 matches Min 0, Max 100000, Step 1, Role 1 (Líder de Área).
        var (creatorClient, _) = await CreateAuthenticatedClientAsync("jferreyra@unaj.com", "Password123!");
        var createResponse = await creatorClient.PostAsJsonAsync("/api/project", new ProjectProposalCreateRequest
        {
            Title = $"DecisionFlow_{Guid.NewGuid():N}",
            Description = "Testing step approval",
            Amount = 15000,
            Duration = 6,
            Area = 1,
            Type = 1,
            User = 1
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = (await createResponse.Content.ReadFromJsonAsync<ProjectResponse>())!;

        var firstStep = created.Steps!.OrderBy(s => s.StepOrder).First();

        // Log in as user with the required role for step 1
        // Role 1 is Líder de Área (alucero@unaj.com, Id = 2)
        var (approverClient, _) = await CreateAuthenticatedClientAsync("alucero@unaj.com", "Password123!");

        var decisionRequest = new DecisionStepUpdateRequest
        {
            Id = firstStep.Id,
            User = 2,
            Status = 2, // 2 = Approved
            Observation = "Aprobado por el Líder de Área en integración"
        };

        // Act
        var decisionResponse = await approverClient.PatchAsJsonAsync($"/api/project/{created.Id}/decision", decisionRequest);

        // Assert
        decisionResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedProject = await decisionResponse.Content.ReadFromJsonAsync<ProjectResponse>();
        updatedProject.Should().NotBeNull();

        var decidedStep = updatedProject!.Steps!.First(s => s.Id == firstStep.Id);
        decidedStep.Status.Id.Should().Be(2, "Step status must be updated to Approved");
        decidedStep.Observations.Should().Be("Aprobado por el Líder de Área en integración");
    }

    [Fact]
    public async Task DecisionFlow_WhenIncompatibleRoleDecides_ReturnsBadRequest()
    {
        // Arrange: Create proposal
        var (creatorClient, _) = await CreateAuthenticatedClientAsync("jferreyra@unaj.com", "Password123!");
        var createResponse = await creatorClient.PostAsJsonAsync("/api/project", new ProjectProposalCreateRequest
        {
            Title = $"IncompatibleRole_{Guid.NewGuid():N}",
            Description = "Testing wrong role decision",
            Amount = 15000,
            Duration = 6,
            Area = 1,
            Type = 1,
            User = 1
        });
        var created = (await createResponse.Content.ReadFromJsonAsync<ProjectResponse>())!;
        var firstStep = created.Steps!.OrderBy(s => s.StepOrder).First();

        // Step 1 requires Role 1 (Líder de Área). We attempt to decide with User 4 (lolivera@unaj.com, Director = Role 3, Id = 4)
        var (wrongRoleClient, _) = await CreateAuthenticatedClientAsync("lolivera@unaj.com", "Password123!");

        var decisionRequest = new DecisionStepUpdateRequest
        {
            Id = firstStep.Id,
            User = 4,
            Status = 2,
            Observation = "Intento indebido de aprobación"
        };

        // Act
        var response = await wrongRoleClient.PatchAsJsonAsync($"/api/project/{created.Id}/decision", decisionRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.message.Should().NotBeNullOrWhiteSpace();
    }
}
