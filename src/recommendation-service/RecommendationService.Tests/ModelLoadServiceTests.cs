using FluentAssertions;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RecommendationService.Models;
using RecommendationService.Services;

namespace RecommendationService.Tests;

/// <summary>
/// Testes unitários para ModelLoaderService.
/// Cobre: carregamento e validação dos pesos SVD a partir de JSON.
/// Estratégia: cada teste escreve um svd_weights.json temporário em disco.
/// </summary>
public class ModelLoaderServiceTests : IDisposable
{
    private readonly List<string> _tempFiles = [];

    // -----------------------------------------------------------------------
    // Helper
    // -----------------------------------------------------------------------

    private ModelLoaderService BuildService(string json)
    {
        var path = Path.GetTempFileName();
        _tempFiles.Add(path);
        File.WriteAllText(path, json);

        var opts = Options.Create(new ModelPathsOptions
        {
            SvdWeightsJson = path,
            DatasetCsv = "unused"
        });

        return new ModelLoaderService(opts, NullLogger<ModelLoaderService>.Instance);
    }

    /// JSON mínimo válido com 1 usuário e 2 itens, 2 fatores latentes.
    private const string ValidWeightsJson = """
        {
          "model_type": "funk_svd",
          "hyperparameters": { "n_factors": 2, "lr_all": 0.001, "reg_all": 0.2, "n_epochs": 50 },
          "rating_scale": [1.0, 5.0],
          "global_mean": 3.5,
          "n_users": 1,
          "n_items": 2,
          "user_id_to_inner": { "10": 0 },
          "item_id_to_inner": { "ShowA": 0, "ShowB": 1 },
          "inner_to_item_id": { "0": "ShowA", "1": "ShowB" },
          "bu": [0.1],
          "bi": [0.2, -0.1],
          "pu": [[0.5, 0.3]],
          "qi": [[0.4, 0.2], [0.1, 0.6]]
        }
        """;

    // -----------------------------------------------------------------------
    // IsLoaded
    // -----------------------------------------------------------------------

    [Fact]
    public async Task IsLoaded_DeveSerFalse_AntesDeStartAsync()
    {
        var svc = BuildService(ValidWeightsJson);
        svc.IsLoaded.Should().BeFalse();
    }

    [Fact]
    public async Task IsLoaded_DeveSerTrue_AposStartAsync()
    {
        var svc = BuildService(ValidWeightsJson);
        await svc.StartAsync(CancellationToken.None);
        svc.IsLoaded.Should().BeTrue();
    }

    // -----------------------------------------------------------------------
    // SvdWeights — propriedades escalares
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SvdWeights_DeveDeserializarGlobalMeanCorretamente()
    {
        var svc = BuildService(ValidWeightsJson);
        await svc.StartAsync(CancellationToken.None);
        svc.SvdWeights.GlobalMean.Should().BeApproximately(3.5, 1e-9);
    }

    [Fact]
    public async Task SvdWeights_DeveDeserializarNUsersENItemsCorretamente()
    {
        var svc = BuildService(ValidWeightsJson);
        await svc.StartAsync(CancellationToken.None);
        svc.SvdWeights.NUsers.Should().Be(1);
        svc.SvdWeights.NItems.Should().Be(2);
    }

    [Fact]
    public async Task SvdWeights_DeveDeserializarRatingScaleCorretamente()
    {
        var svc = BuildService(ValidWeightsJson);
        await svc.StartAsync(CancellationToken.None);
        svc.SvdWeights.RatingScale.Should().Equal(1.0, 5.0);
    }

    // -----------------------------------------------------------------------
    // SvdWeights — dicionários de mapeamento
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SvdWeights_UserIdToInner_DeveMapearCorretamente()
    {
        var svc = BuildService(ValidWeightsJson);
        await svc.StartAsync(CancellationToken.None);
        svc.SvdWeights.UserIdToInner.Should().ContainKey("10").WhoseValue.Should().Be(0);
    }

    [Fact]
    public async Task SvdWeights_InnerToItemId_DeveMapearCorretamente()
    {
        var svc = BuildService(ValidWeightsJson);
        await svc.StartAsync(CancellationToken.None);
        svc.SvdWeights.InnerToItemId["0"].Should().Be("ShowA");
        svc.SvdWeights.InnerToItemId["1"].Should().Be("ShowB");
    }

    // -----------------------------------------------------------------------
    // SvdWeights — vetores de pesos
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SvdWeights_Bu_DeveConterViesesDeUsuario()
    {
        var svc = BuildService(ValidWeightsJson);
        await svc.StartAsync(CancellationToken.None);
        svc.SvdWeights.Bu.Should().HaveCount(1);
        svc.SvdWeights.Bu[0].Should().BeApproximately(0.1, 1e-9);
    }

    [Fact]
    public async Task SvdWeights_Bi_DeveConterViesesDeItem()
    {
        var svc = BuildService(ValidWeightsJson);
        await svc.StartAsync(CancellationToken.None);
        svc.SvdWeights.Bi.Should().HaveCount(2);
        svc.SvdWeights.Bi[0].Should().BeApproximately(0.2, 1e-9);
        svc.SvdWeights.Bi[1].Should().BeApproximately(-0.1, 1e-9);
    }

    [Fact]
    public async Task SvdWeights_Pu_DeveConterFatoresLatentesDeUsuario()
    {
        var svc = BuildService(ValidWeightsJson);
        await svc.StartAsync(CancellationToken.None);
        svc.SvdWeights.Pu.Should().HaveCount(1);
        svc.SvdWeights.Pu[0].Should().HaveCount(2);
        svc.SvdWeights.Pu[0][0].Should().BeApproximately(0.5, 1e-9);
    }

    [Fact]
    public async Task SvdWeights_Qi_DeveConterFatoresLatentesDeItem()
    {
        var svc = BuildService(ValidWeightsJson);
        await svc.StartAsync(CancellationToken.None);
        svc.SvdWeights.Qi.Should().HaveCount(2);
        svc.SvdWeights.Qi[0].Should().Equal(0.4, 0.2);
        svc.SvdWeights.Qi[1].Should().Equal(0.1, 0.6);
    }

    // -----------------------------------------------------------------------
    // SvdWeights — antes de StartAsync (deve lançar)
    // -----------------------------------------------------------------------

    [Fact]
    public void SvdWeights_AntesDeStartAsync_DeveLancarInvalidOperationException()
    {
        var svc = BuildService(ValidWeightsJson);
        var act = () => _ = svc.SvdWeights;
        act.Should().Throw<InvalidOperationException>();
    }

    // -----------------------------------------------------------------------
    // Arquivo inválido
    // -----------------------------------------------------------------------

    [Fact]
    public async Task StartAsync_ComJsonInvalido_DeveLancarExcecao()
    {
        var svc = BuildService("{ json invalido }");
        await svc.Invoking(s => s.StartAsync(CancellationToken.None))
            .Should().ThrowAsync<Exception>();
    }

    [Fact]
    public void Construtor_ComArquivoInexistente_DeveLancarFileNotFoundException()
    {
        var opts = Options.Create(new ModelPathsOptions
        {
            SvdWeightsJson = "/caminho/que/nao/existe/pesos.json",
            DatasetCsv = "unused"
        });

        var act = () => new ModelLoaderService(opts, NullLogger<ModelLoaderService>.Instance);

        act.Should().Throw<FileNotFoundException>();
    }

    // -----------------------------------------------------------------------
    // Cleanup
    // -----------------------------------------------------------------------

    public void Dispose()
    {
        foreach (var f in _tempFiles)
            if (File.Exists(f)) File.Delete(f);
    }
}