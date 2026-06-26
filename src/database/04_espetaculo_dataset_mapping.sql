-- ML integration (Caminho B): mapeamento espetáculo -> show do dataset
--
-- O batch de recomendação (inference-service) emite, para cada recomendação, o
-- nome cru do show do dataset (item_id, CAIXA ALTA, ex.: 'A NOVIÇA REBELDE'),
-- enquanto espetaculos.nome guarda o nome "bonito" ('A Noviça Rebelde'). A
-- igualdade exata nunca casava -> recomendacoes ficava em 0.
--
-- Esta migration adiciona a coluna de mapeamento item_dataset (análoga ao ml_id
-- dos usuários): guarda o nome do show do dataset correspondente ao espetáculo.
-- A escrita (RecommendationWriteRepository) passa a resolver o espetáculo por
-- item_dataset com normalização defensiva (TRIM + UPPER + unaccent), mantendo os
-- nomes "bonitos" em espetaculos.nome.
--
-- Idempotente. Roda após 01_create_database.sql (tabela espetaculos).

-- Normalização acento-insensível usada pelo writer (upper(unaccent(btrim(...)))).
CREATE EXTENSION IF NOT EXISTS unaccent;

ALTER TABLE espetaculos ADD COLUMN IF NOT EXISTS item_dataset TEXT;

-- Espetáculo sem correspondência fica com item_dataset NULL (não recebe recs);
-- o índice único permite múltiplos NULLs e impede dois espetáculos no mesmo show.
CREATE UNIQUE INDEX IF NOT EXISTS idx_espetaculos_item_dataset
    ON espetaculos(item_dataset);

-- Espetáculos do catálogo curado COM correspondência no dataset (6).
-- item_dataset = nome cru EXATO do show no dataset (como o modelo emite).
UPDATE espetaculos SET item_dataset = 'A NOVIÇA REBELDE'
    WHERE nome = 'A Noviça Rebelde';
UPDATE espetaculos SET item_dataset = 'ELIS, A MUSICAL'
    WHERE nome = 'Elis';
UPDATE espetaculos SET item_dataset = 'MUSICAL HAIRSPRAY'
    WHERE nome = 'Hairspray';
UPDATE espetaculos SET item_dataset = 'LES MISERABLES SCHOOL EDITION'
    WHERE nome = 'Les Misérables';
UPDATE espetaculos SET item_dataset = 'VOZES NEGRAS – A FORÇA DO CANTO FEMININO'
    WHERE nome = 'Vozes Negras';
UPDATE espetaculos SET item_dataset = 'CIA DE BALLET DO RIO DE JANEIRO - ROMEU E JULIETA'
    WHERE nome = 'Romeu e Julieta';
