using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Requests
{
    public class ProjectUpdateRequest
    {
        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(255, ErrorMessage = "El título no puede tener más de 255 caracteres.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(255, ErrorMessage = "La descripción no puede tener más de 255 caracteres.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "La duración estimada debe ser un número positivo.")]
        public int? Duration { get; set; }
    }
}
