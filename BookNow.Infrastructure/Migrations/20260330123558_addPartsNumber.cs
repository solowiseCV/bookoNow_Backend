using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookNow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addPartsNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "Products");
        }
    }
}
