using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathwayPlatform.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourseProgresses_CourseItems_CourseItemId",
                table: "StudentCourseProgresses");

            migrationBuilder.DropTable(
                name: "CourseItems");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_CourseId",
                table: "Recommendations",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recommendations_CourseId",
                table: "Recommendations");

            migrationBuilder.CreateTable(
                name: "CourseItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseItems_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseItems_CourseId",
                table: "CourseItems",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourseProgresses_CourseItems_CourseItemId",
                table: "StudentCourseProgresses",
                column: "CourseItemId",
                principalTable: "CourseItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
