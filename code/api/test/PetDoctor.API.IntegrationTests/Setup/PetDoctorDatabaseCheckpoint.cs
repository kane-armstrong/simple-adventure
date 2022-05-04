using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetDoctor.Infrastructure;
using Respawn;
using SqlStreamStore;

namespace PetDoctor.API.IntegrationTests.Setup;

class PetDoctorDatabaseCheckpoint
{
    private static readonly Checkpoint Checkpoint = new Checkpoint
    {
        TablesToIgnore = new[]
        {
            "__EFMigrationsHistory"
        }
    };

    private static readonly string ConnectionString = TestResources.Configuration.GetConnectionString("PetDoctorContext");

    private static bool _initialized;

    public static async Task Reset()
    {
        using var scope = TestResources.ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PetDoctorContext>();
        if (!_initialized)
        {
            await dbContext.Database.MigrateAsync();
            _initialized = true;
        }

        var streamStore = scope.ServiceProvider.GetRequiredService<MsSqlStreamStoreV3>();
        var schemaCheck = await streamStore.CheckSchema();
        if (!schemaCheck.IsMatch())
            await streamStore.CreateSchemaIfNotExists();

        await Checkpoint.Reset(ConnectionString);
    }
}