using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicaRental.DAL.Migrations
{
    public partial class RemoveExtraAdsFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ads",
                table: "Items");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ads",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
