using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightMed.Integration.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabReports_Patients_PatientId",
                table: "LabReports");

            migrationBuilder.AddForeignKey(
                name: "FK_LabReports_Patients_PatientId",
                table: "LabReports",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabReports_Patients_PatientId",
                table: "LabReports");

            migrationBuilder.AddForeignKey(
                name: "FK_LabReports_Patients_PatientId",
                table: "LabReports",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
