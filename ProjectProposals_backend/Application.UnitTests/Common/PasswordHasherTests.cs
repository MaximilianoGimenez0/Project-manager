using Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace Application.UnitTests.Common
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _hasher;

        public PasswordHasherTests()
        {
            _hasher = new PasswordHasher();
        }

        [Fact]
        public void Hash_Should_Return_NonEmpty_BCrypt_Hash()
        {
            var hash = _hasher.Hash("SecretPassword123!");

            hash.Should().NotBeNullOrWhiteSpace();
            hash.Should().StartWith("$2");
        }

        [Fact]
        public void Verify_Should_Return_True_When_Password_Matches()
        {
            var password = "SecurePassword@2026";
            var hash = _hasher.Hash(password);

            var result = _hasher.Verify(password, hash);

            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_Should_Return_False_When_Password_Does_Not_Match()
        {
            var hash = _hasher.Hash("CorrectPassword");

            var result = _hasher.Verify("IncorrectPassword", hash);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(null, "somehash")]
        [InlineData("", "somehash")]
        [InlineData("   ", "somehash")]
        [InlineData("password", null)]
        [InlineData("password", "")]
        [InlineData("password", "   ")]
        public void Verify_Should_Return_False_When_Input_Is_Null_Or_Whitespace(string? password, string? hash)
        {
            var result = _hasher.Verify(password!, hash!);

            result.Should().BeFalse();
        }

        [Fact]
        public void Verify_Should_Return_False_When_Hash_Is_Malformed()
        {
            var result = _hasher.Verify("password", "invalid_not_a_bcrypt_hash");

            result.Should().BeFalse();
        }
    }
}

