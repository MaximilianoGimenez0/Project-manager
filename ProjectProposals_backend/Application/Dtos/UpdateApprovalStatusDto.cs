namespace Application.Dtos
{
    public class UpdateApprovalStatusDto
    {
        public int? User { get; set; }
        public long? Step { get; set; }
        public int? NewStatus { get; set; }
        public string? Observation { get; set; }
    }

    [System.Obsolete("Use UpdateApprovalStatusDto instead.")]
    public class updateApprovalStatusDto : UpdateApprovalStatusDto
    {
    }
}
