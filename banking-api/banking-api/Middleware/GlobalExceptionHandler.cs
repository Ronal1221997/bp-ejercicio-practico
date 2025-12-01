using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace banking_api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Ocurrió un error: {Message}", exception.Message);

            var (statusCode, title) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),

                // InvalidOperation lo usamos para reglas de negocio (ej. Saldo insuficiente)
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Regla de negocio no cumplida"),


                ArgumentException => (StatusCodes.Status400BadRequest, "Error en los datos"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "No autorizado"),

                // Cualquier otra cosa es un error 500
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                // En producción, para errores 500, no muestres exception.Message
                Detail = statusCode == 500 ? "Ocurrió un error interno." : exception.Message,
                Type = exception.GetType().Name
            };

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
