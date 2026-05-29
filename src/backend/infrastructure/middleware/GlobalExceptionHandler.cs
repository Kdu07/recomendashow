using BackEnd.Aventura.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Infrastructure.Middleware
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
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
            var (statusCode, title) = MapException(exception);

            if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);
            }

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode >= StatusCodes.Status500InternalServerError
                    ? "Ocorreu um erro interno. Tente novamente mais tarde."
                    : exception.Message
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }

        private static (int StatusCode, string Title) MapException(Exception exception) =>
            exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Não autorizado"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno")
            };
    }
}
