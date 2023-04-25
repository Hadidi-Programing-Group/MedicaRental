using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicaRental.DAL.Migrations
{
    public partial class ChangingTheReviewConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentOperations_Reviews_ReviewId",
                table: "RentOperations");

            migrationBuilder.DropIndex(
                name: "IX_RentOperations_ReviewId",
                table: "RentOperations");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RentOperationId",
                table: "Reviews",
                column: "RentOperationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_RentOperations_RentOperationId",
                table: "Reviews",
                column: "RentOperationId",
                principalTable: "RentOperations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_RentOperations_RentOperationId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_RentOperationId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_RentOperations_ReviewId",
                table: "RentOperations",
                column: "ReviewId",
                unique: true,
                filter: "[ReviewId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_RentOperations_Reviews_ReviewId",
                table: "RentOperations",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
