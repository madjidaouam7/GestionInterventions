using System.Net;
using System.Text.Json;
using GestionInterventions.Domain.Exceptions;
using GestionInterventions.Application.Common.Exceptions;

namespace GestionInterventions.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Ressource non trouvée");

            await WriteErrorResponse(
                context,
                HttpStatusCode.NotFound,
                ex.Message);
        }
        catch (ForbiddenAccessException ex)
        {
            _logger.LogWarning(ex, "Accès refusé");
            await WriteErrorResponse(context, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Règle métier violée");
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur non gérée");
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError,
                "Une erreur interne est survenue.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new { error = message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}