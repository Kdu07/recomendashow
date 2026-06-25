using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using InferenceService.Controllers;
using InferenceService.Data;
using InferenceService.DTOs;
using InferenceService.Services.Interfaces;

namespace InferenceService.Tests;

public class DiagnosticsControllerHealthTests
{
    private static DiagnosticsController BuildController(bool isLoaded)
    {
        var loader = Substitute.For<IHybridModelLoader>();
        loader.IsLoaded.Returns(isLoaded);
        return new DiagnosticsController(loader, Substitute.For<IDatasetRepository>());
    }

    [Fact]
    public void Health_WhenModelIsLoaded_Returns200()
    {
        var result = BuildController(isLoaded: true).Health();

        result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public void Health_WhenModelIsNotLoaded_Returns503()
    {
        var result = BuildController(isLoaded: false).Health();

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(503);
    }
}

[Collection("hybrid-artifacts")]
public class DiagnosticsControllerModelInfoTests
{
    private readonly HybridArtifactsFixture _fixture;

    public DiagnosticsControllerModelInfoTests(HybridArtifactsFixture fixture)
    {
        _fixture = fixture;
    }

    private DiagnosticsController BuildController(
        IReadOnlyList<InteractionRecord>? interactions = null,
        IReadOnlySet<string>? items = null,
        IReadOnlyList<string>? coldStart = null)
    {
        var recommender = _fixture.BuildRecommender();

        var loader = Substitute.For<IHybridModelLoader>();
        loader.Recommender.Returns(recommender);

        var dataset = Substitute.For<IDatasetRepository>();
        dataset.AllInteractions.Returns(interactions ?? []);
        dataset.AllItems.Returns(items ?? (IReadOnlySet<string>)new HashSet<string>());
        dataset.ColdStartItems.Returns(coldStart ?? []);

        return new DiagnosticsController(loader, dataset);
    }

    [Fact]
    public void ModelInfo_ReturnsModelMetadata_FromRecommender()
    {
        var recommender = _fixture.BuildRecommender(alpha: 0.7f);
        var loader = Substitute.For<IHybridModelLoader>();
        loader.Recommender.Returns(recommender);
        var controller = new DiagnosticsController(loader, Substitute.For<IDatasetRepository>());

        var result = controller.ModelInfo();

        var response = result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ModelInfoResponse>().Subject;

        response.Model.Alpha.Should().Be(recommender.Alpha);
        response.Model.KnownUsers.Should().Be(recommender.KnownUserCount);
        response.Model.CatalogItems.Should().Be(recommender.CatalogSize);
    }

    [Fact]
    public void ModelInfo_ReturnsDatasetInfo_FromRepository()
    {
        var interactions = new List<InteractionRecord>
        {
            new() { UserId = 1, ItemId = "ItemA", Ranking = 5 },
            new() { UserId = 2, ItemId = "ItemB", Ranking = 3 },
        };
        var items = new HashSet<string> { "ItemA", "ItemB", "ItemC" };
        var coldStart = new List<string> { "ItemA", "ItemB" };

        var result = BuildController(interactions, items, coldStart).ModelInfo();

        var response = result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ModelInfoResponse>().Subject;

        response.Dataset.TotalInteractions.Should().Be(2);
        response.Dataset.TotalItems.Should().Be(3);
        response.Dataset.ColdStartItemsCount.Should().Be(2);
    }

    [Fact]
    public void ModelInfo_Returns200()
    {
        var result = BuildController().ModelInfo();

        result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }
}
