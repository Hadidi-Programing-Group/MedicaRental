using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicaRental.DAL.Migrations
{
    public partial class UpdateColumnNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Clients_ReporteeId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "ReporteeId",
                table: "Reports",
                newName: "ReporterId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ReporteeId",
                table: "Reports",
                newName: "IX_Reports_ReporterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Clients_ReporterId",
                table: "Reports",
                column: "ReporterId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Clients_ReporterId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "ReporterId",
                table: "Reports",
                newName: "ReporteeId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ReporterId",
                table: "Reports",
                newName: "IX_Reports_ReporteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Clients_ReporteeId",
                table: "Reports",
                column: "ReporteeId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
