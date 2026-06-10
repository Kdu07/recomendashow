namespace BackEnd.Aventura.E2ETests.Database
{
    internal static class EspetaculoWishlistSeed
    {
        internal static readonly Guid UsuarioId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        internal static readonly Guid EspetaculoId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        internal static string BuildSchemaAndSeedSql() => $@"
DROP TABLE IF EXISTS wishlist_curtidas;
DROP TABLE IF EXISTS espetaculo_genero;
DROP TABLE IF EXISTS espetaculos;
DROP TABLE IF EXISTS generos;
DROP TABLE IF EXISTS usuarios;
DROP TYPE IF EXISTS tipo_lista;

CREATE TYPE tipo_lista AS ENUM ('curtida', 'wishlist');

CREATE TABLE usuarios(
    id UUID PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    senha_hash TEXT NOT NULL,
    idade INT CHECK(idade >= 13),
    data_cadastro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE generos(
    id UUID PRIMARY KEY,
    nome VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE espetaculos(
    id UUID PRIMARY KEY,
    nome VARCHAR(150) UNIQUE NOT NULL,
    tipo_evento VARCHAR(80),
    descricao TEXT,
    url_bilheteria TEXT,
    data_inicio TIMESTAMP NOT NULL,
    data_fim TIMESTAMP NOT NULL,
    valor_ingresso DECIMAL(10,2) CHECK(valor_ingresso >= 0),
    CONSTRAINT chk_datas CHECK(data_fim > data_inicio)
);

CREATE TABLE espetaculo_genero(
    espetaculo_id UUID REFERENCES espetaculos(id) ON DELETE CASCADE,
    genero_id UUID REFERENCES generos(id) ON DELETE CASCADE,
    PRIMARY KEY(espetaculo_id, genero_id)
);

CREATE TABLE wishlist_curtidas(
    usuario_id UUID REFERENCES usuarios(id) ON DELETE CASCADE,
    espetaculo_id UUID REFERENCES espetaculos(id) ON DELETE CASCADE,
    tipo tipo_lista,
    PRIMARY KEY(usuario_id, espetaculo_id, tipo)
);

INSERT INTO usuarios(id, nome, email, senha_hash, idade)
VALUES ('{UsuarioId}', 'Ana Teste', 'ana.e2e@example.com', 'hash', 28);

INSERT INTO generos(id, nome)
VALUES ('33333333-3333-3333-3333-333333333333', 'Musical');

INSERT INTO espetaculos(id, nome, tipo_evento, descricao, url_bilheteria, data_inicio, data_fim, valor_ingresso)
VALUES (
    '{EspetaculoId}',
    'Mamma Mia!',
    'Espetaculo Musical',
    'Musical em cartaz para validacao E2E.',
    'https://ingressos.example/mamma-mia',
    NOW() - INTERVAL '1 day',
    NOW() + INTERVAL '30 days',
    120.00
);

INSERT INTO espetaculo_genero(espetaculo_id, genero_id)
VALUES ('{EspetaculoId}', '33333333-3333-3333-3333-333333333333');
";
    }
}
