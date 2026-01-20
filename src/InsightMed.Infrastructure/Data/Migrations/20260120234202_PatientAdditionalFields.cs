using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightMed.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class PatientAdditionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DietType",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExerciseLevel",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "HeightCm",
                table: "Patients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SmokingStatus",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "WeightKg",
                table: "Patients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DietType",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ExerciseLevel",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "HeightCm",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "SmokingStatus",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "Patients");
        }
    }
}
