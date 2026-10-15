using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dtos.Responses;
using Application.Features.Catalogs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectApproval.Api.Controllers
{
    /// <summary>
    /// Controlador para la consulta de catálogos e información auxiliar del sistema (Áreas, Tipos de Proyecto, Roles, Estados y Usuarios).
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/")]
    public class InformationController : ControllerBase
    {
        private readonly ISender _sender;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="InformationController"/>.
        /// </summary>
        /// <param name="sender">Mediador para la ejecución de consultas.</param>
        public InformationController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Obtiene el catálogo completo de áreas de la organización.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Devuelve la lista de áreas disponibles para asignar a las propuestas de proyectos (Finanzas, Tecnología, Recursos Humanos, Operaciones).
        /// </remarks>
        /// <returns>Lista de áreas con su identificador y nombre.</returns>
        /// <response code="200">Listado de áreas obtenido exitosamente.</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        /// <response code="404">No se encontraron registros de áreas.</response>
        [HttpGet("Area")]
        [ProducesResponseType(typeof(List<GenericResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> AreaGetAll()
        {
            var results = await _sender.Send(new GetAreasQuery());
            return Ok(results);
        }

        /// <summary>
        /// Obtiene el catálogo completo de tipos de proyectos.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Devuelve los tipos de proyectos admitidos (Mejora de Procesos, Innovación y Desarrollo, Infraestructura, Capacitación Interna).
        /// </remarks>
        /// <returns>Lista de tipos de proyectos con su identificador y nombre.</returns>
        /// <response code="200">Listado de tipos de proyectos obtenido exitosamente.</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        /// <response code="404">No se encontraron registros de tipos de proyectos.</response>
        [HttpGet("ProjectType")]
        [ProducesResponseType(typeof(List<GenericResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> TypeGetAll()
        {
            var results = await _sender.Send(new GetProjectTypesQuery());
            return Ok(results);
        }

        /// <summary>
        /// Obtiene el catálogo completo de roles aprobadores del flujo de aprobación.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Devuelve los roles aprobadores del sistema (Líder de Área, Gerente, Director, Comité Técnico).
        /// </remarks>
        /// <returns>Lista de roles con su identificador y nombre.</returns>
        /// <response code="200">Listado de roles obtenido exitosamente.</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        /// <response code="404">No se encontraron registros de roles.</response>
        [HttpGet("Role")]
        [ProducesResponseType(typeof(List<GenericResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> RoleGetAll()
        {
            var results = await _sender.Send(new GetApproverRolesQuery());
            return Ok(results);
        }

        /// <summary>
        /// Obtiene el catálogo completo de estados de aprobación de proyectos.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Devuelve los estados de aprobación posibles (Pending = 1, Approved = 2, Rejected = 3, Observed = 4).
        /// </remarks>
        /// <returns>Lista de estados con su identificador y nombre.</returns>
        /// <response code="200">Listado de estados de aprobación obtenido exitosamente.</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        /// <response code="404">No se encontraron registros de estados.</response>
        [HttpGet("ApprovalStatus")]
        [ProducesResponseType(typeof(List<GenericResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> StatusGetAll()
        {
            var results = await _sender.Send(new GetApprovalStatusesQuery());
            return Ok(results);
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios del sistema y sus roles asignados.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Devuelve los usuarios registrados con su identificador, nombre, email y el rol asociado.
        /// </remarks>
        /// <returns>Lista de usuarios con su información básica y de rol.</returns>
        /// <response code="200">Listado de usuarios obtenido exitosamente.</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        /// <response code="404">No se encontraron usuarios registrados.</response>
        [HttpGet("User")]
        [ProducesResponseType(typeof(List<UserResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> UserGetAll()
        {
            var results = await _sender.Send(new GetUsersQuery());
            return Ok(results);
        }
    }
}
