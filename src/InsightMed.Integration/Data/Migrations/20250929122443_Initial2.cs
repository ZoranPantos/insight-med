using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightMed.Integration.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabReports_LabRequests_LabRequestId",
                table: "LabReports");

            migrationBuilder.DropIndex(
                name: "IX_LabReports_LabRequestId",
                table: "LabReports");

            migrationBuilder.AlterColumn<int>(
                name: "LabRequestId",
                table: "LabReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_LabReports_LabRequestId",
                table: "LabReports",
                column: "LabRequestId",
                unique: true,
                filter: "[LabRequestId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_LabReports_LabRequests_LabRequestId",
                table: "LabReports",
                column: "LabRequestId",
                principalTable: "LabRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabReports_LabRequests_LabRequestId",
                table: "LabReports");

            migrationBuilder.DropIndex(
                name: "IX_LabReports_LabRequestId",
                table: "LabReports");

            migrationBuilder.AlterColumn<int>(
                name: "LabRequestId",
                table: "LabReports",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabReports_LabRequestId",
                table: "LabReports",
                column: "LabRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LabReports_LabRequests_LabRequestId",
                table: "LabReports",
                column: "LabRequestId",
                principalTable: "LabRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
