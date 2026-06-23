using FluentAssertions;
using NSubstitute;
using Xunit;
using InferenceService.Data;
using InferenceService.Services;
using InferenceService.Services.Interfaces;

namespace InferenceService.Tests;

[Collection("hybrid-artifacts")]
public class MetricsServiceTests
{
    private readonly HybridArtifactsFixture _fixture;

    public MetricsServiceTests(HybridArtifactsFixture fixture)
    {
        _fixture = fixture;
    }

    // Mirrors the fixture dataset so PredictRating operates on known users/items.
    private static IReadOnlyList<InteractionRecord> FixtureInteractions() =>
    [
        new() { UserId = 1, ItemId = "ItemA", Ranking = 5, ValorMedioIngresso = 50.0, TipoEvento = "Teatro" },
        new() { UserId = 1, ItemId = "ItemB", Ranking = 4, ValorMedioIngresso = 80.0, TipoEvento = "Musical" },
        new() { UserId = 2, ItemId = "ItemA", Ranking = 3, ValorMedioIngresso = 50.0, TipoEvento = "Teatro" },
        new() { UserId = 2, ItemId = "ItemC", Ranking = 5, ValorMedioIngresso = 30.0, TipoEvento = "Danca" },
        new() { UserId = 3, ItemId = "ItemA", Ranking = 4, ValorMedioIngresso = 50.0, TipoEvento = "Teatro" },
        new() { UserId = 3, ItemId = "ItemB", Ranking = 2, ValorMedioIngresso = 80.0, TipoEvento = "Musical" },
        new() { UserId = 4, ItemId = "ItemA", Ranking = 1, ValorMedioIngresso = 50.0, TipoEvento = "Teatro" },
        new() { UserId = 4, ItemId = "ItemB", Ranking = 5, ValorMedioIngresso = 80.0, TipoEvento = "Musical" },
        new() { UserId = 4, ItemId = "ItemC", Ranking = 2, ValorMedioIngresso = 30.0, TipoEvento = "Danca" },
        new() { UserId = 5, ItemId = "ItemD", Ranking = 3, ValorMedioIngresso = 100.0, TipoEvento = "Opera" },
    ];

    private MetricsService BuildService(IDatasetRepository? dataset = null)
    {
        var loader = Substitute.For<IHybridModelLoader>();
        loader.Recommender.Returns(_fixture.BuildRecommender());

        dataset ??= BuildDataset();
        return new MetricsService(loader, dataset);
    }

    private static IDatasetRepository BuildDataset(IReadOnlyList<InteractionRecord>? interactions = null)
    {
        var dataset = Substitute.For<IDatasetRepository>();
        dataset.AllInteractions.Returns(interactions ?? FixtureInteractions());
        return dataset;
    }

    // -----------------------------------------------------------------------
    // Evaluate
    // -----------------------------------------------------------------------

    [Fact]
    public void Evaluate_ReturnsReport_WithPositiveUsersEvaluated()
    {
        var report = BuildService().Evaluate(topK: 2);

        report.UsersEvaluated.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Evaluate_ReturnsReport_WithAlphaFromRecommender()
    {
        var report = BuildService().Evaluate(topK: 2);

        report.Alpha.Should().Be(_fixture.BuildRecommender().Alpha);
    }

    [Fact]
    public void Evaluate_ReturnsReport_WithNonNegativeRmse()
    {
        var report = BuildService().Evaluate(topK: 2);

        report.Rating.Rmse.Should().BeGreaterThanOrEqualTo(0);
    }

    // -----------------------------------------------------------------------
    // Cache
    // -----------------------------------------------------------------------

    [Fact]
    public void Evaluate_WithSameParameters_ReturnsCachedResult()
    {
        var dataset = BuildDataset();
        var service = BuildService(dataset);

        var first = service.Evaluate(topK: 2, rankingUsers: 5);
        var second = service.Evaluate(topK: 2, rankingUsers: 5);

        second.Should().BeSameAs(first);
        _ = dataset.Received(1).AllInteractions;
    }

    [Fact]
    public void Evaluate_WithDifferentTopK_DoesNotReturnCachedResult()
    {
        var service = BuildService();

        var report2 = service.Evaluate(topK: 2);
        var report3 = service.Evaluate(topK: 3);

        report2.Should().NotBeSameAs(report3);
        report2.TopK.Should().Be(2);
        report3.TopK.Should().Be(3);
    }

    // -----------------------------------------------------------------------
    // GetPredictionErrors
    // -----------------------------------------------------------------------

    [Fact]
    public void GetPredictionErrors_ReturnsNonNullList()
    {
        var errors = BuildService().GetPredictionErrors(topK: 2);

        errors.Should().NotBeNull();
    }

    // -----------------------------------------------------------------------
    // GetRealAndPredicted
    // -----------------------------------------------------------------------

    [Fact]
    public void GetRealAndPredicted_RealAndPredicted_HaveMatchingCount()
    {
        var (real, predicted) = BuildService().GetRealAndPredicted(topK: 2);

        real.Count.Should().Be(predicted.Count);
    }
}
