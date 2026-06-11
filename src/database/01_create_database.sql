CREATE EXTENSION IF NOT EXISTS "pgcrypto";

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_type
        WHERE typname='tipo_interacao'
    ) THEN

        CREATE TYPE tipo_interacao AS ENUM (
            'Clique',
            'Acesso',
            'TempoDeTela'
        );

    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_type
        WHERE typname='tipo_lista'
    ) THEN

        CREATE TYPE tipo_lista AS ENUM (
            'curtida',
            'wishlist'
        );

    END IF;
END $$;

CREATE TABLE IF NOT EXISTS usuarios(

    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    nome VARCHAR(100) NOT NULL,

    email VARCHAR(255)
    NOT NULL
    UNIQUE,

    senha_hash TEXT NOT NULL,

    idade INT
    CHECK(idade >=13),

    data_nascimento DATE,

    data_cadastro TIMESTAMP
    DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_usuario_email
ON usuarios(email);


CREATE TABLE IF NOT EXISTS teatros(

    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    nome VARCHAR(150) UNIQUE NOT NULL,

    endereco VARCHAR(255),

    cidade VARCHAR(100),

    horario_funcionamento VARCHAR(100)
);


CREATE TABLE IF NOT EXISTS espetaculos(

    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    nome VARCHAR(150) UNIQUE NOT NULL,

    tipo_evento VARCHAR(80),

    descricao TEXT,

    url_bilheteria TEXT,

    data_inicio TIMESTAMP NOT NULL,

    data_fim TIMESTAMP NOT NULL,

    valor_ingresso DECIMAL(10,2)
    CHECK(valor_ingresso >=0),

    CONSTRAINT chk_datas
    CHECK(data_fim > data_inicio)
);


CREATE TABLE IF NOT EXISTS generos(

    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    nome VARCHAR(50)
    UNIQUE
    NOT NULL
);


CREATE TABLE IF NOT EXISTS interacoes(

    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    usuario_id UUID NOT NULL,

    espetaculo_id UUID NOT NULL,

    tipo tipo_interacao NOT NULL,

    pontuacao DECIMAL(4,2),

    data_registro TIMESTAMP
    DEFAULT CURRENT_TIMESTAMP,

    data_expiracao TIMESTAMP,

    CONSTRAINT fk_usuario_interacao
    FOREIGN KEY(usuario_id)
    REFERENCES usuarios(id)
    ON DELETE CASCADE,

    CONSTRAINT fk_espetaculo_interacao
    FOREIGN KEY(espetaculo_id)
    REFERENCES espetaculos(id)
    ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_interacoes_usuario
ON interacoes(usuario_id);


CREATE TABLE IF NOT EXISTS preferencias(

    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    usuario_id UUID UNIQUE,

    cidade VARCHAR(100),

    tags TEXT[],

    faixa_preco_min DECIMAL(10,2),
    faixa_preco_max DECIMAL(10,2),
    receber_notificacoes BOOLEAN DEFAULT TRUE,

    data_atualizacao TIMESTAMP
    DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_preferencia_usuario
    FOREIGN KEY(usuario_id)
    REFERENCES usuarios(id)
    ON DELETE CASCADE
);

ALTER TABLE preferencias
    ADD COLUMN IF NOT EXISTS tags TEXT[],
    ADD COLUMN IF NOT EXISTS faixa_preco_min DECIMAL(10,2),
    ADD COLUMN IF NOT EXISTS faixa_preco_max DECIMAL(10,2),
    ADD COLUMN IF NOT EXISTS receber_notificacoes BOOLEAN DEFAULT TRUE,
    ADD COLUMN IF NOT EXISTS cep VARCHAR(9);


CREATE TABLE IF NOT EXISTS recomendacoes(

    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    usuario_id UUID NOT NULL,

    espetaculo_id UUID NOT NULL,

    score DECIMAL(4,2)
    CHECK(score >=0 AND score <=1),

    motivo TEXT,

    data_geracao TIMESTAMP
    DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY(usuario_id)
    REFERENCES usuarios(id)
    ON DELETE CASCADE,

    FOREIGN KEY(espetaculo_id)
    REFERENCES espetaculos(id)
    ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_recomendacao_usuario
ON recomendacoes(usuario_id);


CREATE TABLE IF NOT EXISTS usuario_genero_favorito(

    usuario_id UUID,

    genero_id UUID,

    PRIMARY KEY(usuario_id,genero_id),

    FOREIGN KEY(usuario_id)
    REFERENCES usuarios(id)
    ON DELETE CASCADE,

    FOREIGN KEY(genero_id)
    REFERENCES generos(id)
    ON DELETE CASCADE
);


CREATE TABLE IF NOT EXISTS espetaculo_genero(

    espetaculo_id UUID,

    genero_id UUID,

    PRIMARY KEY(espetaculo_id,genero_id),

    FOREIGN KEY(espetaculo_id)
    REFERENCES espetaculos(id)
    ON DELETE CASCADE,

    FOREIGN KEY(genero_id)
    REFERENCES generos(id)
    ON DELETE CASCADE
);


CREATE TABLE IF NOT EXISTS teatro_espetaculo(

    teatro_id UUID,

    espetaculo_id UUID,

    PRIMARY KEY(teatro_id,espetaculo_id),

    FOREIGN KEY(teatro_id)
    REFERENCES teatros(id)
    ON DELETE RESTRICT,

    FOREIGN KEY(espetaculo_id)
    REFERENCES espetaculos(id)
    ON DELETE CASCADE
);


CREATE TABLE IF NOT EXISTS wishlist_curtidas(

    usuario_id UUID,

    espetaculo_id UUID,

    tipo tipo_lista,

    PRIMARY KEY(
        usuario_id,
        espetaculo_id,
        tipo
    ),

    FOREIGN KEY(usuario_id)
    REFERENCES usuarios(id)
    ON DELETE CASCADE,

    FOREIGN KEY(espetaculo_id)
    REFERENCES espetaculos(id)
    ON DELETE CASCADE
);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'uq_teatros_nome'
    ) THEN
        ALTER TABLE teatros
        ADD CONSTRAINT uq_teatros_nome UNIQUE(nome);
    END IF;
END $$;


DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'uq_espetaculos_nome'
    ) THEN
        ALTER TABLE espetaculos
        ADD CONSTRAINT uq_espetaculos_nome UNIQUE(nome);
    END IF;
END $$;
