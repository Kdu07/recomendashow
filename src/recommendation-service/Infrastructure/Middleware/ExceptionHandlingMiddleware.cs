using System.Net;
using System.Text.Json;

namespace RecommendationService.Infrastructure.Middleware;

/// <summary>
/// Captura exceções não tratadas e retorna respostas JSON padronizadas.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

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
        catch (ArgumentOutOfRangeException ex)
        {
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "Arquivo não encontrado.");
            await WriteError(context, HttpStatusCode.InternalServerError,
                "Arquivo de modelo ou dataset não encontrado. Verifique a configuração ModelPaths.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado.");
            await WriteError(context, HttpStatusCode.InternalServerError,
                "Erro interno do servidor.");
        }
    }

    private static async Task WriteError(HttpContext ctx, HttpStatusCode status, string message)
    {
        ctx.Response.StatusCode = (int)status;
        ctx.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new { error = message }, _jsonOpts);
        await ctx.Response.WriteAsync(body);
    }
}
