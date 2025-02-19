using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class updateitems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Buyer",
                table: "CurrentStock");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "OrderQueue",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ImageLink",
                table: "CurrentStock",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "OrderQueue");

            migrationBuilder.DropColumn(
                name: "ImageLink",
                table: "CurrentStock");

            migrationBuilder.AddColumn<string>(
                name: "Buyer",
                table: "CurrentStock",
                type: "longtext",
                nullable: true);
        }
    }
}
