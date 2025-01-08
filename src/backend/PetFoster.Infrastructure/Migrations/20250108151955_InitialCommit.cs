using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFoster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "species",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "volunteers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    work_expirience = table.Column<int>(type: "integer", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: false),
                    assistance_requisites_list = table.Column<string>(type: "text", nullable: false),
                    social_net_contacts = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    last_name = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    patronymic = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_volunteers", x => x.id);
                    table.CheckConstraint("CK_Volunteer_Email_ValidFormat", "email ~* '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$'");
                    table.CheckConstraint("CK_Volunteer_PhoneNumber_NumericOnly", "phone_number ~ '^[0-9]11$'");
                    table.CheckConstraint("CK_Volunteer_WorkExperience_NonNegative", "work_expirience >= 0");
                });

            migrationBuilder.CreateTable(
                name: "breeds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    specie_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_breeds", x => x.id);
                    table.ForeignKey(
                        name: "fk_breeds_species_specie_id",
                        column: x => x.specie_id,
                        principalTable: "species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    volunteer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    specie_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    breed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    coloration = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    health = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: false),
                    сastrated = table.Column<bool>(type: "boolean", nullable: false),
                    birth_day = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    vaccinated = table.Column<bool>(type: "boolean", nullable: false),
                    assistance_status = table.Column<string>(type: "text", nullable: false),
                    assistance_requisites_list = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apartment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    height = table.Column<double>(type: "double precision", nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pets", x => x.id);
                    table.CheckConstraint("CK_Pet_Height_NonNegative", "height >= 0");
                    table.CheckConstraint("CK_Pet_Weight_NonNegative", "weight >= 0");
                    table.CheckConstraint("CK_Volunteer_PhoneNumber_NumericOnly", "phone_number ~ '^[0-9]11$'");
                    table.ForeignKey(
                        name: "fk_pets_breeds_breed_id",
                        column: x => x.breed_id,
                        principalTable: "breeds",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pets_species_specie_id",
                        column: x => x.specie_id,
                        principalTable: "species",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pets_volunteers_volunteer_id",
                        column: x => x.volunteer_id,
                        principalTable: "volunteers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_breeds_specie_id",
                table: "breeds",
                column: "specie_id");

            migrationBuilder.CreateIndex(
                name: "ix_pets_breed_id",
                table: "pets",
                column: "breed_id");

            migrationBuilder.CreateIndex(
                name: "ix_pets_specie_id",
                table: "pets",
                column: "specie_id");

            migrationBuilder.CreateIndex(
                name: "ix_pets_volunteer_id",
                table: "pets",
                column: "volunteer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pets");

            migrationBuilder.DropTable(
                name: "breeds");

            migrationBuilder.DropTable(
                name: "volunteers");

            migrationBuilder.DropTable(
                name: "species");
        }
    }
}
