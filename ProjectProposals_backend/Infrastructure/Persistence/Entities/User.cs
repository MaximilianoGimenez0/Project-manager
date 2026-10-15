using System.Collections.Generic;

namespace Infrastructure.Persistence.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int Role { get; set; }
        public ApproverRole UserRole { get; set; } = null!;

        public IList<ProjectProposal> ProjectProposals { get; set; } = new List<ProjectProposal>();
        public IList<ProjectApprovalStep> ProjectApprovalSteps { get; set; } = new List<ProjectApprovalStep>();
        public IList<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
