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
            _ = migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "species",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2);

            _ = migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "species",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<string>(
                name: "file_list",
                table: "pets",
                type: "text",
                nullable: true);

            _ = migrationBuilder.AddColumn<int>(
                name: "position",
                table: "pets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "breeds",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2);

            _ = migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "breeds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "species");

            _ = migrationBuilder.DropColumn(
                name: "file_list",
                table: "pets");

            _ = migrationBuilder.DropColumn(
                name: "position",
                table: "pets");

            _ = migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "breeds");

            _ = migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "species",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            _ = migrationBuilder.AlterColumn<string>(
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
