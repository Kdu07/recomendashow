-- ============================================================
-- Correções de qualidade
-- ============================================================

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

-- ============================================================
-- Campos de endereço (suportar a integração ViaCEP)
-- ============================================================
ALTER TABLE usuarios
    ADD COLUMN IF NOT EXISTS data_nascimento DATE,
    ADD COLUMN IF NOT EXISTS cep         VARCHAR(9),
    ADD COLUMN IF NOT EXISTS logradouro  VARCHAR(255),
    ADD COLUMN IF NOT EXISTS numero      VARCHAR(10),
    ADD COLUMN IF NOT EXISTS complemento VARCHAR(255),
    ADD COLUMN IF NOT EXISTS bairro      VARCHAR(100),
    ADD COLUMN IF NOT EXISTS cidade      VARCHAR(100);

-- ============================================================
-- Conformidade com LGPD
-- ============================================================
ALTER TABLE usuarios
    ADD COLUMN IF NOT EXISTS consentimento_aceito BOOLEAN NOT NULL DEFAULT FALSE,
    ADD COLUMN IF NOT EXISTS data_consentimento   TIMESTAMP,
    ADD COLUMN IF NOT EXISTS deleted_at           TIMESTAMP,
    ADD COLUMN IF NOT EXISTS anonimizado          BOOLEAN NOT NULL DEFAULT FALSE;

-- ============================================================
-- Índices de performance
-- ============================================================

-- Removidos: substituídos pelos índices compostos abaixo, que cobrem o prefixo esquerdo
DROP INDEX IF EXISTS idx_interacoes_usuario;
DROP INDEX IF EXISTS idx_recomendacao_usuario;

-- Motor de recomendação filtra interações por usuário e janela temporal constantemente
CREATE INDEX IF NOT EXISTS idx_interacoes_usuario_expiracao
    ON interacoes(usuario_id, data_expiracao);

-- Carrossel "Perto de você" filtra espetáculos pela cidade do teatro
CREATE INDEX IF NOT EXISTS idx_teatros_cidade
    ON teatros(cidade);

-- Filtragem de espetáculos em cartaz usa esse intervalo de datas
CREATE INDEX IF NOT EXISTS idx_espetaculos_datas
    ON espetaculos(data_inicio, data_fim);

-- Busca as top recomendações de um usuário ordenadas por relevância
CREATE INDEX IF NOT EXISTS idx_recomendacoes_usuario_score
    ON recomendacoes(usuario_id, score DESC);

-- A wishlist é carregada a cada abertura da aba pelo usuário
CREATE INDEX IF NOT EXISTS idx_wishlist_usuario
    ON wishlist_curtidas(usuario_id);

-- ============================================================
-- Integração ML: identificador inteiro para o motor de recomendação
-- ============================================================

-- Identificador sequencial para mapear usuarios UUID ↔ índice inteiro do modelo SVD
ALTER TABLE usuarios ADD COLUMN IF NOT EXISTS ml_id SERIAL;
CREATE UNIQUE INDEX IF NOT EXISTS idx_usuarios_ml_id ON usuarios(ml_id);

-- ============================================================
-- Triggers e procedure
-- ============================================================

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

-- ============================================================
-- Objetos de conformidade LGPD
-- ============================================================

CREATE OR REPLACE PROCEDURE sp_anonimizar_usuario(p_id UUID)
LANGUAGE plpgsql AS $$
BEGIN
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
        anonimizado = TRUE
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
