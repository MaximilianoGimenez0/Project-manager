using System;
using System.Collections.Generic;

namespace Application.Dtos.Responses
{
    public class ProjectResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Duration { get; set; }
        public UserResponse User { get; set; } = new();
        public GenericResponse Area { get; set; } = new();
        public GenericResponse Status { get; set; } = new();
        public GenericResponse Type { get; set; } = new();
        public List<ApprovalStepResponse>? Steps { get; set; }
    }
}
