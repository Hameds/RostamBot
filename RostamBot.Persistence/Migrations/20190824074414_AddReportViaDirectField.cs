using Microsoft.EntityFrameworkCore.Migrations;

namespace RostamBot.Persistence.Migrations
{
    public partial class AddReportViaDirectField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsViaDirect",
                table: "SuspiciousAccountReports",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsViaDirect",
                table: "SuspiciousAccountReports");
        }
    }
}
