using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Application.UnitTests.Services
{
    public class JwtTokenServiceTests
    {
        private const string TestSecret = "SuperSecretKeyForTestingProjectProposalsApi2026!";

        [Fact]
        public void GenerateAccessToken_WhenSecretKeyMissing_ThrowsInvalidOperationException()
        {
            // Arrange: Empty configuration
            var inMemorySettings = new Dictionary<string, string?>();
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            var service = new JwtTokenService(config);

            var user = new User { Id = 1, Name = "Test", Email = "test@unaj.com", Role = 1 };

            // Act
            var act = () => service.GenerateAccessToken(user);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("JwtSettings:SecretKey is not configured.");
        }

        [Fact]
        public void GenerateAccessToken_WithValidConfigAndRole_GeneratesTokenWithCorrectClaims()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "JwtSettings:SecretKey", TestSecret },
                { "JwtSettings:Issuer", "TestIssuer" },
                { "JwtSettings:Audience", "TestAudience" },
                { "JwtSettings:AccessTokenExpirationMinutes", "30" }
            };
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            var service = new JwtTokenService(config);

            var user = new User
            {
                Id = 42,
                Name = "Martín Palermo",
                Email = "mpalermo@unaj.com",
                Role = 20,
                UserRole = new ApproverRole { Id = 20, Name = "Gerente" }
            };

            // Act
            var tokenString = service.GenerateAccessToken(user);

            // Assert
            tokenString.Should().NotBeNullOrWhiteSpace();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            jwtToken.Claims.First(c => c.Type == "sub").Value.Should().Be("42");
            jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value.Should().Be("Gerente");

            var rawPayload = System.Text.Encoding.UTF8.GetString(Microsoft.IdentityModel.Tokens.Base64UrlEncoder.DecodeBytes(tokenString.Split('.')[1]));
            rawPayload.Should().Contain("\"iss\":\"TestIssuer\"");
            rawPayload.Should().Contain("\"aud\":\"TestAudience\"");
            rawPayload.Should().Contain("\"roleId\":\"20\"");
            rawPayload.Should().Contain("mpalermo@unaj.com");
            rawPayload.Should().Contain("\"exp\":");
        }

        [Fact]
        public void GenerateAccessToken_WhenUserRoleEntityIsNull_UsesNumericRoleString()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "JwtSettings:SecretKey", TestSecret }
            };
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            var service = new JwtTokenService(config);

            var user = new User
            {
                Id = 10,
                Name = "Usuario Sin Entidad Rol",
                Email = "user@unaj.com",
                Role = 15,
                UserRole = null // Sin entidad asociada
            };

            // Act
            var tokenString = service.GenerateAccessToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value.Should().Be("15");
        }

        [Fact]
        public void GenerateRefreshToken_GeneratesSecureUrlSafeTokenWithConfiguredExpiration()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "JwtSettings:RefreshTokenExpirationDays", "14" }
            };
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            var service = new JwtTokenService(config);

            // Act
            var refreshToken = service.GenerateRefreshToken("192.168.1.100");

            // Assert
            refreshToken.Should().NotBeNull();
            refreshToken.Token.Should().NotBeNullOrWhiteSpace();
            refreshToken.Token.Should().NotContain("+");
            refreshToken.Token.Should().NotContain("/");
            refreshToken.Token.Should().NotEndWith("=");
            refreshToken.CreatedByIp.Should().Be("192.168.1.100");
            refreshToken.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(14), TimeSpan.FromSeconds(5));
            refreshToken.IsActive.Should().BeTrue();
            refreshToken.IsRevoked.Should().BeFalse();
            refreshToken.IsExpired.Should().BeFalse();
        }
    }
}
