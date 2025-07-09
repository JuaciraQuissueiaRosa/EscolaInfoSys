using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscolaInfoSys.Migrations
{
    /// <inheritdoc />
    public partial class EditStaffMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocumentPhoto",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "StaffMembers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_ApplicationUserId",
                table: "StaffMembers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffMembers_AspNetUsers_ApplicationUserId",
                table: "StaffMembers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffMembers_AspNetUsers_ApplicationUserId",
                table: "StaffMembers");

            migrationBuilder.DropIndex(
                name: "IX_StaffMembers_ApplicationUserId",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "StaffMembers");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentPhoto",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
