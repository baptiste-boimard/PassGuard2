using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassGuard.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFKAccountIDonEntityObjectPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "ObjectPasswords",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ObjectPasswords_AccountId",
                table: "ObjectPasswords",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectPasswords_Accounts_AccountId",
                table: "ObjectPasswords",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectPasswords_Accounts_AccountId",
                table: "ObjectPasswords");

            migrationBuilder.DropIndex(
                name: "IX_ObjectPasswords_AccountId",
                table: "ObjectPasswords");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "ObjectPasswords");
        }
    }
}
