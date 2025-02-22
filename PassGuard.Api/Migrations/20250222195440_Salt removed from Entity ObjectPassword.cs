using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassGuard.Api.Migrations
{
    /// <inheritdoc />
    public partial class SaltremovedfromEntityObjectPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "ObjectPasswords");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "ObjectPasswords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
