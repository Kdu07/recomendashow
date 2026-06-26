using BackEnd.Aventura.Infrastructure;
using Npgsql;

namespace BackEnd.Aventura.Services
{
    public sealed class AnonimizacaoJob : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AnonimizacaoJob> _logger;

        public AnonimizacaoJob(IServiceScopeFactory scopeFactory, ILogger<AnonimizacaoJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await AnonimizarPendentesAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Erro ao executar job de anonimização LGPD.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }

        private async Task AnonimizarPendentesAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();

            using var connection = connectionFactory.CreateConnection();
            connection.Open();

            var ids = await BuscarPendentesAsync(connection, cancellationToken);

            if (ids.Count == 0)
            {
                return;
            }

            _logger.LogInformation("Job LGPD: {Count} conta(s) a anonimizar.", ids.Count);

            foreach (var id in ids)
            {
                await AnonimizarAsync(connection, id, cancellationToken);
            }

            _logger.LogInformation("Job LGPD: anonimização concluída.");
        }

        private static async Task<List<Guid>> BuscarPendentesAsync(
            System.Data.IDbConnection connection,
            CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT id
FROM usuarios
WHERE deleted_at IS NOT NULL
  AND deleted_at <= NOW() - INTERVAL '15 days'
  AND anonimizado = FALSE";

            using var command = new NpgsqlCommand(sql, (NpgsqlConnection)connection);
            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            var ids = new List<Guid>();
            while (await reader.ReadAsync(cancellationToken))
            {
                ids.Add(reader.GetGuid(0));
            }

            return ids;
        }

        private static async Task AnonimizarAsync(
            System.Data.IDbConnection connection,
            Guid id,
            CancellationToken cancellationToken)
        {
            const string sql = @"CALL sp_anonimizar_usuario(@Id)";

            using var command = new NpgsqlCommand(sql, (NpgsqlConnection)connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
