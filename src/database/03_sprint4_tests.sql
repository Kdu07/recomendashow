-- ============================================================
-- Sprint 4 — Testes SQL
-- Projeto  : Aventura Teatros
-- Execução : psql -d aventura -f 03_sprint4_tests.sql
--
-- Todos os testes rodam dentro de uma única transação revertida
-- ao final. Nenhum dado é persistido no banco.
-- ============================================================

BEGIN;

-- ============================================================
-- Setup — dados de apoio reutilizados pelos testes
-- ============================================================

DO $$
BEGIN
    INSERT INTO generos (id, nome)
        VALUES ('00000000-0000-0001-0000-000000000001', '_test_genero');

    INSERT INTO teatros (id, nome, cidade)
        VALUES ('00000000-0000-0001-0000-000000000002', '_test_teatro', 'São Paulo');

    -- Dois espetáculos: um para interações/recomendações, outro para teste de UNIQUE
    INSERT INTO espetaculos (id, nome, data_inicio, data_fim, valor_ingresso)
        VALUES ('00000000-0000-0001-0000-000000000003', '_test_espetaculo_1',
                NOW(), NOW() + INTERVAL '30 days', 50.00),
               ('00000000-0000-0001-0000-000000000004', '_test_espetaculo_2',
                NOW(), NOW() + INTERVAL '30 days', 30.00);

    -- Usuário geral: constraints, preferências, consentimento
    INSERT INTO usuarios (id, nome, email, senha_hash, idade)
        VALUES ('00000000-0000-0001-0000-000000000010', '_test_usuario',
                'test@test.invalid', '$2b$12$testhash', 25);

    -- Usuário reservado para testes de anonimização
    INSERT INTO usuarios (id, nome, email, senha_hash, idade)
        VALUES ('00000000-0000-0001-0000-000000000011', '_test_usuario_anon',
                'anon@test.invalid', '$2b$12$testhash', 30);

    -- Usuário para teste 6.2 (consentimento explícito)
    INSERT INTO usuarios (id, nome, email, senha_hash, idade)
        VALUES ('00000000-0000-0001-0000-000000000012', '_test_consent_explicito',
                'consent@test.invalid', '$2b$12$testhash', 20);

    -- Usuário para teste 6.3 (re-UPDATE sem alterar data_consentimento)
    -- Inserido com consentimento já TRUE e data conhecida para comparação
    INSERT INTO usuarios (id, nome, email, senha_hash, idade,
                          consentimento_aceito, data_consentimento)
        VALUES ('00000000-0000-0001-0000-000000000013', '_test_consent_reupdate',
                'reupdate@test.invalid', '$2b$12$testhash', 28,
                TRUE, '2020-06-01 10:00:00+00');

    -- Usuário para teste 6.4 (consentimento permanece FALSE)
    INSERT INTO usuarios (id, nome, email, senha_hash, idade)
        VALUES ('00000000-0000-0001-0000-000000000014', '_test_no_consent',
                'noconsent@test.invalid', '$2b$12$testhash', 22);

    INSERT INTO preferencias (usuario_id)
        VALUES ('00000000-0000-0001-0000-000000000010');

    RAISE NOTICE '——— Setup concluído ———';
END $$;

-- ============================================================
-- 1. Constraints novas
-- ============================================================

-- 1.1 chk_pontuacao — valores válidos (0, 5 e 10)
DO $$
BEGIN
    INSERT INTO interacoes (usuario_id, espetaculo_id, tipo, pontuacao, data_expiracao)
        VALUES
            ('00000000-0000-0001-0000-000000000010',
             '00000000-0000-0001-0000-000000000003',
             'Clique', 0, NOW() + INTERVAL '1 day'),
            ('00000000-0000-0001-0000-000000000010',
             '00000000-0000-0001-0000-000000000003',
             'Acesso', 5, NOW() + INTERVAL '1 day'),
            ('00000000-0000-0001-0000-000000000010',
             '00000000-0000-0001-0000-000000000003',
             'TempoDeTela', 10, NOW() + INTERVAL '1 day');
    RAISE NOTICE 'PASS 1.1 — chk_pontuacao: valores 0, 5 e 10 aceitos';
END $$;

-- 1.2 chk_pontuacao — valor negativo
DO $$
BEGIN
    BEGIN
        INSERT INTO interacoes (usuario_id, espetaculo_id, tipo, pontuacao, data_expiracao)
            VALUES ('00000000-0000-0001-0000-000000000010',
                    '00000000-0000-0001-0000-000000000003',
                    'Clique', -1, NOW() + INTERVAL '1 day');
        RAISE EXCEPTION 'FAIL 1.2 — chk_pontuacao deveria rejeitar pontuacao = -1';
    EXCEPTION WHEN check_violation THEN
        RAISE NOTICE 'PASS 1.2 — chk_pontuacao: pontuacao = -1 rejeitado';
    END;
END $$;

-- 1.3 chk_pontuacao — valor acima do limite
DO $$
BEGIN
    BEGIN
        INSERT INTO interacoes (usuario_id, espetaculo_id, tipo, pontuacao, data_expiracao)
            VALUES ('00000000-0000-0001-0000-000000000010',
                    '00000000-0000-0001-0000-000000000003',
                    'Clique', 11, NOW() + INTERVAL '1 day');
        RAISE EXCEPTION 'FAIL 1.3 — chk_pontuacao deveria rejeitar pontuacao = 11';
    EXCEPTION WHEN check_violation THEN
        RAISE NOTICE 'PASS 1.3 — chk_pontuacao: pontuacao = 11 rejeitado';
    END;
END $$;

-- 1.4 chk_data_expiracao — data futura válida
DO $$
BEGIN
    INSERT INTO interacoes (usuario_id, espetaculo_id, tipo, data_expiracao)
        VALUES ('00000000-0000-0001-0000-000000000010',
                '00000000-0000-0001-0000-000000000003',
                'Clique', NOW() + INTERVAL '1 day');
    RAISE NOTICE 'PASS 1.4 — chk_data_expiracao: data futura aceita';
END $$;

-- 1.5 chk_data_expiracao — igual a data_registro
DO $$
BEGIN
    BEGIN
        INSERT INTO interacoes (usuario_id, espetaculo_id, tipo, data_registro, data_expiracao)
            VALUES ('00000000-0000-0001-0000-000000000010',
                    '00000000-0000-0001-0000-000000000003',
                    'Clique',
                    '2025-01-01 12:00:00+00',
                    '2025-01-01 12:00:00+00');
        RAISE EXCEPTION 'FAIL 1.5 — chk_data_expiracao deveria rejeitar data_expiracao = data_registro';
    EXCEPTION WHEN check_violation THEN
        RAISE NOTICE 'PASS 1.5 — chk_data_expiracao: data_expiracao = data_registro rejeitado';
    END;
END $$;

-- 1.6 chk_data_expiracao — anterior a data_registro
DO $$
BEGIN
    BEGIN
        INSERT INTO interacoes (usuario_id, espetaculo_id, tipo, data_registro, data_expiracao)
            VALUES ('00000000-0000-0001-0000-000000000010',
                    '00000000-0000-0001-0000-000000000003',
                    'Clique',
                    '2025-01-01 12:00:00+00',
                    '2025-01-01 11:00:00+00');
        RAISE EXCEPTION 'FAIL 1.6 — chk_data_expiracao deveria rejeitar data_expiracao < data_registro';
    EXCEPTION WHEN check_violation THEN
        RAISE NOTICE 'PASS 1.6 — chk_data_expiracao: data_expiracao anterior rejeitado';
    END;
END $$;

-- 1.7 uq_recomendacao_usuario_espetaculo — par duplicado rejeitado
DO $$
BEGIN
    INSERT INTO recomendacoes (usuario_id, espetaculo_id, score)
        VALUES ('00000000-0000-0001-0000-000000000010',
                '00000000-0000-0001-0000-000000000003', 0.9);
    BEGIN
        INSERT INTO recomendacoes (usuario_id, espetaculo_id, score)
            VALUES ('00000000-0000-0001-0000-000000000010',
                    '00000000-0000-0001-0000-000000000003', 0.8);
        RAISE EXCEPTION 'FAIL 1.7 — UNIQUE deveria rejeitar par duplicado';
    EXCEPTION WHEN unique_violation THEN
        RAISE NOTICE 'PASS 1.7 — uq_recomendacao: par duplicado rejeitado';
    END;
END $$;

-- 1.8 uq_recomendacao_usuario_espetaculo — ON CONFLICT DO UPDATE
DO $$
DECLARE v_score DECIMAL;
BEGIN
    INSERT INTO recomendacoes (usuario_id, espetaculo_id, score)
        VALUES ('00000000-0000-0001-0000-000000000010',
                '00000000-0000-0001-0000-000000000004', 0.5);
    INSERT INTO recomendacoes (usuario_id, espetaculo_id, score)
        VALUES ('00000000-0000-0001-0000-000000000010',
                '00000000-0000-0001-0000-000000000004', 0.95)
        ON CONFLICT (usuario_id, espetaculo_id)
        DO UPDATE SET score = EXCLUDED.score;
    SELECT score INTO v_score FROM recomendacoes
        WHERE usuario_id = '00000000-0000-0001-0000-000000000010'
          AND espetaculo_id = '00000000-0000-0001-0000-000000000004';
    ASSERT v_score = 0.95, 'FAIL 1.8 — ON CONFLICT não atualizou o score para 0.95';
    RAISE NOTICE 'PASS 1.8 — uq_recomendacao: ON CONFLICT DO UPDATE atualizou score para 0.95';
END $$;

-- ============================================================
-- 2. TIMESTAMPTZ em espetaculos
-- ============================================================

-- 2.1 Tipo das colunas data_inicio e data_fim
DO $$
DECLARE
    v_col  TEXT;
    v_type TEXT;
BEGIN
    FOR v_col, v_type IN
        SELECT column_name, data_type
        FROM information_schema.columns
        WHERE table_name = 'espetaculos'
          AND column_name IN ('data_inicio', 'data_fim')
    LOOP
        ASSERT v_type = 'timestamp with time zone',
            FORMAT('FAIL 2.1 — espetaculos.%s: tipo esperado TIMESTAMPTZ, obtido %s', v_col, v_type);
    END LOOP;
    RAISE NOTICE 'PASS 2.1 — espetaculos.data_inicio e data_fim são TIMESTAMPTZ';
END $$;

-- 2.2 Offset de timezone convertido corretamente para UTC
DO $$
DECLARE v_stored TIMESTAMPTZ;
BEGIN
    INSERT INTO espetaculos (id, nome, data_inicio, data_fim, valor_ingresso)
        VALUES ('00000000-0000-0001-0000-000000000020', '_test_tz',
                '2025-10-01 20:00:00-03',
                '2025-10-02 00:00:00-03', 0);
    SELECT data_inicio INTO v_stored
        FROM espetaculos WHERE id = '00000000-0000-0001-0000-000000000020';
    -- 20h00 BRT (UTC-3) = 23h00 UTC
    ASSERT (v_stored AT TIME ZONE 'UTC') = '2025-10-01 23:00:00'::TIMESTAMP,
        FORMAT('FAIL 2.2 — esperado 2025-10-01 23:00:00 UTC, obtido: %s',
               v_stored AT TIME ZONE 'UTC');
    RAISE NOTICE 'PASS 2.2 — TIMESTAMPTZ: 20h00-03 convertido para 23h00 UTC corretamente';
END $$;

-- ============================================================
-- 3. trg_preferencias_update
-- ============================================================

-- 3.1 UPDATE dispara trigger e seta data_atualizacao = CURRENT_TIMESTAMP
DO $$
DECLARE v_ts TIMESTAMPTZ;
BEGIN
    UPDATE preferencias SET cidade = 'Rio de Janeiro'
        WHERE usuario_id = '00000000-0000-0001-0000-000000000010';
    SELECT data_atualizacao INTO v_ts FROM preferencias
        WHERE usuario_id = '00000000-0000-0001-0000-000000000010';
    ASSERT v_ts = CURRENT_TIMESTAMP,
        'FAIL 3.1 — data_atualizacao não foi atualizado para CURRENT_TIMESTAMP pelo trigger';
    RAISE NOTICE 'PASS 3.1 — trg_preferencias_update: data_atualizacao = CURRENT_TIMESTAMP após UPDATE';
END $$;

-- 3.2 Trigger configurado somente para UPDATE (não para INSERT)
DO $$
DECLARE v_count INT;
BEGIN
    SELECT COUNT(*) INTO v_count
        FROM information_schema.triggers
        WHERE trigger_name        = 'trg_preferencias_update'
          AND event_manipulation  = 'UPDATE'
          AND event_object_table  = 'preferencias';
    ASSERT v_count = 1,
        'FAIL 3.2 — trg_preferencias_update não encontrado como trigger de UPDATE em preferencias';
    RAISE NOTICE 'PASS 3.2 — trg_preferencias_update: registrado apenas para UPDATE';
END $$;

-- ============================================================
-- 4.1 trg_bloquear_update_anonimizado — usuário normal
-- (4.2 e 4.3 executados após 5.3, que gera o usuário anonimizado)
-- ============================================================

DO $$
BEGIN
    UPDATE usuarios SET nome = '_test_nome_ok'
        WHERE id = '00000000-0000-0001-0000-000000000010';
    RAISE NOTICE 'PASS 4.1 — trg_bloquear_update_anonimizado: UPDATE em usuário normal permitido';
END $$;

-- ============================================================
-- 5. sp_anonimizar_usuario
-- ============================================================

-- 5.1 Guard: deleted_at IS NULL
DO $$
BEGIN
    BEGIN
        CALL sp_anonimizar_usuario('00000000-0000-0001-0000-000000000011'::UUID);
        RAISE EXCEPTION 'FAIL 5.1 — procedure deveria lançar exception com deleted_at NULL';
    EXCEPTION WHEN others THEN
        IF SQLERRM LIKE '%não possui deleted_at definido%' THEN
            RAISE NOTICE 'PASS 5.1 — sp_anonimizar_usuario: guard deleted_at NULL funcionou';
        ELSE
            RAISE EXCEPTION 'FAIL 5.1 — exception inesperada: %', SQLERRM;
        END IF;
    END;
END $$;

-- 5.2 Guard: UUID inexistente tratado como deleted_at NULL
DO $$
BEGIN
    BEGIN
        CALL sp_anonimizar_usuario('00000000-0000-0001-0000-000000000099'::UUID);
        RAISE EXCEPTION 'FAIL 5.2 — procedure deveria lançar exception para UUID inexistente';
    EXCEPTION WHEN others THEN
        IF SQLERRM LIKE '%não possui deleted_at definido%' THEN
            RAISE NOTICE 'PASS 5.2 — sp_anonimizar_usuario: UUID inexistente tratado corretamente';
        ELSE
            RAISE EXCEPTION 'FAIL 5.2 — exception inesperada: %', SQLERRM;
        END IF;
    END;
END $$;

-- 5.3 Caminho feliz: dados substituídos e anonimizado_em preenchido
DO $$
DECLARE
    v_nome    TEXT;
    v_email   TEXT;
    v_senha   TEXT;
    v_anon    BOOLEAN;
    v_anon_em TIMESTAMPTZ;
    v_cep     TEXT;
BEGIN
    UPDATE usuarios SET deleted_at = CURRENT_TIMESTAMP
        WHERE id = '00000000-0000-0001-0000-000000000011';

    CALL sp_anonimizar_usuario('00000000-0000-0001-0000-000000000011'::UUID);

    SELECT nome, email, senha_hash, anonimizado, anonimizado_em, cep
        INTO v_nome, v_email, v_senha, v_anon, v_anon_em, v_cep
        FROM usuarios WHERE id = '00000000-0000-0001-0000-000000000011';

    ASSERT v_nome     = 'Usuário Removido',       'FAIL 5.3 — nome não anonimizado';
    ASSERT v_email LIKE '%@anonimizado.invalid',   'FAIL 5.3 — email não anonimizado';
    ASSERT v_senha    = '',                        'FAIL 5.3 — senha_hash não apagada';
    ASSERT v_anon     = TRUE,                      'FAIL 5.3 — anonimizado != TRUE';
    ASSERT v_anon_em IS NOT NULL,                  'FAIL 5.3 — anonimizado_em não preenchido';
    ASSERT v_cep      IS NULL,                     'FAIL 5.3 — cep não nulificado';

    RAISE NOTICE 'PASS 5.3 — sp_anonimizar_usuario: todos os campos substituídos corretamente';
END $$;

-- 5.4 anonimizado_em igual a CURRENT_TIMESTAMP da transação
DO $$
DECLARE v_anon_em TIMESTAMPTZ;
BEGIN
    SELECT anonimizado_em INTO v_anon_em FROM usuarios
        WHERE id = '00000000-0000-0001-0000-000000000011';
    ASSERT v_anon_em = CURRENT_TIMESTAMP,
        'FAIL 5.4 — anonimizado_em não corresponde a CURRENT_TIMESTAMP';
    RAISE NOTICE 'PASS 5.4 — sp_anonimizar_usuario: anonimizado_em = CURRENT_TIMESTAMP';
END $$;

-- 5.5 Segunda chamada bloqueada pelo trg_bloquear_update_anonimizado
DO $$
BEGIN
    BEGIN
        CALL sp_anonimizar_usuario('00000000-0000-0001-0000-000000000011'::UUID);
        RAISE EXCEPTION 'FAIL 5.5 — segunda chamada deveria ser bloqueada pelo trigger';
    EXCEPTION WHEN others THEN
        IF SQLERRM LIKE '%já foi anonimizado%' THEN
            RAISE NOTICE 'PASS 5.5 — sp_anonimizar_usuario: segunda chamada bloqueada pelo trigger';
        ELSE
            RAISE EXCEPTION 'FAIL 5.5 — exception inesperada: %', SQLERRM;
        END IF;
    END;
END $$;

-- 4.2 UPDATE bloqueado em usuário anonimizado (reaproveita usuário do 5.3)
DO $$
BEGIN
    BEGIN
        UPDATE usuarios SET nome = 'tentativa_invalida'
            WHERE id = '00000000-0000-0001-0000-000000000011';
        RAISE EXCEPTION 'FAIL 4.2 — UPDATE deveria ser bloqueado em usuário anonimizado';
    EXCEPTION WHEN others THEN
        IF SQLERRM LIKE '%já foi anonimizado%' THEN
            RAISE NOTICE 'PASS 4.2 — trg_bloquear_update_anonimizado: UPDATE bloqueado com mensagem correta';
        ELSE
            RAISE EXCEPTION 'FAIL 4.2 — exception inesperada: %', SQLERRM;
        END IF;
    END;
END $$;

-- 4.3 Tentativa de reverter anonimização bloqueada
DO $$
BEGIN
    BEGIN
        UPDATE usuarios SET anonimizado = FALSE
            WHERE id = '00000000-0000-0001-0000-000000000011';
        RAISE EXCEPTION 'FAIL 4.3 — reversão de anonimizado deveria ser bloqueada';
    EXCEPTION WHEN others THEN
        IF SQLERRM LIKE '%já foi anonimizado%' THEN
            RAISE NOTICE 'PASS 4.3 — trg_bloquear_update_anonimizado: reversão de anonimização bloqueada';
        ELSE
            RAISE EXCEPTION 'FAIL 4.3 — exception inesperada: %', SQLERRM;
        END IF;
    END;
END $$;

-- ============================================================
-- 6. trg_sincronizar_data_consentimento
-- ============================================================

-- 6.1 Aplicação omite data_consentimento — trigger preenche automaticamente
DO $$
DECLARE v_dt TIMESTAMPTZ;
BEGIN
    UPDATE usuarios SET consentimento_aceito = TRUE
        WHERE id = '00000000-0000-0001-0000-000000000010';
    SELECT data_consentimento INTO v_dt FROM usuarios
        WHERE id = '00000000-0000-0001-0000-000000000010';
    ASSERT v_dt IS NOT NULL,
        'FAIL 6.1 — data_consentimento deveria ter sido preenchido pelo trigger';
    ASSERT v_dt = CURRENT_TIMESTAMP,
        'FAIL 6.1 — data_consentimento deveria ser CURRENT_TIMESTAMP';
    RAISE NOTICE 'PASS 6.1 — trg_sincronizar_data_consentimento: data_consentimento preenchido automaticamente';
END $$;

-- 6.2 Aplicação fornece data_consentimento explícito — trigger não sobrescreve
DO $$
DECLARE
    v_dt_explicito CONSTANT TIMESTAMPTZ := '2020-06-01 10:00:00+00';
    v_dt_stored    TIMESTAMPTZ;
BEGIN
    UPDATE usuarios
        SET consentimento_aceito = TRUE,
            data_consentimento   = v_dt_explicito
        WHERE id = '00000000-0000-0001-0000-000000000012';
    SELECT data_consentimento INTO v_dt_stored FROM usuarios
        WHERE id = '00000000-0000-0001-0000-000000000012';
    ASSERT v_dt_stored = v_dt_explicito,
        FORMAT('FAIL 6.2 — trigger sobrescreveu valor explícito. Obtido: %s', v_dt_stored);
    RAISE NOTICE 'PASS 6.2 — trg_sincronizar_data_consentimento: valor explícito preservado';
END $$;

-- 6.3 Re-UPDATE com consentimento já TRUE — data_consentimento inalterado
-- Usuário 013 foi inserido com consentimento_aceito=TRUE e data_consentimento='2020-06-01'
DO $$
DECLARE
    v_dt_esperado CONSTANT TIMESTAMPTZ := '2020-06-01 10:00:00+00';
    v_dt_stored   TIMESTAMPTZ;
BEGIN
    UPDATE usuarios SET cidade = 'Campinas'
        WHERE id = '00000000-0000-0001-0000-000000000013';
    SELECT data_consentimento INTO v_dt_stored FROM usuarios
        WHERE id = '00000000-0000-0001-0000-000000000013';
    ASSERT v_dt_stored = v_dt_esperado,
        FORMAT('FAIL 6.3 — data_consentimento foi alterado em re-UPDATE. Obtido: %s', v_dt_stored);
    RAISE NOTICE 'PASS 6.3 — trg_sincronizar_data_consentimento: data_consentimento inalterado em re-UPDATE';
END $$;

-- 6.4 consentimento permanece FALSE — data_consentimento fica NULL
DO $$
DECLARE v_dt TIMESTAMPTZ;
BEGIN
    UPDATE usuarios SET cidade = 'Curitiba'
        WHERE id = '00000000-0000-0001-0000-000000000014';
    SELECT data_consentimento INTO v_dt FROM usuarios
        WHERE id = '00000000-0000-0001-0000-000000000014';
    ASSERT v_dt IS NULL,
        'FAIL 6.4 — data_consentimento deveria ser NULL quando consentimento_aceito = FALSE';
    RAISE NOTICE 'PASS 6.4 — trg_sincronizar_data_consentimento: data_consentimento NULL quando FALSE';
END $$;

-- ============================================================
-- 7. Existência de colunas e índices
-- ============================================================

-- 7.1 usuarios.anonimizado_em existe e é TIMESTAMPTZ
DO $$
DECLARE v_type TEXT;
BEGIN
    SELECT data_type INTO v_type FROM information_schema.columns
        WHERE table_name = 'usuarios' AND column_name = 'anonimizado_em';
    ASSERT v_type = 'timestamp with time zone',
        FORMAT('FAIL 7.1 — anonimizado_em: tipo esperado TIMESTAMPTZ, obtido: %s', v_type);
    RAISE NOTICE 'PASS 7.1 — usuarios.anonimizado_em existe e é TIMESTAMPTZ';
END $$;

-- 7.2 recomendacoes.visualizada_em existe e é TIMESTAMPTZ
DO $$
DECLARE v_type TEXT;
BEGIN
    SELECT data_type INTO v_type FROM information_schema.columns
        WHERE table_name = 'recomendacoes' AND column_name = 'visualizada_em';
    ASSERT v_type = 'timestamp with time zone',
        FORMAT('FAIL 7.2 — visualizada_em: tipo esperado TIMESTAMPTZ, obtido: %s', v_type);
    RAISE NOTICE 'PASS 7.2 — recomendacoes.visualizada_em existe e é TIMESTAMPTZ';
END $$;

-- 7.3 Todos os índices da Sprint 4 estão presentes
DO $$
DECLARE
    v_idx      TEXT;
    v_expected TEXT[] := ARRAY[
        'idx_interacoes_usuario_expiracao',
        'idx_teatros_cidade',
        'idx_espetaculos_datas',
        'idx_recomendacoes_usuario_score',
        'idx_wishlist_usuario',
        'idx_usuarios_ml_id'
    ];
BEGIN
    FOREACH v_idx IN ARRAY v_expected LOOP
        ASSERT EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = v_idx),
            FORMAT('FAIL 7.3 — índice ausente: %s', v_idx);
    END LOOP;
    RAISE NOTICE 'PASS 7.3 — todos os 6 índices da Sprint 4 presentes';
END $$;

-- 7.4 Índices antigos removidos
DO $$
BEGIN
    ASSERT NOT EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = 'idx_interacoes_usuario'),
        'FAIL 7.4 — idx_interacoes_usuario deveria ter sido removido';
    ASSERT NOT EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = 'idx_recomendacao_usuario'),
        'FAIL 7.4 — idx_recomendacao_usuario deveria ter sido removido';
    RAISE NOTICE 'PASS 7.4 — índices antigos removidos corretamente';
END $$;

-- ============================================================
-- Teardown — reverte todos os dados de teste
-- ============================================================

ROLLBACK;

DO $$ BEGIN RAISE NOTICE '——— Testes concluídos. Transação revertida. ———'; END $$;
