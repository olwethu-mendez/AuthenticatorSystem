using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class moremultitenancytables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectRole",
                table: "ProfileProject",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OrganisationRole",
                table: "ProfileOrganisations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProjectImageName",
                table: "OrganisationProjects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProjectImageUrl",
                table: "OrganisationProjects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectRole",
                table: "ProfileProject");

            migrationBuilder.DropColumn(
                name: "OrganisationRole",
                table: "ProfileOrganisations");

            migrationBuilder.DropColumn(
                name: "ProjectImageName",
                table: "OrganisationProjects");

            migrationBuilder.DropColumn(
                name: "ProjectImageUrl",
                table: "OrganisationProjects");
        }
    }
}
