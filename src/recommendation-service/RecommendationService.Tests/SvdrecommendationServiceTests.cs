using FluentAssertions;
using NSubstitute;
using Xunit;
using RecommendationService.Data;
using RecommendationService.Models;
using RecommendationService.Services;
using RecommendationService.Services.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;

namespace RecommendationService.Tests;

public class SvdRecommendationServiceTests
{
    private static SvdWeights BuildWeights(double globalMean = 3.0)
    {
        return new SvdWeights
        {
            GlobalMean = globalMean,
            NUsers = 2,
            NItems = 3,
            RatingScale = [1.0, 5.0],
            UserIdToInner = new Dictionary<string, int> { ["1"] = 0, ["2"] = 1 },
            ItemIdToInner = new Dictionary<string, int> { ["ItemA"] = 0, ["ItemB"] = 1, ["ItemC"] = 2 },
            InnerToItemId = new Dictionary<string, string> { ["0"] = "ItemA", ["1"] = "ItemB", ["2"] = "ItemC" },
            Bu = [0.0, 0.0],
            Bi = [0.0, 0.0, 0.0],
            Pu = [[1.0, 0.0], [0.0, 1.0]],
            Qi = [[1.0, 0.0], [0.0, 0.0], [0.5, 0.0]],
            Hyperparameters = new SvdHyperparameters { NFactors = 2 }
        };
    }

    private static IModelLoaderService BuildLoader(SvdWeights? weights = null)
    {
        var loader = Substitute.For<IModelLoaderService>();
        loader.SvdWeights.Returns(weights ?? BuildWeights());
        loader.IsLoaded.Returns(true);
        return loader;
    }

    private static IDatasetRepository BuildDataset(
        int userId,
        IEnumerable<string>? consumed = null,
        IEnumerable<string>? coldStartItems = null)
    {
        var dataset = Substitute.For<IDatasetRepository>();
        var consumedSet = (consumed ?? []).ToHashSet();
        dataset.GetConsumedItems(userId).Returns(consumedSet);
        dataset.ColdStartItems.Returns((coldStartItems ?? ["ItemA", "ItemB", "ItemC"]).ToList().AsReadOnly());
        return dataset;
    }

    private static SvdRecommendationService Build(
        IModelLoaderService? loader = null,
        IDatasetRepository? dataset = null)
    {
        return new SvdRecommendationService(
            loader ?? BuildLoader(),
            dataset ?? BuildDataset(userId: 1),
            NullLogger<SvdRecommendationService>.Instance);
    }

    [Fact]
    public void ComputeAllScores_DeveRetornarScoreCorreto_ParaUsuarioConhecido()
    {
        var svc = Build();
        var scores = svc.ComputeAllScores(innerUserId: 0);
        scores.Should().HaveCount(3);
        scores[0].Should().BeApproximately(4.0, precision: 1e-9); // ItemA: 3+0+0+1.0=4.0
        scores[1].Should().BeApproximately(3.0, precision: 1e-9); // ItemB: 3+0+0+0.0=3.0
        scores[2].Should().BeApproximately(3.5, precision: 1e-9); // ItemC: 3+0+0+0.5=3.5
    }

    [Fact]
    public void ComputeAllScores_DeveFazerClip_QuandoScoreExtrapolaRatingScale()
    {
        var weights = BuildWeights(globalMean: 5.0);
        weights.Bu[0] = 2.0;
        var svc = Build(loader: BuildLoader(weights));
        var scores = svc.ComputeAllScores(innerUserId: 0);
        scores[0].Should().Be(5.0);
    }

    [Fact]
    public void ComputeAllScores_DeveFazerClip_QuandoScoreAbaixoDoMinimo()
    {
        var weights = BuildWeights(globalMean: 1.0);
        weights.Bu[0] = -5.0;
        var svc = Build(loader: BuildLoader(weights));
        var scores = svc.ComputeAllScores(innerUserId: 0);
        scores[0].Should().Be(1.0);
    }

    [Fact]
    public void TryGetInnerUserId_DeveRetornarTrue_ParaUsuarioConhecido()
    {
        var svc = Build();
        var found = svc.TryGetInnerUserId(1, out int inner);
        found.Should().BeTrue();
        inner.Should().Be(0);
    }

    [Fact]
    public void TryGetInnerUserId_DeveRetornarFalse_ParaUsuarioDesconhecido()
    {
        var svc = Build();
        var found = svc.TryGetInnerUserId(999, out int inner);
        found.Should().BeFalse();
    }

    [Fact]
    public async Task RecommendAsync_DeveRetornarItensOrdenadosPorScore()
    {
        var dataset = Substitute.For<IDatasetRepository>();
        dataset.GetConsumedItems(1).Returns(new HashSet<string> { "ItemJaVisto" });
        dataset.ColdStartItems.Returns(new List<string>().AsReadOnly());
        var svc = Build(dataset: dataset);

        var results = await svc.RecommendAsync(userId: 1, n: 3);

        results.Should().HaveCount(3);
        results[0].Espetaculo.Should().Be("ItemA");
        results[1].Espetaculo.Should().Be("ItemC");
        results[2].Espetaculo.Should().Be("ItemB");
    }

    [Fact]
    public async Task RecommendAsync_DeveExcluirItensConsumidos()
    {
        var dataset = BuildDataset(userId: 1, consumed: ["ItemA"]);
        var svc = Build(dataset: dataset);
        var results = await svc.RecommendAsync(userId: 1, n: 10);
        results.Should().NotContain(r => r.Espetaculo == "ItemA");
        results.Select(r => r.Espetaculo).Should().BeEquivalentTo(["ItemC", "ItemB"],
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task RecommendAsync_DeveRespeitarLimiteN()
    {
        var dataset = BuildDataset(userId: 1, consumed: []);
        var svc = Build(dataset: dataset);
        var results = await svc.RecommendAsync(userId: 1, n: 2);
        results.Should().HaveCount(2);
    }

    [Fact]
    public async Task RecommendAsync_DeveTerScoreNaoNulo_ParaUsuarioConhecido()
    {
        var dataset = Substitute.For<IDatasetRepository>();
        dataset.GetConsumedItems(1).Returns(new HashSet<string> { "ItemConsumedAnterior" });
        dataset.ColdStartItems.Returns(new List<string>().AsReadOnly());
        var svc = Build(dataset: dataset);
        var results = await svc.RecommendAsync(userId: 1);
        results.Should().AllSatisfy(r => r.Score.Should().NotBeNull());
    }

    [Fact]
    public async Task RecommendAsync_DeveLancarExcecao_QuandoNMenorOuIgualZero()
    {
        var svc = Build();
        await svc.Invoking(s => s.RecommendAsync(userId: 1, n: 0))
            .Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task RecommendAsync_DeveFazerColdStart_QuandoUsuarioDesconhecido()
    {
        var coldItems = new[] { "ItemA", "ItemB", "ItemC" };
        var dataset = BuildDataset(userId: 999, consumed: [], coldStartItems: coldItems);
        var svc = Build(dataset: dataset);
        var results = await svc.RecommendAsync(userId: 999, n: 3);
        results.Should().HaveCount(3);
        results.Should().AllSatisfy(r => r.Score.Should().BeNull());
    }

    [Fact]
    public async Task RecommendAsync_ColdStart_DeveExcluirItensConsumidos()
    {
        var dataset = BuildDataset(userId: 999, consumed: ["ItemA"],
            coldStartItems: ["ItemA", "ItemB", "ItemC"]);
        var svc = Build(dataset: dataset);
        var results = await svc.RecommendAsync(userId: 999, n: 10);
        results.Should().NotContain(r => r.Espetaculo == "ItemA");
    }

    [Fact]
    public async Task RecommendAsync_ColdStart_DeveRespeitarOrdemDePopularidade()
    {
        var coldItems = new[] { "ItemC", "ItemA", "ItemB" };
        var dataset = BuildDataset(userId: 999, consumed: [], coldStartItems: coldItems);
        var svc = Build(dataset: dataset);
        var results = await svc.RecommendAsync(userId: 999, n: 3);
        results.Select(r => r.Espetaculo).Should()
            .ContainInOrder("ItemC", "ItemA", "ItemB");
    }
}