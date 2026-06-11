using AventuraML;
using Xunit;

namespace InferenceService.Tests;

/// <summary>
/// Treina um modelo híbrido pequeno e real (via Trainer de src/ml) em um diretório
/// temporário, uma única vez por execução da suíte. Os testes do híbrido exercitam
/// o caminho completo: model.zip + JSONs + dataset.csv → Recommender.
///
/// Popularidade desenhada para ser determinística:
///   ItemA = 4 interações &gt; ItemB = 3 &gt; ItemC = 2 &gt; ItemD = 1.
/// Usuário 1 consumiu {ItemA, ItemB}; usuário 999 não existe (cold-start).
/// </summary>
public sealed class HybridArtifactsFixture : IDisposable
{
    public string Dir { get; }

    public string DatasetCsv => Path.Combine(Dir, "dataset.csv");
    public string ModelZip => Path.Combine(Dir, "model.zip");
    public string ColdStartJson => Path.Combine(Dir, "cold_start.json");
    public string ItemCategoriesJson => Path.Combine(Dir, "item_categories.json");
    public string CategoryAffinityJson => Path.Combine(Dir, "category_affinity.json");
    public string ColdStartByCategoryJson => Path.Combine(Dir, "cold_start_by_category.json");

    private const string DatasetContent = """
        user_id,item_id,ranking,valor_medio_ingresso,tipo_evento
        1,ItemA,5,50.0,Teatro
        1,ItemB,4,80.0,Musical
        2,ItemA,3,50.0,Teatro
        2,ItemC,5,30.0,Danca
        3,ItemA,4,50.0,Teatro
        3,ItemB,2,80.0,Musical
        4,ItemA,1,50.0,Teatro
        4,ItemB,5,80.0,Musical
        4,ItemC,2,30.0,Danca
        5,ItemD,3,100.0,Opera
        """;

    public HybridArtifactsFixture()
    {
        Dir = Path.Combine(Path.GetTempPath(), "aventura-hybrid-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Dir);
        File.WriteAllText(DatasetCsv, DatasetContent);

        Trainer.Train(
            DatasetCsv,
            ModelZip,
            ColdStartJson,
            ItemCategoriesJson,
            CategoryAffinityJson,
            ColdStartByCategoryJson);
    }

    public ModelPathsOptions BuildPaths() => new()
    {
        ModelZip = ModelZip,
        ColdStartJson = ColdStartJson,
        ItemCategoriesJson = ItemCategoriesJson,
        CategoryAffinityJson = CategoryAffinityJson,
        DatasetCsv = DatasetCsv,
    };

    public Recommender BuildRecommender(float alpha = 0.7f) => new(
        ModelZip, ColdStartJson, DatasetCsv, ItemCategoriesJson, CategoryAffinityJson, alpha);

    public void Dispose()
    {
        if (Directory.Exists(Dir))
            Directory.Delete(Dir, recursive: true);
    }
}

[CollectionDefinition("hybrid-artifacts")]
public sealed class HybridArtifactsCollection : ICollectionFixture<HybridArtifactsFixture>;
