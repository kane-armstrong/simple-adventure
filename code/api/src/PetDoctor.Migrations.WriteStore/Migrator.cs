using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SqlStreamStore;

namespace PetDoctor.Migrations.WriteStore
{
    public class Migrator : IHostedService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly DatabaseConnectionOptions _connectionOptions;
        private readonly MsSqlStreamStoreV3 _streamStore;

        public Migrator(
            MsSqlStreamStoreV3 streamStore,
            IHostApplicationLifetime lifetime,
            IOptions<DatabaseConnectionOptions> options)
        {
            _streamStore = streamStore;
            _lifetime = lifetime;
            _connectionOptions = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await CreateDatabaseIfNotExists(cancellationToken);
            await _streamStore.CreateSchemaIfNotExists(cancellationToken);
            _lifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task CreateDatabaseIfNotExists(CancellationToken cancellationToken)
        {
            var name = _connectionOptions.ApplicationDatabaseName;
            var sql = $"if not exists(select * from sys.databases where [name] = @name) begin create database {name} end";
            await using var connection = new SqlConnection(_connectionOptions.Master);
            await connection.OpenAsync(cancellationToken);
            var command = new CommandDefinition(sql, new { name }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }
}
