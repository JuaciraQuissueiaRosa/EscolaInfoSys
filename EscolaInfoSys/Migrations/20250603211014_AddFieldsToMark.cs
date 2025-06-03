using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscolaInfoSys.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToMark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EvaluationType",
                table: "Marks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffMemberId",
                table: "Marks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Marks_StaffMemberId",
                table: "Marks",
                column: "StaffMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Marks_StaffMembers_StaffMemberId",
                table: "Marks",
                column: "StaffMemberId",
                principalTable: "StaffMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Marks_StaffMembers_StaffMemberId",
                table: "Marks");

            migrationBuilder.DropIndex(
                name: "IX_Marks_StaffMemberId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "EvaluationType",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "StaffMemberId",
                table: "Marks");
        }
    }
}
