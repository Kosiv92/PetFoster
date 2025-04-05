using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFoster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PetFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "species",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "species",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "file_list",
                table: "pets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "position",
                table: "pets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "breeds",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "breeds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "species");

            migrationBuilder.DropColumn(
                name: "file_list",
                table: "pets");

            migrationBuilder.DropColumn(
                name: "position",
                table: "pets");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "breeds");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "species",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "breeds",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }
    }
}
