using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscolaInfoSys.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_AspNetUsers_StaffMemberId",
                table: "Alerts");

            migrationBuilder.AlterColumn<int>(
                name: "StaffMemberId",
                table: "Alerts",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_StaffMembers_StaffMemberId",
                table: "Alerts",
                column: "StaffMemberId",
                principalTable: "StaffMembers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_StaffMembers_StaffMemberId",
                table: "Alerts");

            migrationBuilder.AlterColumn<string>(
                name: "StaffMemberId",
                table: "Alerts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_AspNetUsers_StaffMemberId",
                table: "Alerts",
                column: "StaffMemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
