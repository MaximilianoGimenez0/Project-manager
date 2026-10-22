using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Dtos.Requests;
using Application.Dtos.Responses;
using FluentAssertions;
using ProjectApproval.IntegrationTests.Common;
using Xunit;

namespace ProjectApproval.IntegrationTests.Endpoints;

public class AuthEndpointsTests : BaseIntegrationTest
{
    public AuthEndpointsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndTokens()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "jferreyra@unaj.com",
            Password = "Password123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth.Should().NotBeNull();
        auth!.AccessToken.Should().NotBeNullOrWhiteSpace();
        auth.RefreshToken.Should().NotBeNullOrWhiteSpace();
        auth.User.Should().NotBeNull();
        auth.User.Email.Should().Be("jferreyra@unaj.com");
        auth.User.Name.Should().Be("José Ferreyra");
        auth.User.Role.Should().Be(2); // Gerente
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "jferreyra@unaj.com",
            Password = "WrongPassword999!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nobody_exists@unaj.com",
            Password = "Password123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithEmptyFields_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "",
            Password = ""
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsRotatedTokens()
    {
        // Arrange: first login to obtain a valid refresh token
        var (client, initialAuth) = await CreateAuthenticatedClientAsync("jferreyra@unaj.com", "Password123!");
        var refreshRequest = new RefreshTokenRequest
        {
            RefreshToken = initialAuth.RefreshToken
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var newAuth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        newAuth.Should().NotBeNull();
        newAuth!.AccessToken.Should().NotBeNullOrWhiteSpace();
        newAuth.RefreshToken.Should().NotBeNullOrWhiteSpace();
        newAuth.RefreshToken.Should().NotBe(initialAuth.RefreshToken, "Refresh tokens must rotate on use");
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "invalid-non-existent-token-string"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh-token", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_WhenAuthenticated_ReturnsCurrentUserData()
    {
        // Arrange
        var (authClient, _) = await CreateAuthenticatedClientAsync("jferreyra@unaj.com", "Password123!");

        // Act
        var response = await authClient.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(1);
        user.Email.Should().Be("jferreyra@unaj.com");
        user.Name.Should().Be("José Ferreyra");
        user.Role.Should().Be(2);
    }

    [Fact]
    public async Task GetCurrentUser_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Act: without Authorization header
        var response = await Client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RevokeToken_WhenAuthenticated_RevokesSuccessfully()
    {
        // Arrange
        var (authClient, auth) = await CreateAuthenticatedClientAsync("jferreyra@unaj.com", "Password123!");
        var revokeRequest = new RevokeTokenRequest
        {
            RefreshToken = auth.RefreshToken
        };

        // Act
        var response = await authClient.PostAsJsonAsync("/api/auth/revoke-token", revokeRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Attempting to refresh using the revoked token must now fail
        var refreshResponse = await Client.PostAsJsonAsync("/api/auth/refresh-token", new RefreshTokenRequest
        {
            RefreshToken = auth.RefreshToken
        });
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
