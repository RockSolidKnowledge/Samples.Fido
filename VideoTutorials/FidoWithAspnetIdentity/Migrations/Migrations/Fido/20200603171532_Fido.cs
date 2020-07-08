using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrations.Migrations.Fido
{
    public partial class Fido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FidoKeys",
                columns: table => new
                {
                    CredentialId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    UserHandle = table.Column<string>(nullable: true),
                    DisplayFriendlyName = table.Column<string>(nullable: true),
                    AttestationType = table.Column<int>(nullable: false),
                    AuthenticatorId = table.Column<string>(nullable: true),
                    AuthenticatorIdType = table.Column<int>(nullable: true),
                    Counter = table.Column<int>(nullable: false),
                    KeyType = table.Column<string>(nullable: true),
                    Algorithm = table.Column<string>(nullable: true),
                    CredentialAsJson = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: true),
                    LastUsed = table.Column<DateTime>(nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FidoKeys");
        }
    }
}
