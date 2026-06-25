INSERT INTO generos(nome)
VALUES
('Comédia'),
('Drama'),
('Musical'),
('Infantil'),
('Suspense'),
('Romance')
ON CONFLICT(nome)
DO NOTHING;


INSERT INTO teatros(
nome,
endereco,
cidade,
horario_funcionamento
)
VALUES
(
'Teatro Municipal',
'Praça Ramos de Azevedo',
'São Paulo',
'08:00 - 22:00'
),

(
'Teatro Frei Caneca',
'Rua Frei Caneca',
'São Paulo',
'10:00 - 23:00'
)
ON CONFLICT(nome)
DO NOTHING;


INSERT INTO usuarios(
nome,
email,
senha_hash,
idade,
consentimento_aceito,
data_consentimento
)
VALUES
(
'Alexsander Barbosa',
'alex@email.com',
'$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi',
21,
TRUE,
'2026-06-01 10:00:00'
),

(
'Maria Silva',
'maria@email.com',
'$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi',
25,
TRUE,
'2026-06-01 10:00:00'
),

(
'João Santos',
'joao@email.com',
'$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi',
30,
TRUE,
'2026-06-01 10:00:00'
)
ON CONFLICT(email)
DO NOTHING;


INSERT INTO espetaculos(
nome,
tipo_evento,
descricao,
url_bilheteria,
data_inicio,
data_fim,
valor_ingresso
)
VALUES
(
'O Fantasma da Ópera',
'Musical',
'Musical clássico',
'https://ingressos.com',
'2026-06-10 19:00',
'2026-06-10 22:00',
120
),

(
'Hamlet',
'Drama',
'Peça inspirada em Shakespeare',
'https://ingressos.com',
'2026-06-15 20:00',
'2026-06-15 22:30',
70
)
ON CONFLICT(nome)
DO NOTHING;


INSERT INTO espetaculo_genero
VALUES
(
(
SELECT id
FROM espetaculos
WHERE nome='O Fantasma da Ópera'
),

(
SELECT id
FROM generos
WHERE nome='Musical'
)
),

(
(
SELECT id
FROM espetaculos
WHERE nome='Hamlet'
),

(
SELECT id
FROM generos
WHERE nome='Drama'
)
)
ON CONFLICT DO NOTHING;


INSERT INTO teatro_espetaculo
VALUES
(
(
SELECT id
FROM teatros
WHERE nome='Teatro Municipal'
),

(
SELECT id
FROM espetaculos
WHERE nome='O Fantasma da Ópera'
)
),

(
(
SELECT id
FROM teatros
WHERE nome='Teatro Frei Caneca'
),

(
SELECT id
FROM espetaculos
WHERE nome='Hamlet'
)
)
ON CONFLICT DO NOTHING;


INSERT INTO preferencias(
usuario_id,
cidade
)
VALUES
(
(
SELECT id
FROM usuarios
WHERE email='alex@email.com'
),

'São Paulo'
)
ON CONFLICT(usuario_id)
DO NOTHING;


INSERT INTO usuario_genero_favorito
VALUES
(
(
SELECT id
FROM usuarios
WHERE email='alex@email.com'
),

(
SELECT id
FROM generos
WHERE nome='Musical'
)
)
ON CONFLICT DO NOTHING;


INSERT INTO wishlist_curtidas(usuario_id, espetaculo_id, tipo)
VALUES
(
(
SELECT id
FROM usuarios
WHERE email='alex@email.com'
),

(
SELECT id
FROM espetaculos
WHERE nome='O Fantasma da Ópera'
),

'curtida'
),

(
(
SELECT id
FROM usuarios
WHERE email='maria@email.com'
),

(
SELECT id
FROM espetaculos
WHERE nome='Hamlet'
),

'wishlist'
)
ON CONFLICT DO NOTHING;


INSERT INTO interacoes(
usuario_id,
espetaculo_id,
tipo,
pontuacao
)
VALUES
(
(
SELECT id
FROM usuarios
WHERE email='alex@email.com'
),

(
SELECT id
FROM espetaculos
WHERE nome='O Fantasma da Ópera'
),

'Clique',
9.0
),

(
(
SELECT id
FROM usuarios
WHERE email='maria@email.com'
),

(
SELECT id
FROM espetaculos
WHERE nome='Hamlet'
),

'Acesso',
7.5
)
ON CONFLICT DO NOTHING;


INSERT INTO recomendacoes(
usuario_id,
espetaculo_id,
score,
motivo
)
VALUES
(
(
SELECT id
FROM usuarios
WHERE email='alex@email.com'
),

(
SELECT id
FROM espetaculos
WHERE nome='Hamlet'
),

0.93,

'Usuário gosta de musicais e dramas'
)
ON CONFLICT DO NOTHING;