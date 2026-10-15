using Xunit;

namespace Application.UnitTests.Common
{
    public class PasswordHasherTests
    {
        [Fact]
        public void PasswordHashing_Should_Hash_And_Verify_Successfully()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Password123!");
            Assert.True(BCrypt.Net.BCrypt.Verify("Password123!", hash));
            Assert.False(BCrypt.Net.BCrypt.Verify("WrongPassword", hash));
        }
    }
}
