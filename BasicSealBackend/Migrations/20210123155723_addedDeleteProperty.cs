using Microsoft.EntityFrameworkCore.Migrations;

namespace BasicSealBackend.Migrations
{
    public partial class addedDeleteProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "SoftwareVersions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Softwares",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "SoftwareLicenses",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "SoftwareVersions");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "SoftwareLicenses");
        }
    }
}
