using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyFootballBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUSerPlayerWeeklyStatsAndWeeklyUserTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserTeamName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Paid = table.Column<bool>(type: "bit", nullable: true),
                    Survival = table.Column<bool>(type: "bit", nullable: true),
                    PaypalEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Admin = table.Column<bool>(type: "bit", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    First = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Last = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    PlayerPictureImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LastUpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyStats",
                columns: table => new
                {
                    WeeklyStatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PassingYards = table.Column<int>(type: "int", nullable: true),
                    PassingTouchdowns = table.Column<int>(type: "int", nullable: true),
                    PassingInterceptions = table.Column<int>(type: "int", nullable: true),
                    PassingSacks = table.Column<int>(type: "int", nullable: true),
                    RushingYards = table.Column<int>(type: "int", nullable: true),
                    RushingTouchdowns = table.Column<int>(type: "int", nullable: true),
                    ReceptionYards = table.Column<int>(type: "int", nullable: true),
                    ReceptionTouchdowns = table.Column<int>(type: "int", nullable: true),
                    ReturnYards = table.Column<int>(type: "int", nullable: true),
                    ReturnTouchdowns = table.Column<int>(type: "int", nullable: true),
                    TwoPointConversions = table.Column<int>(type: "int", nullable: true),
                    FumblesLost = table.Column<int>(type: "int", nullable: true),
                    FieldGoal1 = table.Column<int>(type: "int", nullable: true),
                    FieldGoal2 = table.Column<int>(type: "int", nullable: true),
                    FieldGoal3 = table.Column<int>(type: "int", nullable: true),
                    FieldGoal4 = table.Column<int>(type: "int", nullable: true),
                    FieldGoal5 = table.Column<int>(type: "int", nullable: true),
                    FieldGoalMiss1 = table.Column<int>(type: "int", nullable: true),
                    FieldGoalMiss2 = table.Column<int>(type: "int", nullable: true),
                    PointsAfterMade = table.Column<int>(type: "int", nullable: true),
                    PointsAfterMiss = table.Column<int>(type: "int", nullable: true),
                    PointsAllowed0 = table.Column<int>(type: "int", nullable: true),
                    PointsAllowed1 = table.Column<int>(type: "int", nullable: true),
                    PointsAllowed2 = table.Column<int>(type: "int", nullable: true),
                    PointsAllowed3 = table.Column<int>(type: "int", nullable: true),
                    PointsAllowed4 = table.Column<int>(type: "int", nullable: true),
                    PointsAllowed5 = table.Column<int>(type: "int", nullable: true),
                    PointsAllowed6 = table.Column<int>(type: "int", nullable: true),
                    DefensePointsAllowed = table.Column<int>(type: "int", nullable: true),
                    DefenseSacks = table.Column<int>(type: "int", nullable: true),
                    DefenseInterceptions = table.Column<int>(type: "int", nullable: true),
                    DefenseFumbleRecoveries = table.Column<int>(type: "int", nullable: true),
                    DefenseTouchdowns = table.Column<int>(type: "int", nullable: true),
                    DefenseSafeties = table.Column<int>(type: "int", nullable: true),
                    DefenseBlockedKicks = table.Column<int>(type: "int", nullable: true),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    TotalPoints = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyStats", x => x.WeeklyStatId);
                    table.ForeignKey(
                        name: "FK_WeeklyStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyUserTeams",
                columns: table => new
                {
                    WeeklyUserTeamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Week = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GameTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyUserTeams", x => x.WeeklyUserTeamId);
                    table.ForeignKey(
                        name: "FK_WeeklyUserTeams_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyUserTeams_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyStats_PlayerId",
                table: "WeeklyStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyUserTeams_PlayerId",
                table: "WeeklyUserTeams",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyUserTeams_UserId",
                table: "WeeklyUserTeams",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeeklyStats");

            migrationBuilder.DropTable(
                name: "WeeklyUserTeams");

            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
