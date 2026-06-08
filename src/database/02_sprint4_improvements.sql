-- ============================================================
-- Campos de endereço (suportar a integração ViaCEP)
-- ============================================================
ALTER TABLE usuarios
    ADD COLUMN IF NOT EXISTS cep         VARCHAR(9),
    ADD COLUMN IF NOT EXISTS logradouro  VARCHAR(255),
    ADD COLUMN IF NOT EXISTS numero      VARCHAR(10),
    ADD COLUMN IF NOT EXISTS complemento VARCHAR(255),
    ADD COLUMN IF NOT EXISTS bairro      VARCHAR(100),
    ADD COLUMN IF NOT EXISTS cidade      VARCHAR(100);

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
