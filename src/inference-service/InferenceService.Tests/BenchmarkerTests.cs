using AventuraML;
using FluentAssertions;
using Xunit;

namespace InferenceService.Tests;

/// <summary>
/// Testes do Benchmarker: roda o benchmark de latência sobre o modelo híbrido
/// real treinado pela fixture, com poucas requisições, e valida a saída impressa
/// no console (startup, percentis, throughput). Também cobre o caminho de erro
/// quando um artefato do modelo não existe.
/// </summary>
[Collection("hybrid-artifacts")]
public class BenchmarkerTests
{
    private readonly HybridArtifactsFixture _fixture;

    public BenchmarkerTests(HybridArtifactsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Run_ComArtefatosValidos_ImprimeMetricasDeLatencia()
    {
        var original = Console.Out;
        using var captured = new StringWriter();
        Console.SetOut(captured);

        try
        {
            // n pequeno para o teste ser rápido; topK acima do catálogo (4 itens) é tolerado.
            Benchmarker.Run(
                _fixture.ModelZip,
                _fixture.ColdStartJson,
                _fixture.DatasetCsv,
                _fixture.ItemCategoriesJson,
                _fixture.CategoryAffinityJson,
                n: 50,
                topK: 5);
        }
        finally
        {
            Console.SetOut(original);
        }

        var output = captured.ToString();
        output.Should().Contain("Startup");
        output.Should().Contain("Latência por request");
        output.Should().Contain("P50");
        output.Should().Contain("P95");
        output.Should().Contain("P99");
        output.Should().Contain("Throughput");
        output.Should().Contain("Tempo total");
    }

    [Fact]
    public void Run_ComModeloInexistente_DeveLancar()
    {
        var modeloInexistente = Path.Combine(_fixture.Dir, "nao_existe", "model.zip");

        var act = () => Benchmarker.Run(
            modeloInexistente,
            _fixture.ColdStartJson,
            _fixture.DatasetCsv,
            _fixture.ItemCategoriesJson,
            _fixture.CategoryAffinityJson,
            n: 10,
            topK: 5);

        act.Should().Throw<Exception>();
    }
}
