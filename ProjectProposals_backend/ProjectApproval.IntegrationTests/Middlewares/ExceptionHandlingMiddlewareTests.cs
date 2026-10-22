using System.IO;
using System.Net;
using System.Text.Json;
using Application.Dtos.Responses;
using Application.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectApproval.Api.Middlewares;
using Xunit;

namespace ProjectApproval.IntegrationTests.Middlewares;

public class ExceptionHandlingMiddlewareTests
{
    private static async Task<(int StatusCode, ApiErrorResponse? Response)> InvokeMiddlewareWithExceptionAsync(Exception thrownException)
    {
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw thrownException,
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return (context.Response.StatusCode, errorResponse);
    }

    [Fact]
    public async Task WhenNotFoundExceptionThrown_Returns404WithOriginalMessage()
    {
        var ex = new NotFoundException("El recurso solicitado no fue encontrado.");

        var (statusCode, response) = await InvokeMiddlewareWithExceptionAsync(ex);

        statusCode.Should().Be((int)HttpStatusCode.NotFound);
        response.Should().NotBeNull();
        response!.message.Should().Be("El recurso solicitado no fue encontrado.");
    }

    [Fact]
    public async Task WhenBadRequestExceptionThrown_Returns400WithOriginalMessage()
    {
        var ex = new BadRequestException("Parámetros de consulta incorrectos.");

        var (statusCode, response) = await InvokeMiddlewareWithExceptionAsync(ex);

        statusCode.Should().Be((int)HttpStatusCode.BadRequest);
        response.Should().NotBeNull();
        response!.message.Should().Be("Parámetros de consulta incorrectos.");
    }

    [Fact]
    public async Task WhenConflictExceptionThrown_Returns409WithOriginalMessage()
    {
        var ex = new ConflictException("Ya existe una propuesta con el mismo título.");

        var (statusCode, response) = await InvokeMiddlewareWithExceptionAsync(ex);

        statusCode.Should().Be((int)HttpStatusCode.Conflict);
        response.Should().NotBeNull();
        response!.message.Should().Be("Ya existe una propuesta con el mismo título.");
    }

    [Fact]
    public async Task WhenUnauthorizedExceptionThrown_Returns401WithOriginalMessage()
    {
        var ex = new UnauthorizedException("Credenciales no válidas.");

        var (statusCode, response) = await InvokeMiddlewareWithExceptionAsync(ex);

        statusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        response.Should().NotBeNull();
        response!.message.Should().Be("Credenciales no válidas.");
    }

    [Fact]
    public async Task WhenForbiddenExceptionThrown_Returns403WithOriginalMessage()
    {
        var ex = new ForbiddenException("No tiene permisos para modificar este recurso.");

        var (statusCode, response) = await InvokeMiddlewareWithExceptionAsync(ex);

        statusCode.Should().Be((int)HttpStatusCode.Forbidden);
        response.Should().NotBeNull();
        response!.message.Should().Be("No tiene permisos para modificar este recurso.");
    }

    [Fact]
    public async Task WhenArgumentExceptionThrown_Returns400WithOriginalMessage()
    {
        var ex = new ArgumentException("Argumento inválido suministrado.");

        var (statusCode, response) = await InvokeMiddlewareWithExceptionAsync(ex);

        statusCode.Should().Be((int)HttpStatusCode.BadRequest);
        response.Should().NotBeNull();
        response!.message.Should().Be("Argumento inválido suministrado.");
    }

    [Fact]
    public async Task WhenGenericUnhandledExceptionThrown_Returns500WithSanitizedMessage()
    {
        var ex = new InvalidOperationException("Fatal database crash: root connection password leaked!");

        var (statusCode, response) = await InvokeMiddlewareWithExceptionAsync(ex);

        statusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        response.Should().NotBeNull();
        response!.message.Should().Be("Ha ocurrido un error inesperado en el servidor.",
            "Database/system internal stack traces must not be exposed to client");
    }

    [Fact]
    public async Task WhenNoExceptionThrown_CallsNextDelegate()
    {
        var called = false;
        var middleware = new ExceptionHandlingMiddleware(
            _ =>
            {
                called = true;
                return Task.CompletedTask;
            },
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
    }
}
