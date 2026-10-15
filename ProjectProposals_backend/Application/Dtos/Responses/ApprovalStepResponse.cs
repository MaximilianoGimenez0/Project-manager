using System;

namespace Application.Dtos.Responses
{
    public class ApprovalStepResponse
    {
        public long Id { get; set; }
        public int StepOrder { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string? Observations { get; set; }
        public UserResponse? ApproverUser { get; set; }
        public GenericResponse ApproverRole { get; set; } = new();
        public GenericResponse Status { get; set; } = new();
    }
}
