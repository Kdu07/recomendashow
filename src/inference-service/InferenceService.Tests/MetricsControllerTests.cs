using AventuraML.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using InferenceService.Controllers;
using InferenceService.Data;
using InferenceService.DTOs;
using InferenceService.Services;
using InferenceService.Services.Interfaces;

namespace InferenceService.Tests;

public class MetricsControllerValidationTests
{
    private static MetricsController BuildController() =>
        new(null!, Substitute.For<IChartService>());

    [Fact]
    public void GetMetrics_WhenKIsZero_ReturnsBadRequest()
    {
        var result = BuildController().GetMetrics(k: 0);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void GetMetrics_WhenRankingUsersIsZero_ReturnsBadRequest()
    {
        var result = BuildController().GetMetrics(rankingUsers: 0);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void GetSvdVsHybrid_WhenKIsZero_ReturnsBadRequest()
    {
        var result = BuildController().GetSvdVsHybrid(k: 0);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void GetSvdVsHybrid_WhenRankingUsersIsZero_ReturnsBadRequest()
    {
        var result = BuildController().GetSvdVsHybrid(rankingUsers: 0);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}

[Collection("hybrid-artifacts")]
public class MetricsControllerHappyPathTests
{
    private readonly HybridArtifactsFixture _fixture;

    public MetricsControllerHappyPathTests(HybridArtifactsFixture fixture)
    {
        _fixture = fixture;
    }

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

    private MetricsController BuildController()
    {
        var loader = Substitute.For<IHybridModelLoader>();
        loader.Recommender.Returns(_fixture.BuildRecommender());

        var dataset = Substitute.For<IDatasetRepository>();
        dataset.AllInteractions.Returns(FixtureInteractions());

        var metricsService = new MetricsService(loader, dataset);

        var charts = Substitute.For<IChartService>();
        charts.GenerateErrorHistogram(Arg.Any<IReadOnlyList<double>>(), Arg.Any<string>())
            .Returns(new byte[] { 0 });
        charts.GenerateRealVsPredictedScatter(
                Arg.Any<IReadOnlyList<double>>(), Arg.Any<IReadOnlyList<double>>(), Arg.Any<string>())
            .Returns(new byte[] { 0 });
        charts.GenerateRankingComparisonChart(
                Arg.Any<RankingMetrics>(), Arg.Any<RankingMetrics>(), Arg.Any<int>())
            .Returns(new byte[] { 0 });

        return new MetricsController(metricsService, charts);
    }

    [Fact]
    public void GetMetrics_WithValidParameters_Returns200WithMetricsResponse()
    {
        var result = BuildController().GetMetrics(k: 2, rankingUsers: 5);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<MetricsResponse>();
    }

    [Fact]
    public void GetErrorHistogram_ReturnsPngFile()
    {
        var result = BuildController().GetErrorHistogram();

        result.Should().BeOfType<FileContentResult>()
            .Which.ContentType.Should().Be("image/png");
    }

    [Fact]
    public void GetRealVsPredicted_ReturnsPngFile()
    {
        var result = BuildController().GetRealVsPredicted();

        result.Should().BeOfType<FileContentResult>()
            .Which.ContentType.Should().Be("image/png");
    }

    [Fact]
    public void GetSvdVsHybrid_WithValidParameters_ReturnsPngFile()
    {
        var result = BuildController().GetSvdVsHybrid(k: 2, rankingUsers: 5);

        result.Should().BeOfType<FileContentResult>()
            .Which.ContentType.Should().Be("image/png");
    }
}
