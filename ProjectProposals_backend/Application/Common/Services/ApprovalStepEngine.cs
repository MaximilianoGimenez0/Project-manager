using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Common.Services
{
    public class ApprovalStepEngine : IApprovalStepEngine
    {
        private readonly IApprovalRuleRepository _ruleRepository;
        private readonly IApproverRoleRepository? _roleRepository;

        public ApprovalStepEngine(IApprovalRuleRepository ruleRepository, IApproverRoleRepository roleRepository)
        {
            _ruleRepository = ruleRepository;
            _roleRepository = roleRepository;
        }

        public ApprovalStepEngine(IApprovalRuleRepository ruleRepository) : this(ruleRepository, null!)
        {
        }

        public async Task<List<ProjectApprovalStep>> CalculateStepsAsync(ProjectProposal project)
        {
            var rules = await _ruleRepository.GetAll();

            var applicableRules = rules
                .Where(rule =>
                    rule.MinAmount <= project.EstimatedAmount &&
                    (rule.MaxAmount == 0 || rule.MaxAmount >= project.EstimatedAmount) &&
                    (rule.Area == null || rule.Area == project.Area) &&
                    (rule.Type == null || rule.Type == project.Type)
                )
                .ToList();

            if (!applicableRules.Any())
            {
                return new List<ProjectApprovalStep>();
            }

            var groupedByStep = applicableRules
                .GroupBy(rule => rule.StepOrder)
                .OrderBy(g => g.Key)
                .ToList();

            var selectedRoleIds = new List<int>();

            foreach (var group in groupedByStep)
            {
                var selected = group
                    .OrderByDescending(r =>
                        (r.Area != null ? 1 : 0) + (r.Type != null ? 1 : 0)
                    )
                    .ThenBy(r => r.Id)
                    .FirstOrDefault();

                if (selected != null)
                {
                    selectedRoleIds.Add(selected.ApproverRoleId);
                }
            }

            var distinctRoleIds = selectedRoleIds.Distinct().ToList();

            var roles = _roleRepository != null ? await _roleRepository.GetAll() : new List<ApproverRole>();
            var roleMap = roles.ToDictionary(r => r.Id);

            var orderedRoles = distinctRoleIds
                .Select(id =>
                {
                    if (roleMap.TryGetValue(id, out var role) && role.HierarchyLevel > 0)
                    {
                        return role;
                    }

                    // Fallback hierarchy levels prevent sorting failures if database roles lack hierarchy data
                    var defaultLevel = id switch
                    {
                        (int)ApproverRoleEnum.AreaLeader => 10,
                        (int)ApproverRoleEnum.TechnicalCommittee => 15,
                        (int)ApproverRoleEnum.Manager => 20,
                        (int)ApproverRoleEnum.Director => 30,
                        _ => id * 10
                    };

                    return new ApproverRole
                    {
                        Id = id,
                        HierarchyLevel = defaultLevel,
                        IsSpecializedRole = id == (int)ApproverRoleEnum.TechnicalCommittee
                    };
                })
                .OrderBy(r => r.HierarchyLevel)
                .ThenBy(r => r.Id)
                .ToList();

            var steps = new List<ProjectApprovalStep>();
            for (int i = 0; i < orderedRoles.Count; i++)
            {
                steps.Add(new ProjectApprovalStep
                {
                    ApproverRoleId = orderedRoles[i].Id,
                    StepOrder = i + 1,
                    Status = (int)ApprovalStatusEnum.Pending
                });
            }

            return steps;
        }
    }
}
