-- ============================================================
-- maisCampos de endereço (suportar a integração ViaCEP)
-- ============================================================
ALTER TABLE usuarios
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
