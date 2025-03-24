using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyFootballBlazor.Migrations
{
    /// <inheritdoc />
    public partial class yahooservicemodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LastStatUpdates",
                columns: table => new
                {
                    LastStatUpdateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastAuthKeyUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastStatUpdates", x => x.LastStatUpdateId);
                });

            migrationBuilder.CreateTable(
                name: "LeagueData",
                columns: table => new
                {
                    LeagueDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    YahooGameId = table.Column<int>(type: "int", nullable: false),
                    YahooLeagueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueData", x => x.LeagueDataId);
                });

            migrationBuilder.CreateTable(
                name: "PrizePots",
                columns: table => new
                {
                    PrizePotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Place = table.Column<int>(type: "int", nullable: false),
                    Prize = table.Column<int>(type: "int", nullable: false),
                    GameTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrizePots", x => x.PrizePotId);
                });

            migrationBuilder.CreateTable(
                name: "YahooToken",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastRefreshDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YahooToken", x => x.TokenId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LastStatUpdates");

            migrationBuilder.DropTable(
                name: "LeagueData");

            migrationBuilder.DropTable(
                name: "PrizePots");

            migrationBuilder.DropTable(
                name: "YahooToken");
        }
    }
}
