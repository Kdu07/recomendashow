using System;
using BackEnd.Aventura.Extensions;
using BackEnd.Aventura.Infrastructure;
using BackEnd.Aventura.Infrastructure.Middleware;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void AddAventuraInfrastructure_ShouldRegisterInfrastructureServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddAventuraInfrastructure();

            // Assert
            Assert.Contains(services, d => d.ServiceType == typeof(IDbConnectionFactory) && d.ImplementationType == typeof(DbConnectionFactory));
            Assert.Contains(services, d => d.ServiceType == typeof(IEspetaculoRepository) && d.ImplementationType == typeof(EspetaculoRepository));
            Assert.Contains(services, d => d.ServiceType == typeof(ITeatroRepository) && d.ImplementationType == typeof(TeatroRepository));
            Assert.Contains(services, d => d.ServiceType == typeof(IPreferenciasRepository) && d.ImplementationType == typeof(PreferenciasRepository));
            Assert.Contains(services, d => d.ServiceType == typeof(IWishlistRepository) && d.ImplementationType == typeof(WishlistRepository));
            Assert.Contains(services, d => d.ServiceType == typeof(IUsuarioRepository) && d.ImplementationType == typeof(UsuarioRepository));
            Assert.Contains(services, d => d.ServiceType == typeof(IGeneroRepository) && d.ImplementationType == typeof(GeneroRepository));
        }

        [Fact]
        public void AddAventuraApplication_ShouldRegisterApplicationServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddAventuraApplication();

            // Assert
            Assert.Contains(services, d => d.ServiceType == typeof(IEspetaculoService) && d.ImplementationType == typeof(EspetaculoService));
            Assert.Contains(services, d => d.ServiceType == typeof(ITeatroService) && d.ImplementationType == typeof(TeatroService));
            Assert.Contains(services, d => d.ServiceType == typeof(IPreferenciasService) && d.ImplementationType == typeof(PreferenciasService));
            Assert.Contains(services, d => d.ServiceType == typeof(IWishlistService) && d.ImplementationType == typeof(WishlistService));
            Assert.Contains(services, d => d.ServiceType == typeof(IUsuarioService) && d.ImplementationType == typeof(UsuarioService));
            Assert.Contains(services, d => d.ServiceType == typeof(IAuthService) && d.ImplementationType == typeof(AuthService));
            Assert.Contains(services, d => d.ServiceType == typeof(IGeneroService) && d.ImplementationType == typeof(GeneroService));
        }
    }
}
