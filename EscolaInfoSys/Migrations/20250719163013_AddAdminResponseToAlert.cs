using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscolaInfoSys.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminResponseToAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminResponse",
                table: "Alerts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminResponse",
                table: "Alerts");
        }
    }
}
