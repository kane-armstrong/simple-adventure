﻿using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetDoctor.Infrastructure;
using Respawn;
using SqlStreamStore;

namespace PetDoctor.API.Tests.Functional.Setup
{
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

            var streamStore = scope.ServiceProvider.GetService<MsSqlStreamStore>();
            var schemaCheck = streamStore.CheckSchema().GetAwaiter().GetResult();
            if (!schemaCheck.IsMatch())
                streamStore.CreateSchema().GetAwaiter().GetResult();

            await Checkpoint.Reset(ConnectionString);
        }
    }
}
