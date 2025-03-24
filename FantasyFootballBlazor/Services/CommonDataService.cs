using FantasyFootball.Shared;
using FantasyFootball.Shared.Services;
using FantasyFootballBlazor.Data;
using Microsoft.EntityFrameworkCore;

namespace FantasyFootballBlazor.Services
{
    public class CommonDataService : ICommonDataService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ITimeProvider _datetime;

        public CommonDataService(IDbContextFactory<ApplicationDbContext> dbContextFactory, ITimeProvider datetime)
        {
            _dbContextFactory = dbContextFactory;
            _datetime = datetime;
        }


        /// <summary>
        /// Returns list of positions for players.
        /// </summary>
        /// <returns>
        /// A dropdown list of available player positions.
        /// </returns>
        public List<DropdownModel> GetPlayerPositions()
        {
            // ✅ Predefined list of player positions as dropdown options
            var positions = new List<DropdownModel>
            {
                new DropdownModel {Text = "QB", Value = "QB"},  // Quarterback
                new DropdownModel {Text = "RB", Value = "RB"},  // Running Back
                new DropdownModel {Text = "WR", Value = "WR"},  // Wide Receiver
                new DropdownModel {Text = "TE", Value = "TE"},  // Tight End
                new DropdownModel {Text = "K", Value = "K"},    // Kicker
                new DropdownModel {Text = "DEF", Value = "DEF"} // Defense
            };

            // ✅ Return the view model with available positions
            return positions;
        }

        /// <summary>
        /// Asynchronously retrieves the most recent NFL year from the database.
        /// </summary>
        /// <remarks>
        /// This method queries the <c>NflYears</c> table and returns the most recent
        /// year based on the highest <c>NflYearId</c>. It assumes that <c>NflYearId</c>
        /// corresponds to the chronological order of NFL years.
        /// </remarks>
        /// <returns>The most recent NFL year available in the database.</returns>
        public async Task<int> GetCurrentYearAsync()
        {
            await using var context = _dbContextFactory.CreateDbContext();
            var currentYear = await context.NflYears
                .AsNoTracking()
                .OrderByDescending(x => x.NflYearId) // Sorting by the highest ID to get the latest year
                .Select(x => x.NflYearId) // Selecting the Year ID as the current year
                .FirstOrDefaultAsync();

            return currentYear; // Returns the most recent year found
        }


        /// <summary>
        /// Asynchronously determines the current NFL week based on the current date and time.
        /// </summary>
        /// <remarks>
        /// - Adjusts for daylight savings time before evaluating the current date.
        /// - Considers an NFL week as "current" if the current date is at least two days past the start date.
        /// - Orders by the most recent year and week to find the latest active week.
        /// </remarks>
        /// <returns>
        /// The most recent NFL week number where the current date is at least two days past the start date.
        /// Returns <c>0</c> if no valid week is found.
        /// </returns>
        public async Task<int> GetCurrentWeekAsync()
        {
            await using var context = _dbContextFactory.CreateDbContext();
            var now = _datetime.Now.AddHours(DaylightSavingsAdjustment()); // Adjust for daylight savings

            var currentWeek = await context.NflWeeks
                .AsNoTracking()
                .Where(x => now >= x.StartDate.AddDays(-2)) // Consider week active if 2 days past start
                .OrderByDescending(x => x.Year) // Sort by latest year first
                .ThenByDescending(x => x.Name) // Within the year, sort by latest week
                .Select(x => (int?)x.Name) // Cast to nullable int for safety
                .FirstOrDefaultAsync();

            return currentWeek ?? 0; // Default to 0 if no valid week is found
        }

        /// <summary>
        /// Determines the daylight savings adjustment for Central Standard Time (CST).
        /// </summary>
        /// <remarks>
        /// - Checks whether the current date falls under Daylight Saving Time (DST) in the Central Time Zone.
        /// - CST follows UTC-6 during standard time and UTC-5 during daylight saving time.
        /// </remarks>
        /// <returns>
        /// An integer offset:
        /// - Returns <c>-5</c> if daylight saving time is active (CDT - Central Daylight Time).
        /// - Returns <c>-6</c> if standard time is active (CST - Central Standard Time).
        /// </returns>
        public int DaylightSavingsAdjustment()
        {
            var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var currentTime = DateTime.Now;

            return centralTimeZone.IsDaylightSavingTime(currentTime) ? -5 : -6;
        }

        /// <summary>
        /// Asynchronously retrieves a list of players that a user has picked for a given week and year.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="week">The week number for which the picks are retrieved.</param>
        /// <param name="year">The year for which the picks are retrieved.</param>
        /// <param name="gameTypeId">The game type identifier (e.g., Weekly, Survivor).</param>
        /// <returns>
        /// A <see cref="Task{List{PlayerModel}}"/> representing the asynchronous operation, containing 
        /// the list of players the user has selected for the given week and game type.
        /// </returns>
        public async Task<List<PlayerModel>> GetUsersWeeklyPicksAsync(string userId, int week, int year, int gameTypeId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Fetch the user's weekly picks from the database
            var myPicks = await context.WeeklyUserTeams
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.Week == week &&
                    x.Year == year &&
                    x.GameTypeId == gameTypeId)
                .ToListAsync();

            // Retrieve player data for the selected picks
            var playerData = await context.Players
                .Include(p => p.WeeklyStats) // Include weekly stats for each player
                .AsNoTracking()
                .Where(x => myPicks.Select(x => x.PlayerId).Contains(x.PlayerId)) // Filter players based on picks
                .ToListAsync();

            // Map database entities to PlayerModel objects
            var players = playerData.Select(x => new PlayerModel
            {
                First = x.First,
                FullName = x.FullName,
                Last = x.Position == "DEF" ? "" : x.Last, // Defensive players do not have a last name
                LastUpdatedDateTime = x.LastUpdatedDateTime,
                PlayerId = x.PlayerId,
                PlayerPictureImageUrl = x.PlayerPictureImageUrl,
                Position = x.Position,
                TeamId = x.TeamId,
                TotalPoints = x.WeeklyStats
                    .Where(w => w.Week == week && w.Year == year)
                    .Sum(w => w.TotalPoints)
                    .GetValueOrDefault() // Ensure null safety
            }).ToList();

            return players;
        }

        /// <summary>
        /// Asynchronously retrieves the lock status of all NFL teams for a given week and year.
        /// </summary>
        /// <param name="week">The week number for which to retrieve team lock statuses.</param>
        /// <param name="year">The year for which to retrieve team lock statuses.</param>
        /// <returns>
        /// A <see cref="Task{List{TeamLockStatusModel}}"/> containing a list of team lock statuses,
        /// including whether each team is locked and their scheduled opponents.
        /// </returns>
        public async Task<List<TeamLockStatusModel>> GetTeamLockStatusListAsync(int week, int year)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Retrieve the schedule for the given week and year
            var schedule = await context.TeamSchedules
                .AsNoTracking()
                .Where(x => x.Week == week && x.Year == year)
                .ToListAsync();

            // Determine the current time adjusted for daylight savings
            var now = _datetime.Now.AddHours(DaylightSavingsAdjustment());
            var teamLockStatuses = new List<TeamLockStatusModel>();

            // Iterate through scheduled games to determine lock status
            foreach (var nflSchedule in schedule)
            {
                // Lock teams 5 minutes before game start time
                var lockTime = nflSchedule.GameStartDateTime.AddMinutes(-5);
                var isLocked = now >= lockTime;

                // Add home team lock status
                teamLockStatuses.Add(new TeamLockStatusModel
                {
                    IsLocked = isLocked,
                    ScheduleId = nflSchedule.TeamScheduleId,
                    TeamId = nflSchedule.HomeTeamId,
                    OpponentId = nflSchedule.AwayTeamId
                });

                // Add away team lock status
                teamLockStatuses.Add(new TeamLockStatusModel
                {
                    IsLocked = isLocked,
                    ScheduleId = nflSchedule.TeamScheduleId,
                    TeamId = nflSchedule.AwayTeamId,
                    OpponentId = nflSchedule.HomeTeamId
                });
            }

            // Identify teams that are on a bye week (not in the schedule)
            var byeWeekIds = await context.Teams
                .AsNoTracking()
                .Where(x => !teamLockStatuses.Select(y => y.TeamId).Contains(x.TeamId))
                .Select(x => x.TeamId)
                .ToListAsync();

            // Mark bye-week teams as locked
            foreach (var teamId in byeWeekIds)
            {
                teamLockStatuses.Add(new TeamLockStatusModel
                {
                    IsLocked = true, // Bye-week teams are automatically locked
                    ScheduleId = 0, // No game scheduled
                    TeamId = teamId
                });
            }

            return teamLockStatuses;
        }

    }
}
