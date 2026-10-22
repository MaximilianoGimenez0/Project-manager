using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Dtos.Requests;
using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Features.ProjectProposals.Commands.CreateProjectProposal;
using Application.Features.ProjectProposals.Commands.DecideProjectStep;
using Application.Features.ProjectProposals.Commands.UpdateProjectProposal;
using Application.Features.ProjectProposals.Queries.GetFilteredProjects;
using Application.Features.ProjectProposals.Queries.GetProjectProposalById;
using Application.Features.ProjectProposals.Queries.GetRoleApprovals;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectApproval.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión y flujo de aprobación de propuestas de proyectos (creación, consultas, actualizaciones y decisiones).
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/project")]
    public class ProjectController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUserService;

        public ProjectController(ISender sender, ICurrentUserService currentUserService)
        {
            _sender = sender;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Crea una nueva propuesta de proyecto y calcula automáticamente su flujo de pasos de aprobación según las reglas de negocio.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Asigna automáticamente al usuario autenticado como creador (<c>CreateBy</c>) para evitar suplantaciones de identidad.
        /// Valida la existencia del Área y Tipo de Proyecto, la unicidad del título y calcula los pasos ordenados de aprobación.
        /// </remarks>
        /// <response code="201">Propuesta creada exitosamente.</response>
        /// <response code="400">Datos obligatorios ausentes, área/tipo inexistente o título duplicado.</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectResponse), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<IActionResult> Create([FromBody] ProjectProposalCreateRequest request)
        {
            if (_currentUserService.UserId.HasValue)
            {
                request.User = _currentUserService.UserId.Value;
            }

            ValidateModelState();
            var result = await _sender.Send(new CreateProjectProposalCommand(request));
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Obtiene el detalle completo de una propuesta de proyecto y el historial de sus pasos de aprobación por su ID.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Devuelve los datos generales del proyecto, estado global, solicitante y el detalle de cada paso del flujo con sus aprobadores y observaciones.
        /// </remarks>
        /// <response code="200">Propuesta de proyecto encontrada.</response>
        /// <response code="400">El identificador proporcionado es inválido o vacío.</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        /// <response code="404">Propuesta de proyecto no encontrada.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProjectResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new BadRequestException("El ID del proyecto no puede estar vacío.");
            }

            var result = await _sender.Send(new GetProjectProposalByIdQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un listado resumido de propuestas de proyectos filtradas por criterios de búsqueda opcionales.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Permite buscar por coincidencia parcial de título, estado general de aprobación, usuario solicitante y usuario aprobador interviniente.
        /// </remarks>
        /// <param name="title">Filtro opcional por texto en el título del proyecto.</param>
        /// <param name="status">Filtro opcional por estado de aprobación (Pending = 1, Approved = 2, Rejected = 3, Observed = 4).</param>
        /// <param name="applicant">Filtro opcional por identificador de usuario solicitante/creador.</param>
        /// <param name="approvalUser">Filtro opcional por identificador de usuario aprobador de algún paso.</param>
        /// <response code="200">Listado de proyectos obtenido exitosamente.</response>
        /// <response code="400">Parámetros de búsqueda con formato inválido (título superior a 255 caracteres).</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProjectShortResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] string? title,
            [FromQuery] int? status,
            [FromQuery] int? applicant,
            [FromQuery] int? approvalUser)
        {
            ValidateModelState();

            if (title != null && title.Length > 255)
            {
                throw new BadRequestException("El título no puede superar 255 caracteres.");
            }

            var results = await _sender.Send(new GetFilteredProjectsQuery(title, status, applicant, approvalUser));
            return Ok(results);
        }

        /// <summary>
        /// Modifica la información básica (título, descripción, monto estimado o duración) de una propuesta de proyecto existente.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Permite actualizar atributos generales del proyecto antes de que se concluya el flujo de aprobaciones.
        /// </remarks>
        /// <response code="200">Propuesta modificada exitosamente.</response>
        /// <response code="400">Identificador vacío o datos de actualización inválidos.</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        /// <response code="404">Propuesta no encontrada.</response>
        /// <response code="409">El estado actual de la propuesta no permite modificaciones.</response>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(typeof(ProjectResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 409)]
        public async Task<IActionResult> UpdateProjectInformation(Guid id, [FromBody] ProjectUpdateRequest request)
        {
            if (id == Guid.Empty)
            {
                throw new BadRequestException("El ID del proyecto no puede estar vacío.");
            }

            ValidateModelState();
            var results = await _sender.Send(new UpdateProjectProposalCommand(id, request));
            return Ok(results);
        }

        /// <summary>
        /// Registra la decisión (Aprobar, Rechazar u Observar) sobre el próximo paso pendiente del flujo de aprobación.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Asigna automáticamente al usuario autenticado como decisor del paso (<c>ApproverUserId</c>).
        /// Valida de forma estricta que:
        /// 1. El proyecto no se encuentre en un estado terminal (Approved o Rejected).
        /// 2. El identificador del paso corresponda al próximo paso pendiente de aprobación en la secuencia (<c>StepOrder</c>).
        /// 3. El rol del usuario autenticado coincida con el rol requerido por el paso (<c>ApproverRoleId</c>).
        /// 
        /// Recalcula el estado general del proyecto: si se rechaza pasa a <c>Rejected</c>, si todos los pasos son aprobados pasa a <c>Approved</c>, si se observa pasa a <c>Observed</c>.
        /// </remarks>
        /// <param name="id">Identificador único de la propuesta de proyecto.</param>
        /// <param name="request">Datos de la decisión: Id del paso, Status (2 = Approved, 3 = Rejected, 4 = Observed) y Observation opcional.</param>
        /// <response code="200">Decisión registrada exitosamente.</response>
        /// <response code="400">Paso no correspondiente, rol incompatible o validación fallida.</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        /// <response code="404">Proyecto o paso no encontrado.</response>
        /// <response code="409">El proyecto ya se encuentra en un estado terminal que no admite cambios.</response>
        [HttpPatch("{id:guid}/decision")]
        [ProducesResponseType(typeof(ProjectResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 409)]
        public async Task<IActionResult> ChangeSepStatus(Guid id, [FromBody] DecisionStepUpdateRequest request)
        {
            if (id == Guid.Empty)
            {
                throw new BadRequestException("El ID del proyecto no puede estar vacío.");
            }

            if (_currentUserService.UserId.HasValue)
            {
                request.User = _currentUserService.UserId.Value;
            }

            ValidateModelState();
            var result = await _sender.Send(new DecideProjectStepCommand(id, request));
            return Ok(result);
        }

        /// <summary>
        /// Obtiene todas las propuestas de proyectos pendientes de aprobación que requieren la decisión de un rol específico.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación (<c>Bearer &lt;token&gt;</c>).
        /// Permite a los usuarios de un rol aprobador consultar su bandeja de trabajo (proyectos esperando su intervención).
        /// </remarks>
        /// <param name="role">Identificador del rol aprobador (1: Líder de Área, 2: Gerente, 3: Director, 4: Comité Técnico).</param>
        /// <response code="200">Lista de propuestas pendientes obtenida exitosamente.</response>
        /// <response code="400">El rol especificado se encuentra fuera del rango permitido.</response>
        /// <response code="401">No autorizado (token ausente o inválido).</response>
        /// <response code="404">No se encontraron proyectos pendientes para el rol.</response>
        [HttpGet("rolepending/{role:int}")]
        [ProducesResponseType(typeof(List<ProjectResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<IActionResult> GetRoleApprovals(int role)
        {
            if (role < 0 || role > 999)
            {
                throw new BadRequestException("El rol no puede ser menor a 0 ni mayor a 999.");
            }

            var result = await _sender.Send(new GetRoleApprovalsQuery(role));
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