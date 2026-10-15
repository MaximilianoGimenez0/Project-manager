using System.Collections.Generic;

namespace Infrastructure.Persistence.Entities
{
    public class ApproverRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HierarchyLevel { get; set; }
        public bool IsSpecializedRole { get; set; }
        public IList<User> Users { get; set; } = new List<User>();
        public IList<ApprovalRule> ApprovalRules { get; set; } = new List<ApprovalRule>();
        public IList<ProjectApprovalStep> ProjectApprovalSteps { get; set; } = new List<ProjectApprovalStep>();
    }
}
