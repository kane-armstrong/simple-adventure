using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetDoctor.ReadStore.Migrations.Migrations
{
    public partial class InitialMigration : Migration
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PetDateOfBirth = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                    PetBreed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OwnerFirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OwnerLastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OwnerPhone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    OwnerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AttendingVeterinarianId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReasonForVisit = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ScheduledOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
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
