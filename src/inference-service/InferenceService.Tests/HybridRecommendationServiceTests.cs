using AventuraML;
using FluentAssertions;
using Xunit;
using InferenceService.Services;
using InferenceService.Services.Interfaces;

namespace InferenceService.Tests;

/// <summary>
/// Testes do adaptador híbrido sobre um modelo real pequeno (HybridArtifactsFixture).
/// Cobertura equivalente à antiga suíte do SVD: ordenação por score, exclusão de
/// itens consumidos, limite n, cold-start por popularidade e validação de n.
/// </summary>
[Collection("hybrid-artifacts")]
public class HybridRecommendationServiceTests
{
    private readonly HybridRecommendationService _service;

    public HybridRecommendationServiceTests(HybridArtifactsFixture fixture)
    {
        _service = new HybridRecommendationService(new FakeLoader(fixture.BuildRecommender()));
    }

    private sealed class FakeLoader(Recommender recommender) : IHybridModelLoader
    {
        public Recommender Recommender { get; } = recommender;
        public bool IsLoaded => true;
    }

    [Fact]
    public void ModelType_DeveSerHibrido()
    {
        _service.ModelType.Should().Be("hibrido");
    }

    [Fact]
    public async Task RecommendAsync_DeveRetornarItensOrdenadosPorScoreHibrido()
    {
        var results = await _service.RecommendAsync(userId: 1, n: 10);

        results.Should().NotBeEmpty();
        results.Select(r => r.Score).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task RecommendAsync_DeveExcluirItensConsumidos()
    {
        // Usuário 1 consumiu ItemA e ItemB
        var results = await _service.RecommendAsync(userId: 1, n: 10);

        results.Should().NotContain(r => r.Espetaculo == "ItemA");
        results.Should().NotContain(r => r.Espetaculo == "ItemB");
    }

    [Fact]
    public async Task RecommendAsync_DeveTerScoreNaoNuloEmEscala01_ParaUsuarioConhecido()
    {
        var results = await _service.RecommendAsync(userId: 1, n: 10);

        results.Should().AllSatisfy(r =>
        {
            r.Score.Should().NotBeNull();
            r.Score!.Value.Should().BeInRange(0.0, 1.0);
        });
    }

    [Fact]
    public async Task RecommendAsync_DeveRespeitarLimiteN()
    {
        var results = await _service.RecommendAsync(userId: 1, n: 1);
        results.Should().HaveCount(1);
    }

    [Fact]
    public async Task RecommendAsync_DeveLancarExcecao_QuandoNMenorOuIgualZero()
    {
        var act = () => _service.RecommendAsync(userId: 1, n: 0);
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task RecommendAsync_DeveFazerColdStart_QuandoUsuarioDesconhecido()
    {
        var results = await _service.RecommendAsync(userId: 999, n: 4);

        results.Should().HaveCount(4);
        results.Should().AllSatisfy(r => r.Score.Should().BeNull());
    }

    [Fact]
    public async Task RecommendAsync_ColdStart_DeveRespeitarOrdemDePopularidade()
    {
        // Popularidade do fixture: ItemA(4) > ItemB(3) > ItemC(2) > ItemD(1)
        var results = await _service.RecommendAsync(userId: 999, n: 4);

        results.Select(r => r.Espetaculo).Should()
            .ContainInOrder("ItemA", "ItemB", "ItemC", "ItemD");
    }

    [Fact]
    public async Task RecommendAsync_ColdStart_DeveRespeitarLimiteN()
    {
        var results = await _service.RecommendAsync(userId: 999, n: 2);

        results.Should().HaveCount(2);
        results.Select(r => r.Espetaculo).Should().ContainInOrder("ItemA", "ItemB");
    }
}
