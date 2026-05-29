using RecommendationService.Models;

namespace RecommendationService.Services.Interfaces;

public interface IModelLoaderService
{
    SvdWeights SvdWeights { get; }
    bool IsLoaded { get; }
}
