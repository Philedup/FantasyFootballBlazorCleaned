//make sure in server project (this one)
//1. Add a New Migration
//Add-Migration <MigrationName> 
//2. Update Database
//Update-Database
//if we need to remove migration
// Remove-Migration


using FantasyFootballBlazor.Data.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FantasyFootballBlazor.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Alert> Alerts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<GameType> GameTypes { get; set; }
        public DbSet<LastStatUpdate> LastStatUpdates { get; set; }
        public DbSet<LeagueData> LeagueData { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamSchedule> TeamSchedules { get; set; }


        public DbSet<Player> Players { get; set; }
        public DbSet<PrizePot> PrizePots { get; set; }
        public DbSet<NflWeek> NflWeeks { get; set; }
        public DbSet<NflYear> NflYears { get; set; }

        public DbSet<UserTieBreaker> UserTieBreakers { get; set; }

        public DbSet<WeeklyStat> WeeklyStats { get; set; }
        public DbSet<WeeklyUserTeam> WeeklyUserTeams { get; set; }
        public DbSet<WeeklyWinner> WeeklyWinners { get; set; }
        public DbSet<YahooToken> YahooTokens { get; set; }
        public DbSet<YearEndResults> YearEndResults { get; set; }

    }
}
