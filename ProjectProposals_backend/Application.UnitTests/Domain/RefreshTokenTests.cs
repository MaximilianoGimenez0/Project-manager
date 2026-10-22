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
            var token = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null
            };

            token.IsExpired.Should().BeFalse();
            token.IsRevoked.Should().BeFalse();
            token.IsActive.Should().BeTrue();
        }

        [Fact]
        public void ExpiredToken_ShouldReturnIsExpiredTrueAndIsActiveFalse()
        {
            var token = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddMinutes(-10),
                RevokedAt = null
            };

            token.IsExpired.Should().BeTrue();
            token.IsRevoked.Should().BeFalse();
            token.IsActive.Should().BeFalse();
        }

        [Fact]
        public void RevokedToken_ShouldReturnIsRevokedTrueAndIsActiveFalse()
        {
            var token = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddDays(5),
                RevokedAt = DateTime.UtcNow.AddHours(-1),
                ReasonRevoked = "Cierre de sesión",
                RevokedByIp = "127.0.0.1"
            };

            token.IsExpired.Should().BeFalse();
            token.IsRevoked.Should().BeTrue();
            token.IsActive.Should().BeFalse();
        }

        [Fact]
        public void ExpiredAndRevokedToken_ShouldReturnIsActiveFalse()
        {
            var token = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                RevokedAt = DateTime.UtcNow.AddDays(-2)
            };

            token.IsExpired.Should().BeTrue();
            token.IsRevoked.Should().BeTrue();
            token.IsActive.Should().BeFalse();
        }
    }
}
