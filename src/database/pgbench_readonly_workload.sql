-- 1. Wishlist/curtidas por usuario.
SELECT *
FROM wishlist_curtidas
WHERE usuario_id = '606f6cae-1d85-46b0-aa6f-dc5fabad9037';

-- 2. Espetaculos por cidade.
SELECT e.*
FROM teatros t
JOIN teatro_espetaculo te ON te.teatro_id = t.id
JOIN espetaculos e ON e.id = te.espetaculo_id
WHERE t.cidade = 'São Paulo';

-- 3. Espetaculos por intervalo de datas.
SELECT *
FROM espetaculos
WHERE data_inicio >= CURRENT_DATE
  AND data_fim <= CURRENT_DATE + INTERVAL '90 days';

-- 4. Recomendacoes por usuario.
SELECT *
FROM recomendacoes
WHERE usuario_id = '606f6cae-1d85-46b0-aa6f-dc5fabad9037'
ORDER BY score DESC;
