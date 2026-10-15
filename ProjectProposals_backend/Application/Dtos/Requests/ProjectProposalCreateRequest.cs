using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Requests
{
    public class ProjectProposalCreateRequest
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(255, ErrorMessage = "El título no puede tener más de 255 caracteres.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(255, ErrorMessage = "La descripción no puede tener más de 255 caracteres.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El valor estimado es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto estimado debe ser un valor positivo.")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "La duración estimada es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La duración estimada debe ser un número positivo.")]
        public int? Duration { get; set; }

        [Required(ErrorMessage = "El área es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "El área debe ser un número positivo.")]
        public int? Area { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El tipo debe ser un número positivo.")]
        public int? Type { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El usuario debe ser un número positivo.")]
        public int? User { get; set; }
    }
}
