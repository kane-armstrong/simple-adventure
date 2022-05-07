using Microsoft.EntityFrameworkCore;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Infrastructure.EntityTypeConfigurations;

namespace PetDoctor.Infrastructure;

public class PetDoctorContext : DbContext
{
    public DbSet<AppointmentSnapshot> AppointmentSnapshots { get; set; } = null!;

    public PetDoctorContext(DbContextOptions<PetDoctorContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AppointmentSnapshotEntityTypeConfiguration());
    }
}