using AventuraML;
using AventuraML.Models;
using FluentAssertions;
using Xunit;

namespace InferenceService.Tests;

/// <summary>
/// Testes unitários para o Preprocessor (raw Sympla → dataset.csv).
/// Cobre as regras do pipeline original: filtro de lead time (0–180 dias),
/// dedupe por (usuário, espetáculo, dia de compra), ranking limitado a 5,
/// parse de preço com vírgula decimal e remoção de usuários outliers (P99.9).
///
/// Estratégia: cada teste escreve um raw CSV temporário com apenas as colunas
/// mapeadas por SymplaRawRecord (MissingFieldFound = null permite omitir as demais)
/// e roda o Preprocessor de ponta a ponta.
/// </summary>
public class PreprocessorTests : IDisposable
{
    private readonly List<string> _tempDirs = [];

    private const string Header =
        "ID COMPRADOR.1.ID USUÁRIO,NOME ESPETÁCULO,DATA DA APRESENTAÇÃO,DATA DA COMPRA,VALOR DO INGRESSO,TIPO";

    // -----------------------------------------------------------------------
    // Helper: roda o Preprocessor sobre linhas de raw e retorna o resultado
    // -----------------------------------------------------------------------

    private (IReadOnlyList<ProcessedRecord> Records, string OutputPath) Run(params string[] rawLines)
    {
        var dir = Path.Combine(Path.GetTempPath(), "aventura-preproc-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        _tempDirs.Add(dir);

        var rawPath = Path.Combine(dir, "raw.csv");
        var outPath = Path.Combine(dir, "dataset.csv");
        File.WriteAllLines(rawPath, [Header, .. rawLines]);

        var records = Preprocessor.Preprocess(rawPath, outPath);
        return (records, outPath);
    }

    // -----------------------------------------------------------------------
    // Dedupe por (usuário, espetáculo, dia)
    // -----------------------------------------------------------------------

    [Fact]
    public void ComprasNoMesmoDia_ContamComoUmaInteracao_EUsamApenasAPrimeiraLinha()
    {
        var (records, _) = Run(
            "1,ShowA,6/26/21 20:00,5/20/21 5:03,100,Teatro",
            "1,ShowA,6/26/21 20:00,5/20/21 9:30,50,Teatro");

        var rec = records.Should().ContainSingle().Subject;
        rec.Ranking.Should().Be(1);
        // dedupe mantém a primeira linha do dia: média sobre [100], não [100, 50]
        rec.ValorMedioIngresso.Should().Be(100.0);
    }

    [Fact]
    public void ComprasEmDiasDistintos_IncrementamORanking()
    {
        var (records, _) = Run(
            "1,ShowA,6/26/21 20:00,5/20/21 5:03,100,Teatro",
            "1,ShowA,6/26/21 20:00,5/21/21 5:03,50,Teatro");

        var rec = records.Should().ContainSingle().Subject;
        rec.Ranking.Should().Be(2);
        rec.ValorMedioIngresso.Should().Be(75.0);
    }

    // -----------------------------------------------------------------------
    // Cap do ranking em 5
    // -----------------------------------------------------------------------

    [Fact]
    public void Ranking_LimitadoA5_MesmoComMaisDiasDistintos()
    {
        var lines = Enumerable.Range(1, 7)
            .Select(d => $"1,ShowA,6/26/21 20:00,5/{d}/21 5:03,100,Teatro")
            .ToArray();

        var (records, _) = Run(lines);

        records.Should().ContainSingle().Which.Ranking.Should().Be(5);
    }

    // -----------------------------------------------------------------------
    // Filtro de lead time (0–180 dias)
    // -----------------------------------------------------------------------

    [Fact]
    public void CompraDepoisDaApresentacao_EDescartada()
    {
        var (records, _) = Run(
            "1,ShowA,6/26/21 20:00,7/1/21 5:03,100,Teatro",
            "2,ShowB,6/26/21 20:00,6/1/21 5:03,80,Musical");

        var rec = records.Should().ContainSingle().Subject;
        rec.UserId.Should().Be(2);
    }

    [Fact]
    public void CompraComMaisDe180DiasDeAntecedencia_EDescartada()
    {
        var (records, _) = Run(
            "1,ShowA,12/30/21 20:00,1/1/21 5:03,100,Teatro",
            "2,ShowB,6/26/21 20:00,6/1/21 5:03,80,Musical");

        var rec = records.Should().ContainSingle().Subject;
        rec.UserId.Should().Be(2);
    }

    [Fact]
    public void DataInvalida_DescartaALinha()
    {
        var (records, _) = Run(
            "1,ShowA,nao-e-data,5/20/21 5:03,100,Teatro",
            "2,ShowB,6/26/21 20:00,6/1/21 5:03,80,Musical");

        var rec = records.Should().ContainSingle().Subject;
        rec.UserId.Should().Be(2);
    }

    // -----------------------------------------------------------------------
    // Parse de preço
    // -----------------------------------------------------------------------

    [Fact]
    public void PrecoComVirgulaDecimal_EConvertidoCorretamente()
    {
        var (records, _) = Run("1,ShowA,6/26/21 20:00,5/20/21 5:03,\"19,5\",Teatro");

        records.Should().ContainSingle().Which.ValorMedioIngresso.Should().Be(19.5);
    }

    [Fact]
    public void PrecoNaoNumerico_ViraZero()
    {
        var (records, _) = Run("1,ShowA,6/26/21 20:00,5/20/21 5:03,gratis,Teatro");

        records.Should().ContainSingle().Which.ValorMedioIngresso.Should().Be(0.0);
    }

    // -----------------------------------------------------------------------
    // Remoção de outliers (P99.9 do volume de interações por usuário)
    // -----------------------------------------------------------------------

    [Fact]
    public void UsuarioAcimaDoPercentil999_ERemovido()
    {
        // 10 usuários com 1 interação e 1 usuário com 100 shows distintos:
        // threshold interpolado ≈ 99, então só o usuário 999 é removido.
        var lines = new List<string>();
        for (var u = 1; u <= 10; u++)
            lines.Add($"{u},ShowA,6/26/21 20:00,5/20/21 5:03,100,Teatro");
        for (var s = 1; s <= 100; s++)
            lines.Add($"999,Show{s},6/26/21 20:00,5/20/21 5:03,100,Teatro");

        var (records, _) = Run([.. lines]);

        records.Should().HaveCount(10);
        records.Should().NotContain(r => r.UserId == 999);
    }

    // -----------------------------------------------------------------------
    // Saída consumível pelo restante do pipeline
    // -----------------------------------------------------------------------

    [Fact]
    public void SaidaGerada_ELegivelPeloDataPipeline()
    {
        var (records, outPath) = Run(
            "1,ShowA,6/26/21 20:00,5/20/21 5:03,100,Teatro",
            "2,ShowB,6/26/21 20:00,6/1/21 5:03,\"19,5\",Musical");

        var rows = DataPipeline.LoadRaw(outPath);

        rows.Should().HaveCount(records.Count);
        rows.Select(r => (r.UserId, r.ItemId)).Should()
            .BeEquivalentTo(records.Select(r => (r.UserId, r.ItemId)));
    }

    public void Dispose()
    {
        foreach (var dir in _tempDirs.Where(Directory.Exists))
            Directory.Delete(dir, recursive: true);
    }
}
