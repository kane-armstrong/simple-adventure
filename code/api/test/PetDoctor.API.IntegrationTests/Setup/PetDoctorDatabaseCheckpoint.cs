using System.Threading.Tasks;
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
        var dbContext = scope.ServiceProvider.GetService<PetDoctorContext>();
        if (!_initialized)
        {
            dbContext.Database.Migrate();
            _initialized = true;
        }

        var streamStore = scope.ServiceProvider.GetService<MsSqlStreamStoreV3>();
        var schemaCheck = streamStore.CheckSchema().GetAwaiter().GetResult();
        if (!schemaCheck.IsMatch())
            streamStore.CreateSchemaIfNotExists().GetAwaiter().GetResult();

        await Checkpoint.Reset(ConnectionString);
    }
}