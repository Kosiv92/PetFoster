using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFoster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PetPropertiesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropCheckConstraint(
                name: "CK_Volunteer_PhoneNumber_NumericOnly",
                table: "pets");

            _ = migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "pets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1)",
                oldMaxLength: 1);

            _ = migrationBuilder.AddCheckConstraint(
                name: "CK_Volunteer_PhoneNumber_NumericOnly",
                table: "pets",
                sql: "phone_number ~ '^[0-9]{11}$'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropCheckConstraint(
                name: "CK_Volunteer_PhoneNumber_NumericOnly",
                table: "pets");

            _ = migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "pets",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            _ = migrationBuilder.AddCheckConstraint(
                name: "CK_Volunteer_PhoneNumber_NumericOnly",
                table: "pets",
                sql: "phone_number ~ '^[0-9]11$'");
        }
    }
}
