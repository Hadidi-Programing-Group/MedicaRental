using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicaRental.DAL.Migrations
{
    public partial class AddReportActionsTakenBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "ReportActions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ReportActions_AdminId",
                table: "ReportActions",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportActions_AspNetUsers_AdminId",
                table: "ReportActions",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportActions_AspNetUsers_AdminId",
                table: "ReportActions");

            migrationBuilder.DropIndex(
                name: "IX_ReportActions_AdminId",
                table: "ReportActions");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "ReportActions");
        }
    }
}
