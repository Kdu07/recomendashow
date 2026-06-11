using FluentAssertions;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using InferenceService.Services;

namespace InferenceService.Tests;

/// <summary>
/// Testes do HybridModelLoader: validação fail-fast dos artefatos no construtor
/// e carregamento do Recommender no StartAsync (antes da primeira requisição).
/// </summary>
[Collection("hybrid-artifacts")]
public class HybridModelLoaderTests
{
    private readonly HybridArtifactsFixture _fixture;

    public HybridModelLoaderTests(HybridArtifactsFixture fixture)
    {
        _fixture = fixture;
    }

    private HybridModelLoader BuildLoader(ModelPathsOptions? paths = null, float alpha = 0.7f) =>
        new(
            Options.Create(paths ?? _fixture.BuildPaths()),
            Options.Create(new RecommendationOptions { Alpha = alpha }),
            NullLogger<HybridModelLoader>.Instance);

    [Fact]
    public void Construtor_ComArtefatoInexistente_DeveLancarFileNotFoundException()
    {
        var paths = _fixture.BuildPaths() with
        {
            ModelZip = Path.Combine(_fixture.Dir, "nao_existe", "model.zip"),
        };

        var act = () => BuildLoader(paths);

        act.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void IsLoaded_DeveSerFalse_AntesDeStartAsync()
    {
        var loader = BuildLoader();
        loader.IsLoaded.Should().BeFalse();
    }

    [Fact]
    public void Recommender_AntesDeStartAsync_DeveLancarInvalidOperationException()
    {
        var loader = BuildLoader();
        var act = () => _ = loader.Recommender;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public async Task StartAsync_DeveCarregarModeloHibrido()
    {
        var loader = BuildLoader();

        await loader.StartAsync(CancellationToken.None);

        loader.IsLoaded.Should().BeTrue();
        loader.Recommender.Should().NotBeNull();
    }

    [Fact]
    public async Task StartAsync_DeveExporMetadadosDoModelo()
    {
        var loader = BuildLoader(alpha: 0.6f);

        await loader.StartAsync(CancellationToken.None);

        // Fixture: 5 usuários com histórico, 4 itens no catálogo
        loader.Recommender.KnownUserCount.Should().Be(5);
        loader.Recommender.CatalogSize.Should().Be(4);
        loader.Recommender.Alpha.Should().Be(0.6f);
    }
}
