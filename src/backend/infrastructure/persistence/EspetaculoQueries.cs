namespace BackEnd.Aventura.Infrastructure.Persistence
{
    internal static class EspetaculoQueries
    {
        internal const string SelectColumns = @"
SELECT e.id, e.nome, e.tipo_evento, e.descricao, e.url_bilheteria,
       e.data_inicio, e.data_fim, e.valor_ingresso,
       COALESCE(
           (SELECT g.nome
            FROM espetaculo_genero eg
            JOIN generos g ON g.id = eg.genero_id
            WHERE eg.espetaculo_id = e.id
            LIMIT 1),
           e.tipo_evento
       ) AS genero";

        internal const string FromTable = " FROM espetaculos e ";
    }
}
