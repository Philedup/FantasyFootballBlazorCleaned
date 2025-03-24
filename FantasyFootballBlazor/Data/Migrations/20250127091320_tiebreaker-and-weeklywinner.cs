using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyFootballBlazor.Migrations
{
    /// <inheritdoc />
    public partial class tiebreakerandweeklywinner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyWinner_AspNetUsers_UserId",
                table: "WeeklyWinner");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeeklyWinner",
                table: "WeeklyWinner");

            migrationBuilder.RenameTable(
                name: "WeeklyWinner",
                newName: "WeeklyWinners");

            migrationBuilder.RenameIndex(
                name: "IX_WeeklyWinner_UserId",
                table: "WeeklyWinners",
                newName: "IX_WeeklyWinners_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeeklyWinners",
                table: "WeeklyWinners",
                column: "WeeklyWinnerId");

            migrationBuilder.CreateTable(
                name: "UserTieBreakers",
                columns: table => new
                {
                    UserTieBreakerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTieBreakers", x => x.UserTieBreakerId);
                    table.ForeignKey(
                        name: "FK_UserTieBreakers_TeamSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "TeamSchedules",
                        principalColumn: "TeamScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTieBreakers_ScheduleId",
                table: "UserTieBreakers",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyWinners_AspNetUsers_UserId",
                table: "WeeklyWinners",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyWinners_AspNetUsers_UserId",
                table: "WeeklyWinners");

            migrationBuilder.DropTable(
                name: "UserTieBreakers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeeklyWinners",
                table: "WeeklyWinners");

            migrationBuilder.RenameTable(
                name: "WeeklyWinners",
                newName: "WeeklyWinner");

            migrationBuilder.RenameIndex(
                name: "IX_WeeklyWinners_UserId",
                table: "WeeklyWinner",
                newName: "IX_WeeklyWinner_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeeklyWinner",
                table: "WeeklyWinner",
                column: "WeeklyWinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyWinner_AspNetUsers_UserId",
                table: "WeeklyWinner",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
