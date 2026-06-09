// RecommendationService.Tests/RecommendationBatchJobTests.cs
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using RecommendationService.Data;
using RecommendationService.Models;
using RecommendationService.Services;
using RecommendationService.Services.Interfaces;

namespace RecommendationService.Tests;

public class RecommendationBatchJobTests
{
    private readonly IRecommendationService _recommendationService;
    private readonly IRecommendationWriteRepository _writeRepository;
    private readonly IDatasetRepository _dataset;
    private readonly IServiceProvider _serviceProvider;

    public RecommendationBatchJobTests()
    {
        _recommendationService = Substitute.For<IRecommendationService>();
        _writeRepository = Substitute.For<IRecommendationWriteRepository>();
        _dataset = Substitute.For<IDatasetRepository>();

        _recommendationService.ModelType.Returns("svd");
        _recommendationService.RecommendAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<IReadOnlyList<RecommendationResult>>([]));

        var services = new ServiceCollection();
        services.AddSingleton(_writeRepository);
        _serviceProvider = services.BuildServiceProvider();
    }

    private RecommendationBatchJob BuildJob() =>
        new(_recommendationService, _serviceProvider, _dataset,
            NullLogger<RecommendationBatchJob>.Instance);

    // -----------------------------------------------------------------------
    // Processamento de usuários
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_DeveProcessarTodosOsUsuariosDoDataset()
    {
        _dataset.GetAllUserIds().Returns([1, 2, 3]);

        var cts = new CancellationTokenSource();
        var job = BuildJob();

        // Cancela após primeira iteração
        _ = Task.Run(async () =>
        {
            await Task.Delay(200);
            cts.Cancel();
        });

        await job.StartAsync(cts.Token);
        await job.StopAsync(CancellationToken.None);

        await _recommendationService.Received(1).RecommendAsync(1, 10);
        await _recommendationService.Received(1).RecommendAsync(2, 10);
        await _recommendationService.Received(1).RecommendAsync(3, 10);
    }

    [Fact]
    public async Task ExecuteAsync_DeveSalvarRecomendacoesParaCadaUsuario()
    {
        var recs = new List<RecommendationResult> { new("ItemA", 4.5) };
        _dataset.GetAllUserIds().Returns([1, 2]);
        _recommendationService.RecommendAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<IReadOnlyList<RecommendationResult>>(recs));

        var cts = new CancellationTokenSource();
        var job = BuildJob();

        _ = Task.Run(async () =>
        {
            await Task.Delay(200);
            cts.Cancel();
        });

        await job.StartAsync(cts.Token);
        await job.StopAsync(CancellationToken.None);

        await _writeRepository.Received(1).SaveAsync(1, recs, "svd");
        await _writeRepository.Received(1).SaveAsync(2, recs, "svd");
    }

    // -----------------------------------------------------------------------
    // Resiliência
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_DeveContinuar_QuandoUmUsuarioFalha()
    {
        _dataset.GetAllUserIds().Returns([1, 2, 3]);
        _recommendationService.RecommendAsync(2, 10)
            .ThrowsAsync(new Exception("Falha simulada"));

        var cts = new CancellationTokenSource();
        var job = BuildJob();

        _ = Task.Run(async () =>
        {
            await Task.Delay(200);
            cts.Cancel();
        });

        await job.StartAsync(cts.Token);
        await job.StopAsync(CancellationToken.None);

        await _recommendationService.Received(1).RecommendAsync(1, 10);
        await _recommendationService.Received(1).RecommendAsync(3, 10);
    }

    [Fact]
    public async Task ExecuteAsync_DeveContinuar_QuandoSalvarFalha()
    {
        _dataset.GetAllUserIds().Returns([1, 2]);
        _writeRepository.SaveAsync(1, Arg.Any<IReadOnlyList<RecommendationResult>>(), Arg.Any<string>())
            .ThrowsAsync(new Exception("Falha ao salvar"));

        var cts = new CancellationTokenSource();
        var job = BuildJob();

        _ = Task.Run(async () =>
        {
            await Task.Delay(200);
            cts.Cancel();
        });

        await job.StartAsync(cts.Token);
        await job.StopAsync(CancellationToken.None);

        await _recommendationService.Received(1).RecommendAsync(2, 10);
    }

    // -----------------------------------------------------------------------
    // Cancelamento
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_DeveParar_QuandoCancelado()
    {
        _dataset.GetAllUserIds().Returns([1, 2, 3]);

        var cts = new CancellationTokenSource();
        cts.Cancel(); // cancela imediatamente

        var job = BuildJob();
        await job.StartAsync(cts.Token);
        await job.StopAsync(CancellationToken.None);

        await _recommendationService.DidNotReceive().RecommendAsync(Arg.Any<int>(), Arg.Any<int>());
    }

    // -----------------------------------------------------------------------
    // Dataset vazio
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_ComDatasetVazio_NaoDeveChamarRecommendAsync()
    {
        _dataset.GetAllUserIds().Returns([]);

        var cts = new CancellationTokenSource();
        var job = BuildJob();

        _ = Task.Run(async () =>
        {
            await Task.Delay(200);
            cts.Cancel();
        });

        await job.StartAsync(cts.Token);
        await job.StopAsync(CancellationToken.None);

        await _recommendationService.DidNotReceive().RecommendAsync(Arg.Any<int>(), Arg.Any<int>());
    }
}