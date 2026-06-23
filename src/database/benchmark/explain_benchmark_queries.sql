-- EXPLAIN evidence queries for Sprint 4 indexes with realistic synthetic volume.
-- Run after benchmark/04_synthetic_explain_data.sql on a local/disposable database.
-- Do not use SET enable_seqscan=off here; the goal is natural planner selection.

-- Expected index: idx_recomendacoes_usuario_score
EXPLAIN (ANALYZE, BUFFERS)
SELECT *
FROM recomendacoes
WHERE usuario_id = '606f6cae-1d85-46b0-aa6f-dc5fabad9037'
ORDER BY score DESC
LIMIT 50;

-- Expected index: idx_teatros_cidade
EXPLAIN (ANALYZE, BUFFERS)
SELECT e.*
FROM teatros t
JOIN teatro_espetaculo te ON te.teatro_id = t.id
JOIN espetaculos e ON e.id = te.espetaculo_id
WHERE t.cidade = 'São Paulo';

-- Expected index: idx_espetaculos_datas
EXPLAIN (ANALYZE, BUFFERS)
SELECT *
FROM espetaculos
WHERE data_inicio >= CURRENT_DATE
  AND data_fim <= CURRENT_DATE + INTERVAL '90 days';

-- Expected index: idx_interacoes_usuario_expiracao
EXPLAIN (ANALYZE, BUFFERS)
SELECT *
FROM interacoes
WHERE usuario_id = '606f6cae-1d85-46b0-aa6f-dc5fabad9037'
ORDER BY data_expiracao DESC
LIMIT 50;

-- Expected index: idx_wishlist_usuario
EXPLAIN (ANALYZE, BUFFERS)
SELECT *
FROM wishlist_curtidas
WHERE usuario_id = '606f6cae-1d85-46b0-aa6f-dc5fabad9037';
