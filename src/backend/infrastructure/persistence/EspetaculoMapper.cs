using BackEnd.Aventura.Domain;
using Npgsql;

namespace BackEnd.Aventura.Infrastructure.Persistence
{
    internal static class EspetaculoMapper
    {
        internal static Espetaculo Map(NpgsqlDataReader reader) =>
            new(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("nome")),
                reader.GetString(reader.GetOrdinal("genero")),
                reader.GetString(reader.GetOrdinal("tipo_evento")),
                reader.GetString(reader.GetOrdinal("descricao")),
                reader.GetString(reader.GetOrdinal("url_bilheteria")),
                reader.GetDateTime(reader.GetOrdinal("data_inicio")),
                reader.GetDateTime(reader.GetOrdinal("data_fim")),
                reader.GetDecimal(reader.GetOrdinal("valor_ingresso")));

        internal static List<Espetaculo> MapAll(NpgsqlDataReader reader)
        {
            var items = new List<Espetaculo>();
            while (reader.Read())
            {
                items.Add(Map(reader));
            }

            return items;
        }
    }
}
