using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Dtos.Requests;
using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectApproval.Api.Controllers
{
    /// <summary>
    /// Controlador responsable de la autenticación de usuarios, emisión y rotación de tokens JWT y refresh tokens,
    /// y administración de la sesión activa del usuario.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="AuthController"/>.
        /// </summary>
        /// <param name="sender">Mediador para la ejecución de comandos y consultas (CQRS).</param>
        /// <param name="currentUserService">Servicio para acceder a la identidad y claims del usuario autenticado.</param>
        public AuthController(ISender sender, ICurrentUserService currentUserService)
        {
            _sender = sender;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Inicia sesión de un usuario validando credenciales y emite tokens de acceso y actualización.
        /// </summary>
        /// <remarks>
        /// Recibe las credenciales (correo electrónico y contraseña). Si son válidas, genera:
        /// - Un **Access Token (JWT)** de corta duración (15 minutos) con claims de identidad y rol.
        /// - Un **Refresh Token** criptográfico de larga duración (7 días) almacenado en base de datos para rotación tradicional.
        ///
        /// Usuarios de prueba disponibles (clave: <c>Password123!</c>):
        /// - <c>jferreyra@unaj.com</c> (Gerente)
        /// - <c>alucero@unaj.com</c> (Líder de Área)
        /// - <c>gmolinas@unaj.com</c> (Gerente)
        /// - <c>lolivera@unaj.com</c> (Director)
        /// - <c>dfagundez@unaj.com</c> (Comité Técnico)
        /// - <c>ggalli@unaj.com</c> (Comité Técnico)
        /// - <c>mtorres@unaj.com</c> (Líder de Área)
        /// - <c>cmendoza@unaj.com</c> (Director)
        /// </remarks>
        /// <param name="request">Cuerpo de la petición con correo electrónico y contraseña.</param>
        /// <returns>Objeto <see cref="AuthResponse"/> con el token de acceso JWT, refresh token y datos del usuario.</returns>
        /// <response code="200">Inicio de sesión exitoso. Retorna los tokens generados y datos del perfil.</response>
        /// <response code="400">Petición inválida (campos requeridos ausentes o formato de email incorrecto).</response>
        /// <response code="401">Credenciales inválidas (usuario inexistente o contraseña incorrecta).</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            ValidateModelState();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _sender.Send(new LoginCommand(request, ipAddress));
            return Ok(result);
        }

        /// <summary>
        /// Renueva el token de acceso JWT y rota el token de actualización (Método tradicional con payload JSON).
        /// </summary>
        /// <remarks>
        /// Este endpoint implementa las mejores prácticas de seguridad de tokens:
        /// - **Rotación de Refresh Tokens (Token Rotation)**: El token recibido es invalidado y reemplazado inmediatamente por uno nuevo.
        /// - **Detección de Reúso (Token Reuse Detection)**: Si se intenta utilizar un refresh token previamente revocado,
        ///   el sistema asume un compromiso de seguridad y revoca inmediatamente todas las sesiones activas del usuario.
        /// </remarks>
        /// <param name="request">Cuerpo con el refresh token actual a validar y renovar.</param>
        /// <returns>Objeto <see cref="AuthResponse"/> con el nuevo par de Access Token y Refresh Token.</returns>
        /// <response code="200">Tokens renovados exitosamente.</response>
        /// <response code="400">Token no suministrado o formato inválido.</response>
        /// <response code="401">Token de actualización inválido, expirado o comprometido.</response>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            ValidateModelState();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _sender.Send(new RefreshTokenCommand(request, ipAddress));
            return Ok(result);
        }

        /// <summary>
        /// Cierra la sesión activa revocando de forma explícita el token de actualización en la base de datos.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación previa mediante encabezado <c>Authorization: Bearer &lt;token&gt;</c>.
        /// Marca el Refresh Token enviado como revocado en la base de datos, impidiendo que pueda ser usado nuevamente.
        /// </remarks>
        /// <param name="request">Cuerpo con el refresh token que se desea revocar.</param>
        /// <returns>Mensaje confirmando la revocación exitosa.</returns>
        /// <response code="200">Sesión cerrada y token revocado exitosamente.</response>
        /// <response code="400">Cuerpo de la petición inválido.</response>
        /// <response code="401">No autorizado (token JWT faltante, inválido o expirado).</response>
        /// <response code="404">El token de actualización especificado no fue encontrado.</response>
        [HttpPost("revoke-token")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            ValidateModelState();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _sender.Send(new RevokeTokenCommand(request, ipAddress));
            return Ok(new { message = "Sesión cerrada y token revocado exitosamente." });
        }

        /// <summary>
        /// Obtiene el perfil e información del usuario actualmente autenticado a partir de las claims del token JWT.
        /// </summary>
        /// <remarks>
        /// Requiere encabezado <c>Authorization: Bearer &lt;token&gt;</c>.
        /// Extrae el identificador del usuario de las claims <c>sub</c> / <c>NameIdentifier</c> y consulta sus datos vigentes
        /// en la base de datos. Es ideal para rehidratar el estado del usuario en el frontend tras recargar la página.
        /// </remarks>
        /// <returns>Objeto <see cref="UserResponse"/> con los datos actualizados del usuario y su rol.</returns>
        /// <response code="200">Perfil del usuario obtenido con éxito.</response>
        /// <response code="401">No autorizado (token JWT ausente o sin identificación válida).</response>
        /// <response code="404">El usuario referenciado por el token no existe en el sistema.</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new UnauthorizedException("Usuario no autenticado o token sin identificación válida.");
            }

            var result = await _sender.Send(new GetCurrentUserQuery(userId.Value));
            return Ok(result);
        }

        private void ValidateModelState()
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Error de validación.";

                throw new BadRequestException(firstError);
            }
        }
    }
}
