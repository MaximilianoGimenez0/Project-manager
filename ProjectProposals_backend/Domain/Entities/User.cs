using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int Role { get; set; }
        public ApproverRole? UserRole { get; set; }
        public IList<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
