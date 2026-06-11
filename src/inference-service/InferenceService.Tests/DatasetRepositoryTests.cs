using FluentAssertions;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using InferenceService.Data;

namespace InferenceService.Tests;

/// <summary>
/// Testes unitários para DatasetRepository.
/// Cobre: construção das estruturas de lookup, lista de popularidade
/// e os métodos públicos de consulta.
/// 
/// Estratégia: o DatasetRepository lê um arquivo CSV em disco.
/// Cada teste cria um CSV temporário controlado e passa o caminho via IOptions.
/// </summary>
public class DatasetRepositoryTests : IDisposable
{
    private readonly List<string> _tempFiles = [];

    // -----------------------------------------------------------------------
    // Helper: cria CSV temporário e retorna o DatasetRepository
    // -----------------------------------------------------------------------

    private DatasetRepository BuildFromCsv(string csvContent)
    {
        var path = Path.GetTempFileName();
        _tempFiles.Add(path);
        File.WriteAllText(path, csvContent);

        var opts = Options.Create(new ModelPathsOptions
        {
            DatasetCsv = path,
        });

        return new DatasetRepository(opts, NullLogger<DatasetRepository>.Instance);
    }

    /// CSV mínimo com 3 usuários e 4 itens para a maioria dos testes.
    private const string BaseCsv = """
        user_id,item_id,ranking,valor_medio_ingresso,tipo_evento
        1,ItemA,5,50.0,Teatro
        1,ItemB,4,80.0,Musical
        2,ItemA,3,50.0,Teatro
        2,ItemC,5,30.0,Dança
        3,ItemD,2,100.0,Opera
        3,ItemA,4,50.0,Teatro
        """;

    // -----------------------------------------------------------------------
    // AllItems
    // -----------------------------------------------------------------------

    [Fact]
    public void AllItems_DeveConterTodosOsItensUnicos()
    {
        var repo = BuildFromCsv(BaseCsv);
        repo.AllItems.Should().BeEquivalentTo(["ItemA", "ItemB", "ItemC", "ItemD"]);
    }

    // -----------------------------------------------------------------------
    // ColdStartItems — popularidade
    // -----------------------------------------------------------------------

    [Fact]
    public void ColdStartItems_PrimeiroItemDeveSerOMaisPopular()
    {
        // ItemA aparece 3 vezes → mais popular
        var repo = BuildFromCsv(BaseCsv);
        repo.ColdStartItems[0].Should().Be("ItemA");
    }

    [Fact]
    public void ColdStartItems_DeveConterTodosOsItens()
    {
        var repo = BuildFromCsv(BaseCsv);
        repo.ColdStartItems.Should().BeEquivalentTo(["ItemA", "ItemB", "ItemC", "ItemD"]);
    }

    [Fact]
    public void ColdStartItems_DeveEstarOrdenadoPorContagemDecrescente()
    {
        // ItemA=3, ItemB=1, ItemC=1, ItemD=1
        var repo = BuildFromCsv(BaseCsv);

        // O primeiro deve ser ItemA (maior contagem)
        // Os demais com contagem 1 podem vir em qualquer ordem
        repo.ColdStartItems.First().Should().Be("ItemA");
    }

    // -----------------------------------------------------------------------
    // GetConsumedItems
    // -----------------------------------------------------------------------

    [Fact]
    public void GetConsumedItems_DeveRetornarItensDoUsuario()
    {
        var repo = BuildFromCsv(BaseCsv);
        var consumed = repo.GetConsumedItems(userId: 1);
        consumed.Should().BeEquivalentTo(["ItemA", "ItemB"]);
    }

    [Fact]
    public void GetConsumedItems_DeveRetornarVazio_ParaUsuarioDesconhecido()
    {
        var repo = BuildFromCsv(BaseCsv);
        var consumed = repo.GetConsumedItems(userId: 999);
        consumed.Should().BeEmpty();
    }

    // -----------------------------------------------------------------------
    // GetInteractions
    // -----------------------------------------------------------------------

    [Fact]
    public void GetInteractions_DeveRetornarTodasAsInteracoesDoUsuario()
    {
        var repo = BuildFromCsv(BaseCsv);
        var interactions = repo.GetInteractions(userId: 1);
        interactions.Should().HaveCount(2);
        interactions.Select(i => i.ItemId).Should().BeEquivalentTo(["ItemA", "ItemB"]);
    }

    [Fact]
    public void GetInteractions_DeveRetornarVazio_ParaUsuarioDesconhecido()
    {
        var repo = BuildFromCsv(BaseCsv);
        var interactions = repo.GetInteractions(userId: 999);
        interactions.Should().BeEmpty();
    }

    // -----------------------------------------------------------------------
    // GetRating
    // -----------------------------------------------------------------------

    [Fact]
    public void GetRating_DeveRetornarRanking_QuandoInteracaoExiste()
    {
        var repo = BuildFromCsv(BaseCsv);
        var rating = repo.GetRating(userId: 1, itemId: "ItemA");
        rating.Should().Be(5);
    }

    [Fact]
    public void GetRating_DeveRetornarNull_QuandoInteracaoNaoExiste()
    {
        var repo = BuildFromCsv(BaseCsv);
        var rating = repo.GetRating(userId: 1, itemId: "ItemD");
        rating.Should().BeNull();
    }

    [Fact]
    public void GetRating_DeveRetornarNull_ParaUsuarioDesconhecido()
    {
        var repo = BuildFromCsv(BaseCsv);
        var rating = repo.GetRating(userId: 999, itemId: "ItemA");
        rating.Should().BeNull();
    }

    // -----------------------------------------------------------------------
    // UserExists
    // -----------------------------------------------------------------------

    [Fact]
    public void UserExists_DeveRetornarTrue_ParaUsuarioPresenteNoCsv()
    {
        var repo = BuildFromCsv(BaseCsv);
        repo.UserExists(1).Should().BeTrue();
    }

    [Fact]
    public void UserExists_DeveRetornarFalse_ParaUsuarioAusente()
    {
        var repo = BuildFromCsv(BaseCsv);
        repo.UserExists(999).Should().BeFalse();
    }

    // -----------------------------------------------------------------------
    // AllInteractions
    // -----------------------------------------------------------------------

    [Fact]
    public void AllInteractions_DeveConterTodasAsLinhasDoCSV()
    {
        var repo = BuildFromCsv(BaseCsv);
        repo.AllInteractions.Should().HaveCount(6);
    }

    // -----------------------------------------------------------------------
    // Normalização (Trim) — paridade com DataPipeline de src/ml
    // -----------------------------------------------------------------------

    [Fact]
    public void ItemIdETipoEvento_ComEspacosNasPontas_SaoNormalizadosComTrim()
    {
        const string csv = """
            user_id,item_id,ranking,valor_medio_ingresso,tipo_evento
            1,"  ShowA  ",5,50.0,"  Teatro "
            2,ShowB,3,80.0,Musical
            """;
        var repo = BuildFromCsv(csv);

        repo.AllItems.Should().BeEquivalentTo(["ShowA", "ShowB"]);
        repo.GetConsumedItems(1).Should().BeEquivalentTo(["ShowA"]);
        repo.GetRating(1, "ShowA").Should().Be(5);
        repo.GetInteractions(1).Should().ContainSingle()
            .Which.TipoEvento.Should().Be("Teatro");
    }

    [Fact]
    public void LinhasQueColidemAposOTrim_NaoDerrubamOCarregamento()
    {
        // "ShowA" e "ShowA " viram a mesma chave (user, item) após o Trim;
        // o repositório deve carregar e responder com o maior ranking do par.
        const string csv = """
            user_id,item_id,ranking,valor_medio_ingresso,tipo_evento
            1,ShowA,2,50.0,Teatro
            1,"ShowA ",4,50.0,Teatro
            """;
        var repo = BuildFromCsv(csv);

        repo.AllItems.Should().BeEquivalentTo(["ShowA"]);
        repo.GetRating(1, "ShowA").Should().Be(4);
    }

    // -----------------------------------------------------------------------
    // Edge cases
    // -----------------------------------------------------------------------

    [Fact]
    public void Repositorio_ComCsvApenasComCabecalho_DeveCarregarSemErros()
    {
        const string csv = "user_id,item_id,ranking,valor_medio_ingresso,tipo_evento\n";
        var repo = BuildFromCsv(csv);
        repo.AllItems.Should().BeEmpty();
        repo.ColdStartItems.Should().BeEmpty();
        repo.AllInteractions.Should().BeEmpty();
    }

    [Fact]
    public void Repositorio_ComUmUnicoUsuario_DeveCarregarCorretamente()
    {
        const string csv = """
            user_id,item_id,ranking,valor_medio_ingresso,tipo_evento
            42,ShowX,5,100.0,Teatro
            """;
        var repo = BuildFromCsv(csv);
        repo.UserExists(42).Should().BeTrue();
        repo.GetConsumedItems(42).Should().Contain("ShowX");
        repo.ColdStartItems.Should().ContainSingle().Which.Should().Be("ShowX");
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