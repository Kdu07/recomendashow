-- ============================================================
-- SEÇÃO 1: Correções de qualidade
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
-- SEÇÃO 5: Campos de endereço (suportar a integração ViaCEP)
-- ============================================================
ALTER TABLE usuarios
    ADD COLUMN IF NOT EXISTS cep         VARCHAR(9),
    ADD COLUMN IF NOT EXISTS logradouro  VARCHAR(255),
    ADD COLUMN IF NOT EXISTS numero      VARCHAR(10),
    ADD COLUMN IF NOT EXISTS complemento VARCHAR(255),
    ADD COLUMN IF NOT EXISTS bairro      VARCHAR(100),
    ADD COLUMN IF NOT EXISTS cidade      VARCHAR(100);
