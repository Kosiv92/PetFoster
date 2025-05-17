using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFoster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "volunteers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "pets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "volunteers");

            _ = migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "pets");
        }
    }
}
