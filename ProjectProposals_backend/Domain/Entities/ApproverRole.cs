using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ApproverRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HierarchyLevel { get; set; }
        public bool IsSpecializedRole { get; set; }
    }
}
