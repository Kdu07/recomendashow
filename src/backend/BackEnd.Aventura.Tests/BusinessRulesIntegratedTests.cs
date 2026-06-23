using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class BusinessRulesIntegratedTests
    {
        private const string StrongJwtKey = "super-secret-jwt-key-for-business-rules-tests-123";

        [Fact]
        [Trait("Category", "BusinessRules")]
        public void TRN01_Register_WithConsent_ShouldCreateUserNormalizeEmailAndCreateDefaultPreferences()
        {
            var usuarios = new FakeUsuarioRepository();
            var preferencias = new FakePreferenciasRepository();
            var service = CreateAuthService(usuarios, preferencias);

            var response = service.Register(new RegisterRequest
            {
                Nome = "  Ana Cliente  ",
                Email = "ANA.CLIENTE@EMAIL.COM ",
                Senha = "senha123",
                Idade = 29,
                Cep = "01310-100",
                Logradouro = "Av. Paulista",
                Numero = "1000",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                ConsentimentoAceito = true
            });

            var usuario = usuarios.GetById(response.UsuarioId)!;
            var prefs = preferencias.GetPreferencias(response.UsuarioId);

            Assert.Equal("Ana Cliente", usuario.Nome);
            Assert.Equal("ana.cliente@email.com", usuario.Email);
            Assert.True(usuario.ConsentimentoAceito);
            Assert.NotNull(usuario.DataConsentimento);
            Assert.Equal("São Paulo", usuario.Cidade);
            Assert.Equal(response.UsuarioId, prefs.UsuarioId);
            Assert.True(prefs.ReceberNotificacoes);
        }

        [Fact]
        [Trait("Category", "BusinessRules")]
        public void TRN02_Register_WithoutConsent_ShouldBlockPersonalDataProcessing()
        {
            var service = CreateAuthService(new FakeUsuarioRepository(), new FakePreferenciasRepository());

            var ex = Assert.Throws<ArgumentException>(() => service.Register(new RegisterRequest
            {
                Nome = "Ana Cliente",
                Email = "ana@email.com",
                Senha = "senha123",
                Idade = 29,
                ConsentimentoAceito = false
            }));

            Assert.Contains("consentimento", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("Category", "BusinessRules")]
        public void TRN03_ProfileUpdate_ShouldRejectDuplicatedEmailAndUnderageBirthDate()
        {
            var usuarios = new FakeUsuarioRepository();
            var service = new UsuarioService(usuarios);
            var userA = usuarios.Create(Usuario.Create("Ana", "ana@email.com", "hash", 25));
            usuarios.Create(Usuario.Create("Beto", "beto@email.com", "hash", 31));

            Assert.Throws<ConflictException>(() => service.Update(userA.Id, new UsuarioUpdateRequest
            {
                Email = "beto@email.com"
            }));

            Assert.Throws<ArgumentException>(() => service.Update(userA.Id, new UsuarioUpdateRequest
            {
                DataNascimento = DateTime.UtcNow.Date.AddYears(-12)
            }));
        }

        [Fact]
        [Trait("Category", "BusinessRules")]
        public void TRN04_PreferencesUpdate_ShouldDeduplicateTrimAndRejectInvalidPriceRange()
        {
            var repository = new FakePreferenciasRepository();
            var service = new PreferenciasService(repository);
            var usuarioId = Guid.NewGuid();

            var updated = service.Update(usuarioId, new PreferenciasUpdateDto
            {
                Generos = new[] { " Musical ", "musical", "Drama", " " },
                Tags = new[] { " Familia ", "familia", "Noite" },
                Cep = " 01310-100 ",
                Cidade = " São Paulo ",
                FaixaPrecoMin = 50,
                FaixaPrecoMax = 300,
                ReceberNotificacoes = false
            });

            Assert.Equal(new[] { "Musical", "Drama" }, updated.Generos);
            Assert.Equal(new[] { "Familia", "Noite" }, updated.Tags);
            Assert.Equal("01310-100", updated.Cep);
            Assert.Equal("São Paulo", updated.Cidade);
            Assert.False(updated.ReceberNotificacoes);
            Assert.Throws<ArgumentException>(() => service.Update(usuarioId, new PreferenciasUpdateDto
            {
                FaixaPrecoMin = 301,
                FaixaPrecoMax = 300
            }));
        }

        [Fact]
        [Trait("Category", "BusinessRules")]
        public void TRN05_Wishlist_ShouldRequireExistingShowPreventDuplicatesAndRemoveOnlySavedItems()
        {
            var wishlist = new FakeWishlistRepository();
            var espetaculos = new FakeEspetaculoRepository();
            var service = new WishlistService(wishlist, espetaculos);
            var usuarioId = Guid.NewGuid();
            var espetaculo = CreateEspetaculo("A Noviça Rebelde", "Musical");

            Assert.Throws<NotFoundException>(() => service.Add(usuarioId, espetaculo.Id));

            espetaculos.AddEspetaculo(espetaculo);
            service.Add(usuarioId, espetaculo.Id);

            Assert.True(service.Contains(usuarioId, espetaculo.Id));
            Assert.Throws<ConflictException>(() => service.Add(usuarioId, espetaculo.Id));

            service.Remove(usuarioId, espetaculo.Id);

            Assert.False(service.Contains(usuarioId, espetaculo.Id));
            Assert.Throws<NotFoundException>(() => service.Remove(usuarioId, espetaculo.Id));
        }

        [Fact]
        [Trait("Category", "BusinessRules")]
        public void TRN06_Recommendations_ShouldPreferStoredResultsRespectLimitAndFallbackToFavoriteGenres()
        {
            var recomendacoes = new FakeRecomendacaoRepository();
            var espetaculos = new FakeEspetaculoRepository();
            var wishlist = new FakeWishlistRepository();
            var service = new RecomendacaoService(
                new IRecommendationStrategy[]
                {
                    new StoredRecommendationStrategy(recomendacoes),
                    new FavoriteGenresRecommendationStrategy(espetaculos)
                },
                wishlist,
                new WishlistRecommendationPostProcessor());
            var usuarioId = Guid.NewGuid();
            var stored = CreateEspetaculo("Musical Armazenado", "Musical");
            var fallback = CreateEspetaculo("Musical Fallback", "Musical");

            recomendacoes.AddRecomendacao(usuarioId, new Recomendacao(stored, 0.95m, "Histórico do usuário"));
            espetaculos.AddEspetaculoParaUsuario(usuarioId, fallback);

            var storedResult = service.GetRecomendacoes(usuarioId, 10);

            Assert.Single(storedResult);
            Assert.Equal(stored.Id, storedResult[0].Espetaculo.Id);
            Assert.Equal("Histórico do usuário", storedResult[0].Motivo);
            Assert.Throws<ArgumentException>(() => service.GetRecomendacoes(usuarioId, 0));
            Assert.Throws<ArgumentException>(() => service.GetRecomendacoes(usuarioId, 51));

            var usuarioSemRecomendacao = Guid.NewGuid();
            espetaculos.AddEspetaculoParaUsuario(usuarioSemRecomendacao, fallback);

            var fallbackResult = service.GetRecomendacoes(usuarioSemRecomendacao, 5);

            Assert.Single(fallbackResult);
            Assert.Equal(fallback.Id, fallbackResult[0].Espetaculo.Id);
            Assert.Contains("gêneros favoritos", fallbackResult[0].Motivo);
        }

        [Fact]
        [Trait("Category", "BusinessRules")]
        public void TRN07_ShowCatalog_ShouldFilterGenreCaseInsensitiveAndExposePurchaseUrl()
        {
            var musical = CreateEspetaculo("A Noviça Rebelde", "Musical", "https://tickets.example/musical");
            var drama = CreateEspetaculo("Hamlet", "Drama", "https://tickets.example/drama");
            var repository = new FakeEspetaculoRepository();
            repository.AddEspetaculo(musical);
            repository.AddEspetaculo(drama);
            var service = new EspetaculoService(repository);

            var result = service.Search("musical", null);

            Assert.Single(result);
            Assert.Equal(musical.Id, result[0].Id);
            Assert.Equal("https://tickets.example/musical", musical.ObterUrlCompra());
            Assert.True(musical.EstaEmCartaz());
        }

        [Fact]
        [Trait("Category", "BusinessRules")]
        public void TRN08_Theater_ShouldRejectDuplicatedShowInSameTheater()
        {
            var teatro = new Teatro(Guid.NewGuid(), "Vibra São Paulo", "Av. das Nações Unidas", "São Paulo", "10h às 22h");
            var espetaculo = CreateEspetaculo("A Noviça Rebelde", "Musical");

            teatro.AdicionarEspetaculo(espetaculo);

            var ex = Assert.Throws<InvalidOperationException>(() => teatro.AdicionarEspetaculo(espetaculo));
            Assert.Contains("já cadastrado", ex.Message);
        }

        [Fact]
        [Trait("Category", "BusinessRules")]
        public void TRN09_DeletedUser_ShouldNotBeAbleToLogin()
        {
            var usuarios = new FakeUsuarioRepository();
            var preferencias = new FakePreferenciasRepository();
            var service = CreateAuthService(usuarios, preferencias);
            var registered = service.Register(new RegisterRequest
            {
                Nome = "Usuário Removido",
                Email = "removido@email.com",
                Senha = "senha123",
                Idade = 40,
                ConsentimentoAceito = true
            });
            usuarios.SolicitarExclusao(registered.UsuarioId);

            var ex = Assert.Throws<UnauthorizedAccessException>(() => service.Login(new LoginRequest
            {
                Email = "removido@email.com",
                Senha = "senha123"
            }));

            Assert.Contains("encerrada", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("Category", "BusinessRules")]
        public void TRN10_DeletionRequest_ShouldSetDeletedAtAndBlockLogin()
        {
            // A anonimização em si é responsabilidade de sp_anonimizar_usuario (testada em testes de banco).
            // Aqui validamos que SolicitarExclusao marca DeletedAt e que o usuário não consegue logar após isso.
            var usuarios = new FakeUsuarioRepository();
            var preferencias = new FakePreferenciasRepository();
            var service = CreateAuthService(usuarios, preferencias);

            var registered = service.Register(new RegisterRequest
            {
                Nome = "Cliente LGPD",
                Email = "lgpd@email.com",
                Senha = "senha123",
                Idade = 32,
                Cep = "01310-100",
                Logradouro = "Av. Paulista",
                Numero = "1000",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                ConsentimentoAceito = true
            });

            var antes = usuarios.GetById(registered.UsuarioId)!;
            Assert.Null(antes.DeletedAt);

            usuarios.SolicitarExclusao(registered.UsuarioId);

            var depois = usuarios.GetById(registered.UsuarioId)!;
            Assert.NotNull(depois.DeletedAt);

            Assert.Throws<UnauthorizedAccessException>(() => service.Login(new LoginRequest
            {
                Email = "lgpd@email.com",
                Senha = "senha123"
            }));
        }

        private static AuthService CreateAuthService(
            FakeUsuarioRepository usuarios,
            FakePreferenciasRepository preferencias)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT_KEY"] = StrongJwtKey,
                    ["Jwt:Issuer"] = "business-rules-tests",
                    ["Jwt:Audience"] = "business-rules-tests",
                    ["Jwt:ExpiryMinutes"] = "60"
                })
                .Build();

            return new AuthService(usuarios, preferencias, configuration);
        }

        private static Espetaculo CreateEspetaculo(
            string nome,
            string genero,
            string urlBilheteria = "https://tickets.example/show") =>
            new(
                Guid.NewGuid(),
                nome,
                genero,
                "Teatro",
                $"Descrição de {nome}",
                urlBilheteria,
                DateTime.UtcNow.Date.AddDays(-1),
                DateTime.UtcNow.Date.AddDays(30),
                120);
    }
}
