using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightMed.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationRequesterId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequesterId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "Notifications");
        }
    }
}
