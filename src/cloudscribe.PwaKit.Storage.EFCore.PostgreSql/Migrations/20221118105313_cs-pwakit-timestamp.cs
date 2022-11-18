using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cloudscribe.PwaKit.Storage.EFCore.PostgreSql.Migrations
{
    public partial class cspwakittimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
           @"DO $$
            BEGIN
                ALTER TABLE cspwa_push_subscription
                  ALTER created_utc TYPE timestamptz USING created_utc AT TIME ZONE 'UTC'
                , ALTER created_utc SET DEFAULT now();
             
            END$$;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
