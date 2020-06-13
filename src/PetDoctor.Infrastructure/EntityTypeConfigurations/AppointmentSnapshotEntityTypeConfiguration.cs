using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetDoctor.Domain.Aggregates.Appointments;

namespace PetDoctor.Infrastructure.EntityTypeConfigurations
{
    public class AppointmentSnapshotEntityTypeConfiguration : IEntityTypeConfiguration<AppointmentSnapshot>
    {
        public void Configure(EntityTypeBuilder<AppointmentSnapshot> builder)
        {
            builder.ToTable("AppointmentSnapshots", "dbo");

            builder.OwnsOne(p => p.Pet)
                .Property(p => p.Name)
                .HasColumnName("PetName")
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsOne(p => p.Pet)
                .Property(p => p.Breed)
                .HasColumnName("PetBreed")
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsOne(p => p.Pet)
                .Property(p => p.DateOfBirth)
                .HasColumnName("PetDateOfBirth")
                .HasColumnType("datetimeoffset(7)")
                .IsRequired();

            builder.OwnsOne(p => p.Owner)
                .Property(p => p.FirstName)
                .HasColumnName("OwnerFirstName")
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsOne(p => p.Owner)
                .Property(p => p.LastName)
                .HasColumnName("OwnerLastName")
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsOne(p => p.Owner)
                .Property(p => p.Phone)
                .HasColumnName("OwnerPhone")
                .HasMaxLength(25)
                .IsRequired();

            builder.OwnsOne(p => p.Owner)
                .Property(p => p.Email)
                .HasColumnName("OwnerEmail")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.AttendingVeterinarianId)
                .IsRequired(false);

            builder.Property(p => p.ReasonForVisit)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(p => p.ScheduledOn)
                .HasColumnType("datetimeoffset(7)");
        }
    }
}
