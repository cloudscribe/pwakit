using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace cloudscribe.PwaKit.Storage.EFCore.PostgreSql.Migrations
{
    public partial class cspwakitinitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cspwa_push_subscription",
                columns: table => new
                {
                    key = table.Column<Guid>(nullable: false),
                    endpoint = table.Column<string>(nullable: true),
                    p256_dh = table.Column<string>(nullable: true),
                    auth = table.Column<string>(nullable: true),
                    tenant_id = table.Column<string>(maxLength: 50, nullable: true),
                    user_id = table.Column<string>(maxLength: 50, nullable: true),
                    user_agent = table.Column<string>(nullable: true),
                    created_from_ip_address = table.Column<string>(nullable: true),
                    created_utc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cspwa_push_subscription", x => x.key);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cspwa_push_subscription_endpoint",
                table: "cspwa_push_subscription",
                column: "endpoint");

            migrationBuilder.CreateIndex(
                name: "ix_cspwa_push_subscription_tenant_id",
                table: "cspwa_push_subscription",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_cspwa_push_subscription_user_id",
                table: "cspwa_push_subscription",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cspwa_push_subscription");
        }
    }
}
