using AventuraML;

namespace InferenceService.Services.Interfaces;

public interface IHybridModelLoader
{
    /// <summary>
    /// Recomendador híbrido (src/ml) construído a partir dos artefatos
    /// (model.zip + cold_start/item_categories/category_affinity.json).
    /// Lança InvalidOperationException antes do carregamento.
    /// </summary>
    Recommender Recommender { get; }

    bool IsLoaded { get; }
}
