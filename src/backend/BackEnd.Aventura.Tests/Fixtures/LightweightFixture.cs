using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace BackEnd.Aventura.Tests.Fixtures;

public sealed class LightweightFixture : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    public HttpClient Client { get; }

    public LightweightFixture()
    {
        var mockRecommendation = new Mock<IRecommendationService>();
        mockRecommendation
            .Setup(s => s.Recommend(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new List<RecommendationResult>());

        var mockPreference = new Mock<IPreferenceService>();
        mockPreference
            .Setup(s => s.GetPreferences(It.IsAny<int>()))
            .Returns(Array.Empty<string>());

        // Caminhos falsos: o RecommendationService real é substituído pelo mock e
        // nunca acessa esses arquivos.
        var fakePath = Path.Combine(Path.GetTempPath(), "fake.zip");

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:DefaultConnection",
                    "Host=localhost;Database=test;Username=test;Password=test");
                builder.UseSetting("ML:ModelPath",               fakePath);
                builder.UseSetting("ML:ColdStartPath",           fakePath);
                builder.UseSetting("ML:DatasetPath",             fakePath);
                builder.UseSetting("ML:ItemCategoriesPath",      fakePath);
                builder.UseSetting("ML:CategoryAffinityPath",    fakePath);
                builder.UseSetting("ML:ColdStartByCategoryPath", fakePath);

                builder.ConfigureServices(services =>
                {
                    services.Replace(ServiceDescriptor.Singleton<IRecommendationService>(
                        mockRecommendation.Object));
                    services.Replace(ServiceDescriptor.Singleton<IPreferenceService>(
                        mockPreference.Object));
                });
            });

        Client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();
}
