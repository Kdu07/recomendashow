-- Optional synthetic data for EXPLAIN/benchmark evidence only.
-- Use this script only on a local or disposable database.
-- It is not functional seed data and must not be called by Dockerfile or DbRunner.
-- Expected order for manual use:
--   1. 01_create_database.sql
--   2. 02_sprint4_improvements.sql
--   3. 03_initial_data.sql
--   4. benchmark/04_synthetic_explain_data.sql

BEGIN;

-- Keep the pgbench/EXPLAIN workload user available with a deterministic id.
INSERT INTO usuarios (
    id,
    nome,
    email,
    senha_hash,
    idade,
    consentimento_aceito,
    data_consentimento
)
VALUES (
    '606f6cae-1d85-46b0-aa6f-dc5fabad9037',
    'Usuario Benchmark Explain',
    'benchmark.explain@example.invalid',
    'synthetic-not-for-login',
    30,
    TRUE,
    CURRENT_TIMESTAMP
)
ON CONFLICT (id) DO UPDATE
SET nome = EXCLUDED.nome,
    email = EXCLUDED.email,
    senha_hash = EXCLUDED.senha_hash,
    idade = EXCLUDED.idade,
    consentimento_aceito = EXCLUDED.consentimento_aceito,
    data_consentimento = COALESCE(usuarios.data_consentimento, EXCLUDED.data_consentimento);

-- Thousands of users with unique ids and emails.
INSERT INTO usuarios (
    id,
    nome,
    email,
    senha_hash,
    idade,
    consentimento_aceito,
    data_consentimento
)
SELECT
    ('00000000-0000-0000-0001-' || lpad(gs::text, 12, '0'))::uuid,
    'Usuario Sintetico ' || gs,
    'synthetic.user.' || gs || '@example.invalid',
    'synthetic-not-for-login',
    18 + (gs % 50),
    TRUE,
    CURRENT_TIMESTAMP - ((gs % 365) || ' days')::interval
FROM generate_series(1, 5000) AS gs
ON CONFLICT (id) DO NOTHING;

-- Theaters distributed across many cities; Sao Paulo is intentionally selective.
INSERT INTO teatros (
    id,
    nome,
    endereco,
    cidade,
    horario_funcionamento
)
SELECT
    ('00000000-0000-0000-0002-' || lpad(gs::text, 12, '0'))::uuid,
    'Teatro Sintetico ' || gs,
    'Rua Sintetica ' || gs,
    CASE
        WHEN gs % 30 = 0 THEN 'São Paulo'
        ELSE 'Cidade Sintetica ' || (gs % 80)
    END,
    '10:00 - 22:00'
FROM generate_series(1, 3000) AS gs
ON CONFLICT (id) DO NOTHING;

-- Shows across broad date ranges; only a minority falls inside the next 90 days.
INSERT INTO espetaculos (
    id,
    nome,
    tipo_evento,
    descricao,
    url_bilheteria,
    data_inicio,
    data_fim,
    valor_ingresso
)
SELECT
    ('00000000-0000-0000-0003-' || lpad(gs::text, 12, '0'))::uuid,
    'Espetaculo Sintetico ' || gs,
    CASE WHEN gs % 4 = 0 THEN 'Musical' ELSE 'Teatro' END,
    'Espetaculo sintetico para benchmark de planos',
    'https://example.invalid/ingressos/' || gs,
    CASE
        WHEN gs % 25 = 0 THEN CURRENT_DATE + ((gs % 89) || ' days')::interval
        ELSE CURRENT_DATE + ((180 + gs) || ' days')::interval
    END,
    CASE
        WHEN gs % 25 = 0 THEN CURRENT_DATE + ((gs % 89) || ' days')::interval + INTERVAL '2 hours'
        ELSE CURRENT_DATE + ((180 + gs) || ' days')::interval + INTERVAL '2 hours'
    END,
    20 + (gs % 180)
FROM generate_series(1, 5000) AS gs
ON CONFLICT (id) DO NOTHING;

-- Link every synthetic show to a deterministic theater.
INSERT INTO teatro_espetaculo (teatro_id, espetaculo_id)
SELECT
    ('00000000-0000-0000-0002-' || lpad((((gs - 1) % 3000) + 1)::text, 12, '0'))::uuid,
    ('00000000-0000-0000-0003-' || lpad(gs::text, 12, '0'))::uuid
FROM generate_series(1, 5000) AS gs
ON CONFLICT DO NOTHING;

-- Extra links for Sao Paulo theaters to make the city join realistic but selective.
INSERT INTO teatro_espetaculo (teatro_id, espetaculo_id)
SELECT
    ('00000000-0000-0000-0002-' || lpad((gs * 30)::text, 12, '0'))::uuid,
    ('00000000-0000-0000-0003-' || lpad((4000 + gs)::text, 12, '0'))::uuid
FROM generate_series(1, 100) AS gs
ON CONFLICT DO NOTHING;

-- Many target-user recommendations exercise idx_recomendacoes_usuario_score.
INSERT INTO recomendacoes (
    usuario_id,
    espetaculo_id,
    score,
    motivo
)
SELECT
    '606f6cae-1d85-46b0-aa6f-dc5fabad9037',
    ('00000000-0000-0000-0003-' || lpad(gs::text, 12, '0'))::uuid,
    round(((1000 - (gs % 1000)) / 1000.0)::numeric, 2),
    'Recomendacao sintetica do usuario benchmark'
FROM generate_series(1, 1500) AS gs
ON CONFLICT (usuario_id, espetaculo_id) DO NOTHING;

-- Broader recommendation volume while respecting UNIQUE(usuario_id, espetaculo_id).
INSERT INTO recomendacoes (
    usuario_id,
    espetaculo_id,
    score,
    motivo
)
SELECT
    ('00000000-0000-0000-0001-' || lpad((((gs - 1) % 5000) + 1)::text, 12, '0'))::uuid,
    ('00000000-0000-0000-0003-' || lpad((((gs - 1) / 5000)::int + 1)::text, 12, '0'))::uuid,
    round(((gs % 100) / 100.0)::numeric, 2),
    'Recomendacao sintetica geral'
FROM generate_series(1, 50000) AS gs
ON CONFLICT (usuario_id, espetaculo_id) DO NOTHING;

-- Target-user interactions exercise idx_interacoes_usuario_expiracao.
INSERT INTO interacoes (
    id,
    usuario_id,
    espetaculo_id,
    tipo,
    pontuacao,
    data_registro,
    data_expiracao
)
SELECT
    ('00000000-0000-0000-0004-' || lpad(gs::text, 12, '0'))::uuid,
    '606f6cae-1d85-46b0-aa6f-dc5fabad9037',
    ('00000000-0000-0000-0003-' || lpad((((gs - 1) % 5000) + 1)::text, 12, '0'))::uuid,
    CASE WHEN gs % 2 = 0 THEN 'Clique'::tipo_interacao ELSE 'Acesso'::tipo_interacao END,
    round(((gs % 100) / 10.0)::numeric, 2),
    CURRENT_TIMESTAMP - (((gs % 120) + 1) || ' days')::interval,
    CURRENT_TIMESTAMP + (((gs % 180) + 1) || ' days')::interval
FROM generate_series(1, 5000) AS gs
ON CONFLICT (id) DO NOTHING;

-- General interaction volume with valid dates and scores.
INSERT INTO interacoes (
    id,
    usuario_id,
    espetaculo_id,
    tipo,
    pontuacao,
    data_registro,
    data_expiracao
)
SELECT
    ('00000000-0000-0000-0005-' || lpad(gs::text, 12, '0'))::uuid,
    ('00000000-0000-0000-0001-' || lpad((((gs - 1) % 5000) + 1)::text, 12, '0'))::uuid,
    ('00000000-0000-0000-0003-' || lpad((((gs - 1) % 5000) + 1)::text, 12, '0'))::uuid,
    CASE
        WHEN gs % 3 = 0 THEN 'TempoDeTela'::tipo_interacao
        WHEN gs % 3 = 1 THEN 'Clique'::tipo_interacao
        ELSE 'Acesso'::tipo_interacao
    END,
    round(((gs % 100) / 10.0)::numeric, 2),
    CURRENT_TIMESTAMP - ((gs % 180) || ' days')::interval,
    CURRENT_TIMESTAMP + ((gs % 365) || ' days')::interval + INTERVAL '1 day'
FROM generate_series(1, 50000) AS gs
ON CONFLICT (id) DO NOTHING;

-- Target-user wishlist/curtidas exercise idx_wishlist_usuario.
INSERT INTO wishlist_curtidas (usuario_id, espetaculo_id, tipo)
SELECT
    '606f6cae-1d85-46b0-aa6f-dc5fabad9037',
    ('00000000-0000-0000-0003-' || lpad(gs::text, 12, '0'))::uuid,
    CASE WHEN gs % 2 = 0 THEN 'wishlist'::tipo_lista ELSE 'curtida'::tipo_lista END
FROM generate_series(1, 2000) AS gs
ON CONFLICT DO NOTHING;

-- General wishlist/curtidas volume.
INSERT INTO wishlist_curtidas (usuario_id, espetaculo_id, tipo)
SELECT
    ('00000000-0000-0000-0001-' || lpad((((gs - 1) % 5000) + 1)::text, 12, '0'))::uuid,
    ('00000000-0000-0000-0003-' || lpad((((gs - 1) % 5000) + 1)::text, 12, '0'))::uuid,
    CASE WHEN gs % 2 = 0 THEN 'wishlist'::tipo_lista ELSE 'curtida'::tipo_lista END
FROM generate_series(1, 20000) AS gs
ON CONFLICT DO NOTHING;

COMMIT;

ANALYZE usuarios;
ANALYZE teatros;
ANALYZE espetaculos;
ANALYZE teatro_espetaculo;
ANALYZE recomendacoes;
ANALYZE interacoes;
ANALYZE wishlist_curtidas;
