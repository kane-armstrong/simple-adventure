﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PetDoctor.Infrastructure;

namespace PetDoctor.API.Infrastructure.Migrations.PetDoctor.PetDoctorDb
{
    [DbContext(typeof(PetDoctorContext))]
    partial class PetDoctorContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PetDoctor.Domain.Aggregates.Appointments.AppointmentSnapshot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AttendingVeterinarianId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CancellationReason")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("ReasonForVisit")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("RejectionReason")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<DateTimeOffset>("ScheduledOn")
                        .HasColumnType("datetimeoffset(7)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("AppointmentSnapshots","dbo");
                });

            modelBuilder.Entity("PetDoctor.Domain.Aggregates.Appointments.AppointmentSnapshot", b =>
                {
                    b.OwnsOne("PetDoctor.Domain.Aggregates.Appointments.Owner", "Owner", b1 =>
                        {
                            b1.Property<Guid>("AppointmentSnapshotId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnName("OwnerEmail")
                                .HasColumnType("nvarchar(100)")
                                .HasMaxLength(100);

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasColumnName("OwnerFirstName")
                                .HasColumnType("nvarchar(100)")
                                .HasMaxLength(100);

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasColumnName("OwnerLastName")
                                .HasColumnType("nvarchar(100)")
                                .HasMaxLength(100);

                            b1.Property<string>("Phone")
                                .IsRequired()
                                .HasColumnName("OwnerPhone")
                                .HasColumnType("nvarchar(25)")
                                .HasMaxLength(25);

                            b1.HasKey("AppointmentSnapshotId");

                            b1.ToTable("AppointmentSnapshots");

                            b1.WithOwner()
                                .HasForeignKey("AppointmentSnapshotId");
                        });

                    b.OwnsOne("PetDoctor.Domain.Aggregates.Appointments.Pet", "Pet", b1 =>
                        {
                            b1.Property<Guid>("AppointmentSnapshotId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Breed")
                                .IsRequired()
                                .HasColumnName("PetBreed")
                                .HasColumnType("nvarchar(100)")
                                .HasMaxLength(100);

                            b1.Property<DateTimeOffset>("DateOfBirth")
                                .HasColumnName("PetDateOfBirth")
                                .HasColumnType("datetimeoffset(7)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnName("PetName")
                                .HasColumnType("nvarchar(100)")
                                .HasMaxLength(100);

                            b1.HasKey("AppointmentSnapshotId");

                            b1.ToTable("AppointmentSnapshots");

                            b1.WithOwner()
                                .HasForeignKey("AppointmentSnapshotId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}