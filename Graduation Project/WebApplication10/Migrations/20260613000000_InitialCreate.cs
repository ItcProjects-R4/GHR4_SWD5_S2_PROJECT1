using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathwayPlatform.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Users ─────────────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id        = table.Column<long>(type: "bigint", nullable: false)
                                    .Annotation("SqlServer:Identity", "1, 1"),
                    Name      = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email     = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Password  = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role      = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name:    "IX_Users_Email",
                table:   "Users",
                column:  "Email",
                unique:  true);

            // ── StudentProfiles ───────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    Id             = table.Column<long>(type: "bigint", nullable: false)
                                         .Annotation("SqlServer:Identity", "1, 1"),
                    UserId         = table.Column<long>(type: "bigint", nullable: false),
                    FullName       = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Bio            = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PhoneNumber    = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Address        = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DateOfBirth    = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender         = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    University     = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Faculty        = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Major          = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    AcademicYear   = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Level          = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GPA            = table.Column<double>(type: "float", nullable: true),
                    Skills         = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LinkedInUrl    = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    GithubUrl      = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PortfolioUrl   = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CVUrl          = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProfileImageUrl= table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt      = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt      = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.Id);
                    table.ForeignKey(
                        name:            "FK_StudentProfiles_Users_UserId",
                        column:          x => x.UserId,
                        principalTable:  "Users",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name:   "IX_StudentProfiles_UserId",
                table:  "StudentProfiles",
                column: "UserId",
                unique: true);

            // ── StudentAnalytics ──────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "StudentAnalytics",
                columns: table => new
                {
                    Id                    = table.Column<long>(type: "bigint", nullable: false)
                                               .Annotation("SqlServer:Identity", "1, 1"),
                    UserId                = table.Column<long>(type: "bigint", nullable: false),
                    TotalCoursesCompleted = table.Column<int>(type: "int", nullable: false),
                    CompletionRate        = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StrengthAreas         = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeaknessAreas         = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt             = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnalytics", x => x.Id);
                    table.ForeignKey(
                        name:            "FK_StudentAnalytics_Users_UserId",
                        column:          x => x.UserId,
                        principalTable:  "Users",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name:   "IX_StudentAnalytics_UserId",
                table:  "StudentAnalytics",
                column: "UserId",
                unique: true);

            // ── Initiatives ───────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Initiatives",
                columns: table => new
                {
                    Id          = table.Column<long>(type: "bigint", nullable: false)
                                      .Annotation("SqlServer:Identity", "1, 1"),
                    Name        = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Initiatives", x => x.Id);
                });

            // ── Tracks ────────────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id           = table.Column<long>(type: "bigint", nullable: false)
                                       .Annotation("SqlServer:Identity", "1, 1"),
                    InitiativeId = table.Column<long>(type: "bigint", nullable: false),
                    Name         = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description  = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type         = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                    table.ForeignKey(
                        name:            "FK_Tracks_Initiatives_InitiativeId",
                        column:          x => x.InitiativeId,
                        principalTable:  "Initiatives",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name:   "IX_Tracks_InitiativeId",
                table:  "Tracks",
                column: "InitiativeId");

            // ── Courses ───────────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id          = table.Column<long>(type: "bigint", nullable: false)
                                      .Annotation("SqlServer:Identity", "1, 1"),
                    TrackId     = table.Column<long>(type: "bigint", nullable: false),
                    Title       = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ContentUrl  = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Duration    = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceType  = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt   = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name:            "FK_Courses_Tracks_TrackId",
                        column:          x => x.TrackId,
                        principalTable:  "Tracks",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name:   "IX_Courses_TrackId",
                table:  "Courses",
                column: "TrackId");

            // ── CourseItems ───────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "CourseItems",
                columns: table => new
                {
                    Id          = table.Column<long>(type: "bigint", nullable: false)
                                      .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId    = table.Column<long>(type: "bigint", nullable: false),
                    Title       = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OrderIndex  = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseItems", x => x.Id);
                    table.ForeignKey(
                        name:            "FK_CourseItems_Courses_CourseId",
                        column:          x => x.CourseId,
                        principalTable:  "Courses",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name:   "IX_CourseItems_CourseId",
                table:  "CourseItems",
                column: "CourseId");

            // ── Enrollments ───────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id                 = table.Column<long>(type: "bigint", nullable: false)
                                            .Annotation("SqlServer:Identity", "1, 1"),
                    UserId             = table.Column<long>(type: "bigint", nullable: false),
                    CourseId           = table.Column<long>(type: "bigint", nullable: false),
                    EnrolledAt         = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Status             = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "not-started"),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name:            "FK_Enrollments_Users_UserId",
                        column:          x => x.UserId,
                        principalTable:  "Users",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:            "FK_Enrollments_Courses_CourseId",
                        column:          x => x.CourseId,
                        principalTable:  "Courses",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name:   "IX_Enrollments_UserId_CourseId",
                table:  "Enrollments",
                columns: new[] { "UserId", "CourseId" },
                unique: true);

            // ── StudentCourseProgresses ───────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "StudentCourseProgresses",
                columns: table => new
                {
                    Id           = table.Column<long>(type: "bigint", nullable: false)
                                       .Annotation("SqlServer:Identity", "1, 1"),
                    UserId       = table.Column<long>(type: "bigint", nullable: false),
                    CourseItemId = table.Column<long>(type: "bigint", nullable: false),
                    IsCompleted  = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt  = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourseProgresses", x => x.Id);
                    table.ForeignKey(
                        name:            "FK_StudentCourseProgresses_Users_UserId",
                        column:          x => x.UserId,
                        principalTable:  "Users",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:            "FK_StudentCourseProgresses_CourseItems_CourseItemId",
                        column:          x => x.CourseItemId,
                        principalTable:  "CourseItems",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name:    "IX_StudentCourseProgresses_UserId_CourseItemId",
                table:   "StudentCourseProgresses",
                columns: new[] { "UserId", "CourseItemId" },
                unique:  true);

            // ── Recommendations ───────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Recommendations",
                columns: table => new
                {
                    Id        = table.Column<long>(type: "bigint", nullable: false)
                                    .Annotation("SqlServer:Identity", "1, 1"),
                    UserId    = table.Column<long>(type: "bigint", nullable: false),
                    CourseId  = table.Column<long>(type: "bigint", nullable: false),
                    Type      = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Reason    = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Score     = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.Id);
                    table.ForeignKey(
                        name:            "FK_Recommendations_Users_UserId",
                        column:          x => x.UserId,
                        principalTable:  "Users",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:            "FK_Recommendations_Courses_CourseId",
                        column:          x => x.CourseId,
                        principalTable:  "Courses",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name:    "IX_Recommendations_UserId_CourseId",
                table:   "Recommendations",
                columns: new[] { "UserId", "CourseId" });

            // ── CourseRequests ────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "CourseRequests",
                columns: table => new
                {
                    Id                = table.Column<long>(type: "bigint", nullable: false)
                                           .Annotation("SqlServer:Identity", "1, 1"),
                    UserId            = table.Column<long>(type: "bigint", nullable: false),
                    TrackId           = table.Column<long>(type: "bigint", nullable: true),
                    GeneratedCourseId = table.Column<long>(type: "bigint", nullable: true),
                    Title             = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description       = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ContentUrl        = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Duration          = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderName      = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProviderEmail     = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status            = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "pending"),
                    CreatedAt         = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseRequests", x => x.Id);
                    table.ForeignKey(
                        name:            "FK_CourseRequests_Users_UserId",
                        column:          x => x.UserId,
                        principalTable:  "Users",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:            "FK_CourseRequests_Tracks_TrackId",
                        column:          x => x.TrackId,
                        principalTable:  "Tracks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name:            "FK_CourseRequests_Courses_GeneratedCourseId",
                        column:          x => x.GeneratedCourseId,
                        principalTable:  "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name:   "IX_CourseRequests_UserId",
                table:  "CourseRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name:   "IX_CourseRequests_TrackId",
                table:  "CourseRequests",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name:   "IX_CourseRequests_GeneratedCourseId",
                table:  "CourseRequests",
                column: "GeneratedCourseId");

            // ── CourseValidationLogs ──────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "CourseValidationLogs",
                columns: table => new
                {
                    Id              = table.Column<long>(type: "bigint", nullable: false)
                                         .Annotation("SqlServer:Identity", "1, 1"),
                    CourseRequestId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewerId      = table.Column<long>(type: "bigint", nullable: false),
                    Decision        = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Comment         = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReviewedAt      = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseValidationLogs", x => x.Id);
                    table.ForeignKey(
                        name:            "FK_CourseValidationLogs_CourseRequests_CourseRequestId",
                        column:          x => x.CourseRequestId,
                        principalTable:  "CourseRequests",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:            "FK_CourseValidationLogs_Users_ReviewerId",
                        column:          x => x.ReviewerId,
                        principalTable:  "Users",
                        principalColumn: "Id",
                        onDelete:        ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name:   "IX_CourseValidationLogs_CourseRequestId",
                table:  "CourseValidationLogs",
                column: "CourseRequestId");

            migrationBuilder.CreateIndex(
                name:   "IX_CourseValidationLogs_ReviewerId",
                table:  "CourseValidationLogs",
                column: "ReviewerId");

            // ── Opportunities ─────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    Id             = table.Column<long>(type: "bigint", nullable: false)
                                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title          = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Type           = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description    = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiredSkills = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Link           = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Source         = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt      = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CourseValidationLogs");
            migrationBuilder.DropTable(name: "CourseRequests");
            migrationBuilder.DropTable(name: "StudentCourseProgresses");
            migrationBuilder.DropTable(name: "Recommendations");
            migrationBuilder.DropTable(name: "Enrollments");
            migrationBuilder.DropTable(name: "CourseItems");
            migrationBuilder.DropTable(name: "Courses");
            migrationBuilder.DropTable(name: "Tracks");
            migrationBuilder.DropTable(name: "Initiatives");
            migrationBuilder.DropTable(name: "StudentAnalytics");
            migrationBuilder.DropTable(name: "StudentProfiles");
            migrationBuilder.DropTable(name: "Opportunities");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
