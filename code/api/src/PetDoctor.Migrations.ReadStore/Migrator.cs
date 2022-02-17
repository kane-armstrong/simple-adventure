using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PetDoctor.Infrastructure;

namespace PetDoctor.Migrations.ReadStore
{
    public class Migrator : IHostedService
    {
        private readonly PetDoctorContext _context;
        private readonly IHostApplicationLifetime _lifetime;

        public Migrator(PetDoctorContext context, IHostApplicationLifetime lifetime)
        {
            _context = context;
            _lifetime = lifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _context.Database.MigrateAsync(cancellationToken);
            _lifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
