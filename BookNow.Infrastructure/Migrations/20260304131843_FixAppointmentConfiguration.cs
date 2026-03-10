using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookNow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAppointmentConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentAttachments_Appointments_AppointmentId1",
                table: "AppointmentAttachments");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentAttachments_AppointmentId1",
                table: "AppointmentAttachments");

            migrationBuilder.DropColumn(
                name: "AppointmentId1",
                table: "AppointmentAttachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId1",
                table: "AppointmentAttachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentAttachments_AppointmentId1",
                table: "AppointmentAttachments",
                column: "AppointmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentAttachments_Appointments_AppointmentId1",
                table: "AppointmentAttachments",
                column: "AppointmentId1",
                principalTable: "Appointments",
                principalColumn: "Id");
        }
    }
}
