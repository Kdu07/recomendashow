using BackEnd.Aventura.Infrastructure;
using BackEnd.Aventura.Infrastructure.Middleware;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;

namespace BackEnd.Aventura.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAventuraInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<IMongoConnectionFactory, MongoConnectionFactory>();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddScoped<IEspetaculoRepository, EspetaculoRepository>();
            services.AddScoped<IEspetaculoImagemRepository, EspetaculoImagemRepository>();
            services.AddScoped<ITeatroRepository, TeatroRepository>();
            services.AddScoped<IPreferenciasRepository, PreferenciasRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IRecomendacaoRepository, RecomendacaoRepository>();
            services.AddScoped<IGeneroRepository, GeneroRepository>();

            return services;
        }

        public static IServiceCollection AddAventuraApplication(this IServiceCollection services)
        {
            services.AddScoped<IEspetaculoService, EspetaculoService>();
            services.AddScoped<IEspetaculoImagemService, EspetaculoImagemService>();
            services.AddScoped<ITeatroService, TeatroService>();
            services.AddScoped<IPreferenciasService, PreferenciasService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRecommendationStrategy, FavoriteGenresRecommendationStrategy>();
            services.AddScoped<IRecommendationStrategy, WishlistGenresRecommendationStrategy>();
            services.AddScoped<IRecommendationStrategy, StoredRecommendationStrategy>();
            services.AddSingleton<IWishlistRecommendationPostProcessor, WishlistRecommendationPostProcessor>();
            services.AddScoped<IRecomendacaoService, RecomendacaoService>();
            services.AddScoped<IGeneroService, GeneroService>();

            services.AddHostedService<AnonimizacaoJob>();

            return services;
        }
    }
}