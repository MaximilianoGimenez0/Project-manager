using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Common
{
    public class ApprovalStepEngineTests
    {
        private readonly Mock<IApprovalRuleRepository> _ruleRepositoryMock;
        private readonly Mock<IApproverRoleRepository> _roleRepositoryMock;
        private readonly ApprovalStepEngine _sut;

        public ApprovalStepEngineTests()
        {
            _ruleRepositoryMock = new Mock<IApprovalRuleRepository>();
            _roleRepositoryMock = new Mock<IApproverRoleRepository>();

            var defaultRoles = new List<ApproverRole>
            {
                new ApproverRole { Id = (int)ApproverRoleEnum.AreaLeader, Name = "Líder de Área", HierarchyLevel = 10, IsSpecializedRole = false },
                new ApproverRole { Id = (int)ApproverRoleEnum.TechnicalCommittee, Name = "Comité Técnico", HierarchyLevel = 15, IsSpecializedRole = true },
                new ApproverRole { Id = (int)ApproverRoleEnum.Manager, Name = "Gerente", HierarchyLevel = 20, IsSpecializedRole = false },
                new ApproverRole { Id = (int)ApproverRoleEnum.Director, Name = "Director", HierarchyLevel = 30, IsSpecializedRole = false }
            };
            _roleRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(defaultRoles);

            _sut = new ApprovalStepEngine(_ruleRepositoryMock.Object, _roleRepositoryMock.Object);
        }

        [Fact]
        public async Task CalculateStepsAsync_WhenNoRulesExist_ReturnsEmptyList()
        {
            _ruleRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ApprovalRule>());

            var project = new ProjectProposal
            {
                EstimatedAmount = 50000,
                Area = 1,
                Type = 1
            };

            var result = await _sut.CalculateStepsAsync(project);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CalculateStepsAsync_WhenAmountDoesNotMatchRange_ReturnsEmptyList()
        {
            var rules = new List<ApprovalRule>
            {
                new ApprovalRule(10000, 20000, 1, 1) { Id = 1 }
            };
            _ruleRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(rules);

            var project = new ProjectProposal
            {
                EstimatedAmount = 5000,
                Area = 1,
                Type = 1
            };

            var result = await _sut.CalculateStepsAsync(project);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CalculateStepsAsync_WhenMatchingRulesExist_GeneratesStepsInAscendingOrderWithPendingStatus()
        {
            var rules = new List<ApprovalRule>
            {
                new ApprovalRule(0, 100000, 1, (int)ApproverRoleEnum.AreaLeader) { Id = 1 },
                new ApprovalRule(10000, 50000, 2, (int)ApproverRoleEnum.Manager) { Id = 2 },
                new ApprovalRule(30000, 0, 3, (int)ApproverRoleEnum.Director) { Id = 3 } // 0 max means unlimited
            };
            _ruleRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(rules);

            var project = new ProjectProposal
            {
                EstimatedAmount = 35000,
                Area = 1,
                Type = 1
            };

            var result = await _sut.CalculateStepsAsync(project);

            result.Should().HaveCount(3);
            result[0].StepOrder.Should().Be(1);
            result[0].ApproverRoleId.Should().Be((int)ApproverRoleEnum.AreaLeader);
            result[0].Status.Should().Be((int)ApprovalStatusEnum.Pending);

            result[1].StepOrder.Should().Be(2);
            result[1].ApproverRoleId.Should().Be((int)ApproverRoleEnum.Manager);
            result[1].Status.Should().Be((int)ApprovalStatusEnum.Pending);

            result[2].StepOrder.Should().Be(3);
            result[2].ApproverRoleId.Should().Be((int)ApproverRoleEnum.Director);
            result[2].Status.Should().Be((int)ApprovalStatusEnum.Pending);
        }

        [Fact]
        public async Task CalculateStepsAsync_WhenMultipleRulesForSameStep_PrioritizesMostSpecificRuleByAreaAndType()
        {
            var genericRule = new ApprovalRule(0, 50000, 1, (int)ApproverRoleEnum.AreaLeader)
            {
                Id = 1,
                Area = null,
                Type = null
            };
            var specificRule = new ApprovalRule(0, 50000, 1, (int)ApproverRoleEnum.TechnicalCommittee)
            {
                Id = 2,
                Area = 2,
                Type = 2
            };

            _ruleRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ApprovalRule> { genericRule, specificRule });

            var project = new ProjectProposal
            {
                EstimatedAmount = 15000,
                Area = 2,
                Type = 2
            };

            var result = await _sut.CalculateStepsAsync(project);

            result.Should().HaveCount(1);
            result[0].StepOrder.Should().Be(1);
            result[0].ApproverRoleId.Should().Be((int)ApproverRoleEnum.TechnicalCommittee);
        }

        [Fact]
        public async Task CalculateStepsAsync_WhenMultipleMatchingRulesSelectSameRole_DeduplicatesRoleToSingleStep()
        {
            var rule1 = new ApprovalRule(0, 20000, 1, (int)ApproverRoleEnum.Manager) { Id = 3, Area = 2, Type = 2 };
            var rule2 = new ApprovalRule(10000, 30000, 2, (int)ApproverRoleEnum.Manager) { Id = 8, Area = 2 };

            _ruleRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ApprovalRule> { rule1, rule2 });

            var project = new ProjectProposal
            {
                EstimatedAmount = 15000,
                Area = 2,
                Type = 2
            };

            var result = await _sut.CalculateStepsAsync(project);

            result.Should().HaveCount(1);
            result[0].StepOrder.Should().Be(1);
            result[0].ApproverRoleId.Should().Be((int)ApproverRoleEnum.Manager);
        }

        [Fact]
        public async Task CalculateStepsAsync_WhenStepGapsExist_GeneratesConsecutiveStepOrdersWithoutGaps()
        {
            var rule1 = new ApprovalRule(0, 100000, 1, (int)ApproverRoleEnum.AreaLeader) { Id = 1 };
            var rule3 = new ApprovalRule(20000, 0, 3, (int)ApproverRoleEnum.Director) { Id = 4 };

            _ruleRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ApprovalRule> { rule1, rule3 });

            var project = new ProjectProposal
            {
                EstimatedAmount = 25000,
                Area = 4,
                Type = 3
            };

            var result = await _sut.CalculateStepsAsync(project);

            result.Should().HaveCount(2);
            result[0].StepOrder.Should().Be(1);
            result[0].ApproverRoleId.Should().Be((int)ApproverRoleEnum.AreaLeader);

            result[1].StepOrder.Should().Be(2);
            result[1].ApproverRoleId.Should().Be((int)ApproverRoleEnum.Director);
        }

        [Fact]
        public async Task CalculateStepsAsync_WhenRulesDefineInvertedOrder_SortsByRoleHierarchy()
        {
            var invertedRule1 = new ApprovalRule(0, 100000, 1, (int)ApproverRoleEnum.Director) { Id = 1 };
            var invertedRule2 = new ApprovalRule(0, 100000, 2, (int)ApproverRoleEnum.AreaLeader) { Id = 2 };

            _ruleRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ApprovalRule> { invertedRule1, invertedRule2 });

            var project = new ProjectProposal
            {
                EstimatedAmount = 15000,
                Area = 1,
                Type = 1
            };

            var result = await _sut.CalculateStepsAsync(project);

            result.Should().HaveCount(2);
            result[0].StepOrder.Should().Be(1);
            result[0].ApproverRoleId.Should().Be((int)ApproverRoleEnum.AreaLeader);

            result[1].StepOrder.Should().Be(2);
            result[1].ApproverRoleId.Should().Be((int)ApproverRoleEnum.Director);
        }

        [Fact]
        public async Task CalculateStepsAsync_WhenRoleRepositoryIsNull_UsesDefaultHierarchies()
        {
            var engineWithoutRoles = new ApprovalStepEngine(_ruleRepositoryMock.Object);

            var rule1 = new ApprovalRule(0, 100000, 1, (int)ApproverRoleEnum.Director) { Id = 1 };
            var rule2 = new ApprovalRule(0, 100000, 2, (int)ApproverRoleEnum.TechnicalCommittee) { Id = 2 };
            var rule3 = new ApprovalRule(0, 100000, 3, 99) { Id = 3 }; // Unknown role, falls back to 99 * 10

            _ruleRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ApprovalRule> { rule1, rule2, rule3 });

            var project = new ProjectProposal { EstimatedAmount = 50000 };

            var result = await engineWithoutRoles.CalculateStepsAsync(project);

            result.Should().HaveCount(3);
            result[0].StepOrder.Should().Be(1);
            result[0].ApproverRoleId.Should().Be((int)ApproverRoleEnum.TechnicalCommittee);

            result[1].StepOrder.Should().Be(2);
            result[1].ApproverRoleId.Should().Be((int)ApproverRoleEnum.Director);

            result[2].StepOrder.Should().Be(3);
            result[2].ApproverRoleId.Should().Be(99);
        }
    }
}
