using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Requests
{
    public class DecisionStepUpdateRequest
    {
        [Required(ErrorMessage = "El id es obligatorio")]
        [Range(1, long.MaxValue, ErrorMessage = "El id del paso debe ser un número positivo.")]
        public long Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El usuario debe ser un número positivo.")]
        public int User { get; set; }

        [Required(ErrorMessage = "El status es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El status debe ser un número positivo.")]
        public int Status { get; set; }

        [StringLength(255, ErrorMessage = "La observación no puede tener más de 255 caracteres.")]
        public string? Observation { get; set; }
    }
}
