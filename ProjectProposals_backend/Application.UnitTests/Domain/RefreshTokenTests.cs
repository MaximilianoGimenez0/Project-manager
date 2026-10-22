using System;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Application.UnitTests.Domain
{
    public class RefreshTokenTests
    {
        [Fact]
        public void ActiveToken_ShouldReturnCorrectBooleans()
        {
            // Arrange
            var token = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null
            };

            // Assert
            token.IsExpired.Should().BeFalse();
            token.IsRevoked.Should().BeFalse();
            token.IsActive.Should().BeTrue();
        }

        [Fact]
        public void ExpiredToken_ShouldReturnIsExpiredTrueAndIsActiveFalse()
        {
            // Arrange
            var token = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddMinutes(-10),
                RevokedAt = null
            };

            // Assert
            token.IsExpired.Should().BeTrue();
            token.IsRevoked.Should().BeFalse();
            token.IsActive.Should().BeFalse();
        }

        [Fact]
        public void RevokedToken_ShouldReturnIsRevokedTrueAndIsActiveFalse()
        {
            // Arrange
            var token = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddDays(5),
                RevokedAt = DateTime.UtcNow.AddHours(-1),
                ReasonRevoked = "Cierre de sesión",
                RevokedByIp = "127.0.0.1"
            };

            // Assert
            token.IsExpired.Should().BeFalse();
            token.IsRevoked.Should().BeTrue();
            token.IsActive.Should().BeFalse();
        }

        [Fact]
        public void ExpiredAndRevokedToken_ShouldReturnIsActiveFalse()
        {
            // Arrange
            var token = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                RevokedAt = DateTime.UtcNow.AddDays(-2)
            };

            // Assert
            token.IsExpired.Should().BeTrue();
            token.IsRevoked.Should().BeTrue();
            token.IsActive.Should().BeFalse();
        }
    }
}
