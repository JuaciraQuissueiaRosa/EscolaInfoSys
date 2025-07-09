using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscolaInfoSys.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalLessonsToSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalLessons",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalLessons",
                table: "Subjects");
        }
    }
}
