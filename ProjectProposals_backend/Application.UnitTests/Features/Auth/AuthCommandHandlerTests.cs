using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Dtos.Requests;
using Application.Exceptions;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Features.Auth.Queries.GetCurrentUser;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

using Microsoft.Extensions.Logging.Abstractions;

namespace Application.UnitTests.Features.Auth
{
    public class AuthCommandHandlerTests
    {
        private readonly IMapper _mapper;

        public AuthCommandHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommonMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Login_Should_Throw_When_User_Not_Found()
        {
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmail(It.IsAny<string>())).ReturnsAsync((User?)null);

            var handler = new LoginCommandHandler(
                userRepoMock.Object,
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IJwtTokenService>(),
                _mapper);

            var act = () => handler.Handle(new LoginCommand(new LoginRequest
            {
                Email = "test@unaj.com",
                Password = "Password123!"
            }), CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("Credenciales incorrectas.");
        }

        [Fact]
        public async Task Login_Should_Throw_When_Password_Invalid()
        {
            var user = new User { Id = 1, Email = "test@unaj.com", PasswordHash = "hashed" };
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmail(user.Email)).ReturnsAsync(user);

            var hasherMock = new Mock<IPasswordHasher>();
            hasherMock.Setup(h => h.Verify("WrongPassword", "hashed")).Returns(false);

            var handler = new LoginCommandHandler(
                userRepoMock.Object,
                Mock.Of<IRefreshTokenRepository>(),
                hasherMock.Object,
                Mock.Of<IJwtTokenService>(),
                _mapper);

            var act = () => handler.Handle(new LoginCommand(new LoginRequest
            {
                Email = "test@unaj.com",
                Password = "WrongPassword"
            }), CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("Credenciales incorrectas.");
        }

        [Fact]
        public async Task Login_Should_Return_Tokens_When_Credentials_Valid()
        {
            var user = new User { Id = 1, Name = "Test User", Email = "test@unaj.com", Role = 2, PasswordHash = "validhash" };
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmail(user.Email)).ReturnsAsync(user);

            var hasherMock = new Mock<IPasswordHasher>();
            hasherMock.Setup(h => h.Verify("Password123!", "validhash")).Returns(true);

            var tokenServiceMock = new Mock<IJwtTokenService>();
            tokenServiceMock.Setup(t => t.GenerateAccessToken(user)).Returns("access-token-123");
            tokenServiceMock.Setup(t => t.GenerateRefreshToken(It.IsAny<string>()))
                .Returns(new RefreshToken { Token = "refresh-token-123", ExpiresAt = DateTime.UtcNow.AddDays(7) });

            var refreshRepoMock = new Mock<IRefreshTokenRepository>();

            var handler = new LoginCommandHandler(
                userRepoMock.Object,
                refreshRepoMock.Object,
                hasherMock.Object,
                tokenServiceMock.Object,
                _mapper);

            var response = await handler.Handle(new LoginCommand(new LoginRequest
            {
                Email = user.Email,
                Password = "Password123!"
            }), CancellationToken.None);

            response.Should().NotBeNull();
            response.AccessToken.Should().Be("access-token-123");
            response.RefreshToken.Should().Be("refresh-token-123");
            response.User.Email.Should().Be("test@unaj.com");
            refreshRepoMock.Verify(r => r.Add(It.IsAny<RefreshToken>()), Times.Once);
        }

        [Fact]
        public async Task RefreshToken_Should_Detect_Reuse_And_Revoke_All_Tokens()
        {
            var revokedToken = new RefreshToken
            {
                UserId = 1,
                Token = "compromised-token",
                RevokedAt = DateTime.UtcNow.AddMinutes(-5),
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };

            var refreshRepoMock = new Mock<IRefreshTokenRepository>();
            refreshRepoMock.Setup(r => r.GetByToken("compromised-token")).ReturnsAsync(revokedToken);

            var handler = new RefreshTokenCommandHandler(
                refreshRepoMock.Object,
                Mock.Of<IUserRepository>(),
                Mock.Of<IJwtTokenService>(),
                _mapper);

            var act = () => handler.Handle(new RefreshTokenCommand(new RefreshTokenRequest
            {
                RefreshToken = "compromised-token"
            }), CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("*revocado*");

            refreshRepoMock.Verify(r => r.RevokeUserTokens(1, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RefreshToken_Should_Rotate_Token_Successfully()
        {
            var user = new User { Id = 1, Name = "Test User", Email = "test@unaj.com", Role = 1 };
            var activeToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                User = user,
                Token = "valid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };

            var refreshRepoMock = new Mock<IRefreshTokenRepository>();
            refreshRepoMock.Setup(r => r.GetByToken("valid-refresh-token")).ReturnsAsync(activeToken);

            var tokenServiceMock = new Mock<IJwtTokenService>();
            tokenServiceMock.Setup(t => t.GenerateRefreshToken(It.IsAny<string>()))
                .Returns(new RefreshToken { Token = "new-refresh-token", ExpiresAt = DateTime.UtcNow.AddDays(7) });
            tokenServiceMock.Setup(t => t.GenerateAccessToken(user)).Returns("new-access-token");

            var handler = new RefreshTokenCommandHandler(
                refreshRepoMock.Object,
                Mock.Of<IUserRepository>(),
                tokenServiceMock.Object,
                _mapper);

            var response = await handler.Handle(new RefreshTokenCommand(new RefreshTokenRequest
            {
                RefreshToken = "valid-refresh-token"
            }), CancellationToken.None);

            response.Should().NotBeNull();
            response.AccessToken.Should().Be("new-access-token");
            response.RefreshToken.Should().Be("new-refresh-token");
            activeToken.IsRevoked.Should().BeTrue();
            activeToken.ReplacedByToken.Should().Be("new-refresh-token");
            refreshRepoMock.Verify(r => r.Update(activeToken), Times.Once);
            refreshRepoMock.Verify(r => r.Add(It.IsAny<RefreshToken>()), Times.Once);
        }

        [Fact]
        public async Task RevokeToken_Should_Revoke_Active_Token()
        {
            var activeToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                Token = "token-to-revoke",
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };

            var refreshRepoMock = new Mock<IRefreshTokenRepository>();
            refreshRepoMock.Setup(r => r.GetByToken("token-to-revoke")).ReturnsAsync(activeToken);
            refreshRepoMock.Setup(r => r.Update(activeToken)).ReturnsAsync(true);

            var handler = new RevokeTokenCommandHandler(refreshRepoMock.Object);

            var result = await handler.Handle(new RevokeTokenCommand(new RevokeTokenRequest
            {
                RefreshToken = "token-to-revoke"
            }), CancellationToken.None);

            result.Should().BeTrue();
            activeToken.IsRevoked.Should().BeTrue();
            refreshRepoMock.Verify(r => r.Update(activeToken), Times.Once);
        }

        [Fact]
        public async Task GetCurrentUser_Should_Return_UserResponse()
        {
            var user = new User { Id = 1, Name = "Test User", Email = "test@unaj.com", Role = 1 };
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(user);

            var handler = new GetCurrentUserQueryHandler(userRepoMock.Object, _mapper);

            var result = await handler.Handle(new GetCurrentUserQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Test User");
            result.Email.Should().Be("test@unaj.com");
        }
    }
}
