using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightMed.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NotificationUserFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequesterId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RequesterId",
                table: "Notifications",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_RequesterId",
                table: "Notifications",
                column: "RequesterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_RequesterId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_RequesterId",
                table: "Notifications");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequesterId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
