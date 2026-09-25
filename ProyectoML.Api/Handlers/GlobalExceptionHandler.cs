using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoML.Application.Exceptions;
using ProyectoML.Domain.Exceptions;

namespace ProyectoML.Api.Handlers;

public class GlobalExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            DomainException => (400, "Datos inválidos"),
            ConflictException => (409, "Conflicto"),
            NotFoundException => (404, "No encontrado"),
            _ => (500, "Error interno")
        };

        context.Response.StatusCode = status;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == 500 ? "Ocurrió un error inesperado" : exception.Message
            }
        });
    }
}