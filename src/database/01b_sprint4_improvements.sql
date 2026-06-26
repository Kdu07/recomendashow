
-- Quality enhancement

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'chk_pontuacao'
    ) THEN
        ALTER TABLE interacoes
        ADD CONSTRAINT chk_pontuacao
        CHECK (pontuacao >= 0 AND pontuacao <= 10);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'chk_data_expiracao'
    ) THEN
        ALTER TABLE interacoes
        ADD CONSTRAINT chk_data_expiracao
        CHECK (data_expiracao > data_registro);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'uq_recomendacao_usuario_espetaculo'
    ) THEN
        ALTER TABLE recomendacoes
        ADD CONSTRAINT uq_recomendacao_usuario_espetaculo
        UNIQUE (usuario_id, espetaculo_id);
    END IF;
END $$;

-- Type migration: TIMESTAMP → TIMESTAMPTZ

DO $$
BEGIN
    IF (SELECT data_type FROM information_schema.columns
        WHERE table_name = 'espetaculos' AND column_name = 'data_inicio') = 'timestamp without time zone' THEN
        ALTER TABLE espetaculos ALTER COLUMN data_inicio TYPE TIMESTAMPTZ USING data_inicio AT TIME ZONE 'UTC';
    END IF;
END $$;

DO $$
BEGIN
    IF (SELECT data_type FROM information_schema.columns
        WHERE table_name = 'espetaculos' AND column_name = 'data_fim') = 'timestamp without time zone' THEN
        ALTER TABLE espetaculos ALTER COLUMN data_fim TYPE TIMESTAMPTZ USING data_fim AT TIME ZONE 'UTC';
    END IF;
END $$;

-- Adress fields (suport ViaCEP integration)

ALTER TABLE usuarios
    ADD COLUMN IF NOT EXISTS data_nascimento DATE,
    ADD COLUMN IF NOT EXISTS cep         VARCHAR(9),
    ADD COLUMN IF NOT EXISTS logradouro  VARCHAR(255),
    ADD COLUMN IF NOT EXISTS numero      VARCHAR(10),
    ADD COLUMN IF NOT EXISTS complemento VARCHAR(255),
    ADD COLUMN IF NOT EXISTS bairro      VARCHAR(100),
    ADD COLUMN IF NOT EXISTS cidade      VARCHAR(100);

-- LGPD

ALTER TABLE usuarios
    ADD COLUMN IF NOT EXISTS consentimento_aceito BOOLEAN NOT NULL DEFAULT FALSE,
    ADD COLUMN IF NOT EXISTS data_consentimento   TIMESTAMPTZ,
    ADD COLUMN IF NOT EXISTS deleted_at           TIMESTAMPTZ,
    ADD COLUMN IF NOT EXISTS anonimizado          BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE usuarios ADD COLUMN IF NOT EXISTS anonimizado_em TIMESTAMPTZ;

-- Performance indexes

DROP INDEX IF EXISTS idx_interacoes_usuario;
DROP INDEX IF EXISTS idx_recomendacao_usuario;

CREATE INDEX IF NOT EXISTS idx_interacoes_usuario_expiracao
    ON interacoes(usuario_id, data_expiracao);

CREATE INDEX IF NOT EXISTS idx_teatros_cidade
    ON teatros(cidade);

CREATE INDEX IF NOT EXISTS idx_espetaculos_datas
    ON espetaculos(data_inicio, data_fim);

CREATE INDEX IF NOT EXISTS idx_recomendacoes_usuario_score
    ON recomendacoes(usuario_id, score DESC);

CREATE INDEX IF NOT EXISTS idx_wishlist_usuario
    ON wishlist_curtidas(usuario_id);

-- ML integration: integer identification of recomender

ALTER TABLE usuarios ADD COLUMN IF NOT EXISTS ml_id SERIAL;
CREATE UNIQUE INDEX IF NOT EXISTS idx_usuarios_ml_id ON usuarios(ml_id);

-- New Collumn

ALTER TABLE recomendacoes ADD COLUMN IF NOT EXISTS visualizada_em TIMESTAMPTZ;

-- Triggers e procedure

CREATE OR REPLACE FUNCTION fn_atualizar_data_preferencias()
RETURNS TRIGGER AS $$
BEGIN
    NEW.data_atualizacao = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_preferencias_update ON preferencias;

CREATE TRIGGER trg_preferencias_update
    BEFORE UPDATE ON preferencias
    FOR EACH ROW
    EXECUTE FUNCTION fn_atualizar_data_preferencias();

-- Objects for LGPD

CREATE OR REPLACE PROCEDURE sp_anonimizar_usuario(p_id UUID)
LANGUAGE plpgsql AS $$
DECLARE
    v_deleted_at TIMESTAMPTZ;
BEGIN
    SELECT deleted_at INTO v_deleted_at
    FROM usuarios
    WHERE id = p_id;

    IF v_deleted_at IS NULL THEN
        RAISE EXCEPTION
            'Usuário % não possui deleted_at definido. Execute soft-delete antes de anonimizar.',
            p_id;
    END IF;

    UPDATE usuarios SET
        nome        = 'Usuário Removido',
        email       = id::text || '@anonimizado.invalid',
        senha_hash  = '',
        data_nascimento = NULL,
        cep         = NULL,
        logradouro  = NULL,
        numero      = NULL,
        complemento = NULL,
        bairro      = NULL,
        cidade      = NULL,
        anonimizado    = TRUE,
        anonimizado_em = CURRENT_TIMESTAMP
    WHERE id = p_id
      AND deleted_at IS NOT NULL
      AND anonimizado = FALSE;
END; $$;

CREATE OR REPLACE FUNCTION fn_bloquear_update_anonimizado()
RETURNS TRIGGER AS $$
BEGIN
    IF OLD.anonimizado = TRUE THEN
        RAISE EXCEPTION
            'Usuário % já foi anonimizado (LGPD). Nenhuma alteração é permitida.',
            OLD.id;
    END IF;
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_bloquear_update_anonimizado ON usuarios;
CREATE TRIGGER trg_bloquear_update_anonimizado
    BEFORE UPDATE ON usuarios
    FOR EACH ROW
    EXECUTE FUNCTION fn_bloquear_update_anonimizado();

CREATE OR REPLACE FUNCTION fn_sincronizar_data_consentimento()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.consentimento_aceito = TRUE
       AND OLD.consentimento_aceito = FALSE
       AND NEW.data_consentimento IS NULL THEN
        NEW.data_consentimento = CURRENT_TIMESTAMP;
    END IF;
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_sincronizar_data_consentimento ON usuarios;
CREATE TRIGGER trg_sincronizar_data_consentimento
    BEFORE UPDATE ON usuarios
    FOR EACH ROW
    EXECUTE FUNCTION fn_sincronizar_data_consentimento();

-- Rastreabilidade: data de atualização do perfil do usuário

ALTER TABLE usuarios
    ADD COLUMN IF NOT EXISTS data_atualizacao TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP;

CREATE OR REPLACE FUNCTION fn_atualizar_data_usuario()
RETURNS TRIGGER AS $$
BEGIN
    NEW.data_atualizacao = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_usuarios_update ON usuarios;
CREATE TRIGGER trg_usuarios_update
    BEFORE UPDATE ON usuarios
    FOR EACH ROW
    EXECUTE FUNCTION fn_atualizar_data_usuario();

-- Rastreabilidade: momento de adição à wishlist/curtidas

ALTER TABLE wishlist_curtidas
    ADD COLUMN IF NOT EXISTS adicionado_em TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP;
