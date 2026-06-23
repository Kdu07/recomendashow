using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Infrastructure;
using BackEnd.Aventura.Infrastructure.Persistence;
using Npgsql;

namespace BackEnd.Aventura.Repository
{
    public class EspetaculoRepository : IEspetaculoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EspetaculoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public IReadOnlyList<Espetaculo> GetAll() => Query($"{EspetaculoQueries.SelectColumns}{EspetaculoQueries.FromTable} ORDER BY e.data_inicio");

        public Espetaculo? GetById(Guid id)
        {
            EntityGuards.NotEmpty(id, nameof(id));
            var items = Query(
                $"{EspetaculoQueries.SelectColumns}{EspetaculoQueries.FromTable} WHERE e.id = @Id",
                new NpgsqlParameter("@Id", id));
            return items.FirstOrDefault();
        }

        public IReadOnlyList<Espetaculo> GetByIds(IReadOnlyList<Guid> ids)
        {
            if (ids.Count == 0)
            {
                return Array.Empty<Espetaculo>();
            }

            return Query(
                $"{EspetaculoQueries.SelectColumns}{EspetaculoQueries.FromTable} WHERE e.id = ANY(@Ids) ORDER BY e.data_inicio",
                new NpgsqlParameter("@Ids", ids.ToArray()));
        }

        public IReadOnlyList<Espetaculo> GetByGenero(string genero)
        {
            EntityGuards.NotNullOrWhiteSpace(genero, nameof(genero));

            var sql = $@"
{EspetaculoQueries.SelectColumns}
{EspetaculoQueries.FromTable}
WHERE EXISTS (
    SELECT 1
    FROM espetaculo_genero eg
    JOIN generos g ON g.id = eg.genero_id
    WHERE eg.espetaculo_id = e.id
      AND LOWER(g.nome) = LOWER(@Genero)
)
OR LOWER(e.tipo_evento) = LOWER(@Genero)
ORDER BY e.data_inicio";

            return Query(sql, new NpgsqlParameter("@Genero", genero));
        }

        public IReadOnlyList<Espetaculo> GetByGenerosFavoritos(Guid usuarioId, int limit)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));

            var sql = $@"
{EspetaculoQueries.SelectColumns}
{EspetaculoQueries.FromTable}
WHERE EXISTS (
    SELECT 1
    FROM espetaculo_genero eg
    JOIN generos g ON g.id = eg.genero_id
    JOIN usuario_genero_favorito ugf ON ugf.genero_id = g.id
    WHERE eg.espetaculo_id = e.id
      AND ugf.usuario_id = @UsuarioId
)
ORDER BY e.data_inicio
LIMIT @Limit";

            return Query(
                sql,
                new NpgsqlParameter("@UsuarioId", usuarioId),
                new NpgsqlParameter("@Limit", limit));
        }

        public IReadOnlyList<Espetaculo> GetByGenerosDaWishlist(Guid usuarioId, int limit)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));

            var sql = $@"
{EspetaculoQueries.SelectColumns}
{EspetaculoQueries.FromTable}
WHERE EXISTS (
    SELECT 1
    FROM espetaculo_genero eg
    JOIN espetaculo_genero eg_wish ON eg_wish.genero_id = eg.genero_id
    JOIN wishlist_curtidas w ON w.espetaculo_id = eg_wish.espetaculo_id
    WHERE eg.espetaculo_id = e.id
      AND w.usuario_id = @UsuarioId
      AND w.tipo = 'wishlist'
)
AND NOT EXISTS (
    SELECT 1
    FROM wishlist_curtidas w2
    WHERE w2.espetaculo_id = e.id
      AND w2.usuario_id = @UsuarioId
      AND w2.tipo = 'wishlist'
)
ORDER BY e.data_inicio
LIMIT @Limit";

            return Query(
                sql,
                new NpgsqlParameter("@UsuarioId", usuarioId),
                new NpgsqlParameter("@Limit", limit));
        }

        public bool Exists(Guid id)
        {
            EntityGuards.NotEmpty(id, nameof(id));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"SELECT EXISTS(SELECT 1 FROM espetaculos WHERE id = @Id)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteScalar() is bool exists && exists;
        }

        private IReadOnlyList<Espetaculo> Query(string sql, params NpgsqlParameter[] parameters)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddRange(parameters);

            using var reader = command.ExecuteReader();
            return EspetaculoMapper.MapAll(reader);
        }
    }
}
