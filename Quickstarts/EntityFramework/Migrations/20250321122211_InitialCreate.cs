using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FidoKeys",
                columns: table => new
                {
                    CredentialId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    UserHandle = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayFriendlyName = table.Column<string>(type: "TEXT", nullable: true),
                    AttestationType = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthenticatorId = table.Column<string>(type: "TEXT", nullable: true),
                    AuthenticatorIdType = table.Column<int>(type: "INTEGER", nullable: true),
                    Counter = table.Column<int>(type: "INTEGER", nullable: false),
                    KeyType = table.Column<string>(type: "TEXT", nullable: true),
                    Algorithm = table.Column<string>(type: "TEXT", nullable: true),
                    CredentialAsJson = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastUsed = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidoKeys", x => x.CredentialId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FidoKeys_UserId",
                table: "FidoKeys",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FidoKeys");
        }
    }
}
