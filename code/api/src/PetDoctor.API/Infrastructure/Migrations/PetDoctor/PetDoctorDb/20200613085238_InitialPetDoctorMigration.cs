using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PetDoctor.API.Infrastructure.Migrations.PetDoctor.PetDoctorDb
{
    public partial class InitialPetDoctorMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "AppointmentSnapshots",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PetName = table.Column<string>(maxLength: 100, nullable: true),
                    PetDateOfBirth = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                    PetBreed = table.Column<string>(maxLength: 100, nullable: true),
                    OwnerFirstName = table.Column<string>(maxLength: 100, nullable: true),
                    OwnerLastName = table.Column<string>(maxLength: 100, nullable: true),
                    OwnerPhone = table.Column<string>(maxLength: 25, nullable: true),
                    OwnerEmail = table.Column<string>(maxLength: 100, nullable: true),
                    AttendingVeterinarianId = table.Column<Guid>(nullable: true),
                    ReasonForVisit = table.Column<string>(maxLength: 1000, nullable: false),
                    RejectionReason = table.Column<string>(maxLength: 1000, nullable: true),
                    CancellationReason = table.Column<string>(maxLength: 1000, nullable: true),
                    ScheduledOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentSnapshots", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentSnapshots",
                schema: "dbo");
        }
    }
}
