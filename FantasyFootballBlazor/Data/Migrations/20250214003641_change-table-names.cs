using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyFootballBlazor.Migrations
{
    /// <inheritdoc />
    public partial class changetablenames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NFLYear",
                table: "NFLYear");

            migrationBuilder.RenameTable(
                name: "NFLYear",
                newName: "NflYears");

            migrationBuilder.RenameColumn(
                name: "WeekId",
                table: "NflWeeks",
                newName: "NflWeekId");

            migrationBuilder.RenameColumn(
                name: "YearId",
                table: "NflYears",
                newName: "NflYearId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NflYears",
                table: "NflYears",
                column: "NflYearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NflYears",
                table: "NflYears");

            migrationBuilder.RenameTable(
                name: "NflYears",
                newName: "NFLYear");

            migrationBuilder.RenameColumn(
                name: "NflWeekId",
                table: "NflWeeks",
                newName: "WeekId");

            migrationBuilder.RenameColumn(
                name: "NflYearId",
                table: "NFLYear",
                newName: "YearId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NFLYear",
                table: "NFLYear",
                column: "YearId");
        }
    }
}
