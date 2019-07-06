using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace cloudscribe.PwaKit.Storage.EFCore.MySql.Migrations
{
    public partial class cspwakitinitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cspwa_PushSubscription",
                columns: table => new
                {
                    Key = table.Column<Guid>(nullable: false),
                    Endpoint = table.Column<string>(nullable: true),
                    P256DH = table.Column<string>(nullable: true),
                    Auth = table.Column<string>(nullable: true),
                    TenantId = table.Column<string>(maxLength: 50, nullable: true),
                    UserId = table.Column<string>(maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(nullable: true),
                    CreatedFromIpAddress = table.Column<string>(nullable: true),
                    CreatedUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cspwa_PushSubscription", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cspwa_PushSubscription_Endpoint",
                table: "cspwa_PushSubscription",
                column: "Endpoint");

            migrationBuilder.CreateIndex(
                name: "IX_cspwa_PushSubscription_TenantId",
                table: "cspwa_PushSubscription",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_cspwa_PushSubscription_UserId",
                table: "cspwa_PushSubscription",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cspwa_PushSubscription");
        }
    }
}
