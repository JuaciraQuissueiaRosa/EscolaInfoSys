using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscolaInfoSys.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseToFormGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "FormGroups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormGroups_CourseId",
                table: "FormGroups",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormGroups_Courses_CourseId",
                table: "FormGroups",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormGroups_Courses_CourseId",
                table: "FormGroups");

            migrationBuilder.DropIndex(
                name: "IX_FormGroups_CourseId",
                table: "FormGroups");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "FormGroups");
        }
    }
}
