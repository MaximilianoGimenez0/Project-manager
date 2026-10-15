using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Requests
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "El refresh token es obligatorio.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
