using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using InferenceService.Infrastructure.Middleware;

namespace InferenceService.Tests;

public class ExceptionHandlingMiddlewareTests
{
    private static ILogger<ExceptionHandlingMiddleware> Logger() =>
        Substitute.For<ILogger<ExceptionHandlingMiddleware>>();

    private static async Task<(int StatusCode, string Body)> InvokeAsync(RequestDelegate next)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(next, Logger());
        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        return (context.Response.StatusCode, body);
    }

    private static string? ErrorField(string json) =>
        JsonDocument.Parse(json).RootElement.GetProperty("error").GetString();

    [Fact]
    public async Task ArgumentOutOfRangeException_Returns400_AndPropagatesMessage()
    {
        var (status, body) = await InvokeAsync(
            _ => throw new ArgumentOutOfRangeException("param", "must be positive"));

        status.Should().Be(400);
        ErrorField(body).Should().Contain("must be positive");
    }

    [Fact]
    public async Task ArgumentException_Returns400_AndPropagatesMessage()
    {
        var (status, body) = await InvokeAsync(
            _ => throw new ArgumentException("invalid argument"));

        status.Should().Be(400);
        ErrorField(body).Should().Contain("invalid argument");
    }

    [Fact]
    public async Task FileNotFoundException_Returns500_WithFixedMessage()
    {
        var (status, body) = await InvokeAsync(
            _ => throw new FileNotFoundException("model.zip not found"));

        status.Should().Be(500);
        ErrorField(body).Should().Contain("ModelPaths");
    }

    [Fact]
    public async Task GenericException_Returns500_WithFixedMessage()
    {
        var (status, body) = await InvokeAsync(
            _ => throw new Exception("something unexpected"));

        status.Should().Be(500);
        ErrorField(body).Should().Be("Erro interno do servidor.");
    }

    [Fact]
    public async Task NoException_PassesThroughWithoutAlteringStatus()
    {
        var (status, body) = await InvokeAsync(_ => Task.CompletedTask);

        status.Should().Be(200);
        body.Should().BeEmpty();
    }

    [Fact]
    public async Task ErrorResponse_HasJsonContentType()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new Exception("err"), Logger());
        await middleware.InvokeAsync(context);

        context.Response.ContentType.Should().Contain("application/json");
    }
}
