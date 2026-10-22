using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Application.UnitTests.Domain
{
    public class ApprovalRuleTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Act
            var rule = new ApprovalRule(10000m, 50000m, 2, 20)
            {
                Id = 1,
                Area = 3,
                Type = 4
            };

            // Assert
            rule.MinAmount.Should().Be(10000m);
            rule.MaxAmount.Should().Be(50000m);
            rule.StepOrder.Should().Be(2);
            rule.ApproverRoleId.Should().Be(20);
            rule.Area.Should().Be(3);
            rule.Type.Should().Be(4);
        }
    }
}
