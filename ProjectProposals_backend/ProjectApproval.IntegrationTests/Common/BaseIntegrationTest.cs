using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Common.Interfaces;
using Application.Dtos.Requests;
using Application.Dtos.Responses;
using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ProjectApproval.IntegrationTests.Common;

[Collection("IntegrationTests")]
public abstract class BaseIntegrationTest
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task ExecuteDbContextAsync(Func<ProjectApprovalDbContext, Task> action)
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProjectApprovalDbContext>();
        await action(context);
    }

    protected async Task<T> ExecuteDbContextAsync<T>(Func<ProjectApprovalDbContext, Task<T>> action)
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProjectApprovalDbContext>();
        return await action(context);
    }

    protected async Task<(HttpClient Client, AuthResponse Auth)> CreateAuthenticatedClientAsync(
        string email = "jferreyra@unaj.com",
        string password = "Password123!")
    {
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });

        loginResponse.EnsureSuccessStatusCode();

        var auth = (await loginResponse.Content.ReadFromJsonAsync<AuthResponse>())!;
        var authenticatedClient = Factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        return (authenticatedClient, auth);
    }

    protected string GenerateCustomJwtToken(int userId, string email, int roleId, string name = "Test User")
    {
        using var scope = Factory.Services.CreateScope();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();
        var user = new Domain.Entities.User
        {
            Id = userId,
            Email = email,
            Role = roleId,
            Name = name
        };
        return jwtService.GenerateAccessToken(user);
    }
}
