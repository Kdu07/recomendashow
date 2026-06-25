using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class GlobalExceptionHandlerTests
    {
        private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
        private readonly GlobalExceptionHandler _handler;

        public GlobalExceptionHandlerTests()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
            _handler = new GlobalExceptionHandler(_loggerMock.Object);
        }

        private async Task<(DefaultHttpContext Context, ProblemDetails Problem)> HandleExceptionAsync(Exception ex)
        {
            var context = new DefaultHttpContext();
            var stream = new MemoryStream();
            context.Response.Body = stream;

            var result = await _handler.TryHandleAsync(context, ex, CancellationToken.None);
            Assert.True(result);

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            var problem = JsonSerializer.Deserialize<ProblemDetails>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return (context, problem!);
        }

        [Fact]
        public async Task TryHandleAsync_ArgumentException_ShouldReturn400()
        {
            var ex = new ArgumentException("Erro de teste");

            var (context, problem) = await HandleExceptionAsync(ex);

            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
            Assert.Equal("Requisição inválida", problem.Title);
            Assert.Equal("Erro de teste", problem.Detail);
        }

        [Fact]
        public async Task TryHandleAsync_UnauthorizedAccessException_ShouldReturn401()
        {
            var ex = new UnauthorizedAccessException("Sem acesso");

            var (context, problem) = await HandleExceptionAsync(ex);

            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
            Assert.Equal("Não autorizado", problem.Title);
            Assert.Equal("Sem acesso", problem.Detail);
        }

        [Fact]
        public async Task TryHandleAsync_NotFoundException_ShouldReturn404()
        {
            var ex = new NotFoundException("Objeto não encontrado");

            var (context, problem) = await HandleExceptionAsync(ex);

            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
            Assert.Equal("Recurso não encontrado", problem.Title);
            Assert.Equal("Objeto não encontrado", problem.Detail);
        }

        [Fact]
        public async Task TryHandleAsync_KeyNotFoundException_ShouldReturn404()
        {
            var ex = new KeyNotFoundException("Chave não encontrada");

            var (context, problem) = await HandleExceptionAsync(ex);

            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
            Assert.Equal("Recurso não encontrado", problem.Title);
            Assert.Equal("Chave não encontrada", problem.Detail);
        }

        [Fact]
        public async Task TryHandleAsync_ConflictException_ShouldReturn409()
        {
            var ex = new ConflictException("Objeto duplicado");

            var (context, problem) = await HandleExceptionAsync(ex);

            Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
            Assert.Equal("Conflito", problem.Title);
            Assert.Equal("Objeto duplicado", problem.Detail);
        }

        [Fact]
        public async Task TryHandleAsync_GenericException_ShouldReturn500()
        {
            var ex = new Exception("Erro fatal");

            var (context, problem) = await HandleExceptionAsync(ex);

            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.Equal("Erro interno", problem.Title);
            Assert.Equal("Ocorreu um erro interno. Tente novamente mais tarde.", problem.Detail);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Erro não tratado")),
                    ex,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}