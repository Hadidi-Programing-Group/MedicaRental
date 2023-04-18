using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicaRental.DAL.Migrations
{
    public partial class AddRatingForClients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Clients");
        }
    }
}
