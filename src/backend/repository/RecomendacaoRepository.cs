using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Infrastructure;
using BackEnd.Aventura.Infrastructure.Persistence;
using Npgsql;

namespace BackEnd.Aventura.Repository
{
    public class RecomendacaoRepository : IRecomendacaoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RecomendacaoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public IReadOnlyList<Recomendacao> GetByUsuario(Guid usuarioId, int limit)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"
SELECT r.score, r.motivo,
       e.id, e.nome, e.tipo_evento, e.descricao, e.url_bilheteria,
       e.data_inicio, e.data_fim, e.valor_ingresso,
       COALESCE(
           (SELECT g.nome
            FROM espetaculo_genero eg
            JOIN generos g ON g.id = eg.genero_id
            WHERE eg.espetaculo_id = e.id
            LIMIT 1),
           e.tipo_evento
       ) AS genero
FROM recomendacoes r
JOIN espetaculos e ON e.id = r.espetaculo_id
WHERE r.usuario_id = @UsuarioId
ORDER BY r.score DESC
LIMIT @Limit";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            command.Parameters.AddWithValue("@Limit", limit);

            using var reader = command.ExecuteReader();
            var result = new List<Recomendacao>();
            while (reader.Read())
            {
                var espetaculo = EspetaculoMapper.Map(reader);
                var score = reader.GetDecimal(reader.GetOrdinal("score"));
                var motivo = reader.IsDBNull(reader.GetOrdinal("motivo"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("motivo"));

                result.Add(new Recomendacao(espetaculo, score, motivo));
            }

            return result;
        }
    }
}
