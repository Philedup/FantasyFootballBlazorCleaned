using FantasyFootball.Shared;
using FantasyFootball.Shared.Services;
using FantasyFootballBlazor.Data;
using FantasyFootballBlazor.Data.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FantasyFootballBlazor.Services
{
    public class PlayersService : IPlayersService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ICommonDataService _commonDataService;

        public PlayersService(IDbContextFactory<ApplicationDbContext> dbContextFactory, ICommonDataService common)
        {
            _dbContextFactory = dbContextFactory;
            _commonDataService = common;
        }

        /// <summary>
        /// Creates an <see cref="AddPlayersViewModel"/> with filtered, sorted, and paged players,
        /// lock statuses, and the current player's info for the given user.
        /// </summary>
        /// <param name="positionFilter">Filters players by position (e.g., "QB", "RB").</param>
        /// <param name="playerNameFilter">Filters players whose name contains this value.</param>
        /// <param name="userId">User identifier to fetch user-specific data.</param>
        /// <param name="gameTypeId">Type of game being played.</param>
        /// <param name="year">The current year for stats retrieval.</param>
        /// <param name="week">The current week for stats retrieval.</param>
        /// <param name="sortColumn">Column name to sort by (default: "TotalPoints").</param>
        /// <param name="sortAscending">Sorting direction; false for descending, true for ascending.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of players per page.</param>
        /// <returns>An instance of <see cref="AddPlayersViewModel"/> containing the filtered, sorted, and paged player list.</returns>
        public async Task<AddPlayersViewModel> GetAllPlayersAsync(
            string positionFilter,
            string playerNameFilter,
            string userId,
            int gameTypeId,
            int year,
            int week,
            string sortColumn = "TotalPoints",
            bool sortAscending = false,
            int pageNumber = 1,
            int pageSize = 10)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // 1) Retrieve user's current picks for the specified week and year
            var userPicks = await _commonDataService.GetUsersWeeklyPicksAsync(userId, week, year, gameTypeId);

            // 2) Initialize the base query for fetching players, including their weekly stats
            var playersQuery = context.Players
                .Include(p => p.WeeklyStats)
                .AsNoTracking()
                .Where(p => p.LastUpdatedDateTime != null && p.TeamId > 0); // Ensure valid players only

            // 3) Apply Position Filter if specified
            if (!string.IsNullOrEmpty(positionFilter))
            {
                playersQuery = playersQuery.Where(p => p.Position == positionFilter);
            }

            // 4) Apply Name Filter if specified (case-insensitive)
            if (!string.IsNullOrEmpty(playerNameFilter))
            {
                string lowerCaseFilter = playerNameFilter.ToLower();
                playersQuery = playersQuery.Where(p => p.FullName.ToLower().Contains(lowerCaseFilter));
            }

            // 5) Fetch and map player data along with their stats for the specified year
            var players = await playersQuery
                .Select(x => new PlayerModel
                {
                    PlayerId = x.PlayerId,
                    Position = x.Position,
                    First = x.First,
                    FullName = x.FullName,
                    Last = x.Last,
                    TeamId = x.TeamId,
                    PlayerStats = x.WeeklyStats
                        .Where(y => y.Year == year)
                        .Select(s => new WeeklyStatModel
                        {
                            PassingYards = s.PassingYards,
                            PassingTouchdowns = s.PassingTouchdowns,
                            PassingInterceptions = s.PassingInterceptions,
                            RushingYards = s.RushingYards,
                            RushingTouchdowns = s.RushingTouchdowns,
                            FumblesLost = s.FumblesLost,
                            TwoPointConversions = s.TwoPointConversions,
                            ReceptionYards = s.ReceptionYards,
                            ReceptionTouchdowns = s.ReceptionTouchdowns,
                            ReturnYards = s.ReturnYards,
                            ReturnTouchdowns = s.ReturnTouchdowns,
                            DefensePointsAllowed = s.DefensePointsAllowed,
                            DefenseSacks = s.DefenseSacks,
                            DefenseInterceptions = s.DefenseInterceptions,
                            DefenseFumbleRecoveries = s.DefenseFumbleRecoveries,
                            DefenseSafeties = s.DefenseSafeties,
                            DefenseTouchdowns = s.DefenseTouchdowns,
                            DefenseBlockedKicks = s.DefenseBlockedKicks,
                            PointsAfterMade = s.PointsAfterMade,
                            PointsAfterMiss = s.PointsAfterMiss,
                            FieldGoal1 = s.FieldGoal1,
                            FieldGoal2 = s.FieldGoal2,
                            FieldGoal3 = s.FieldGoal3,
                            FieldGoal4 = s.FieldGoal4,
                            FieldGoal5 = s.FieldGoal5,
                            TotalPoints = s.TotalPoints
                        })
                        .ToList(),
                    TotalPoints = x.WeeklyStats
                        .Where(ws => ws.Year == year)
                        .Sum(ws => ws.TotalPoints) ?? 0
                })
                .ToListAsync();

            // 6) Fetch team lock statuses for the current week and year
            var teamLockStatuses = await _commonDataService.GetTeamLockStatusListAsync(week, year);
            var lockedTeams = teamLockStatuses.Where(x => x.IsLocked).Select(x => x.TeamId).ToList();

            // 7) Fetch all NFL teams (needed to determine opponent names)
            var nflTeams = await context.Teams.AsNoTracking().ToListAsync();

            // 8) Update player lock status and set their opponent's name
            GetPlayerLockStatusAndOpponent(players, lockedTeams, nflTeams, teamLockStatuses);

            // 9) Apply Sorting based on the specified column and direction
            players = sortAscending
                ? players.OrderBy(p => GetPropertyValue(p, sortColumn)).ToList()
                : players.OrderByDescending(p => GetPropertyValue(p, sortColumn)).ToList();

            // 10) Apply Pagination (calculate total count before paging)
            int totalPlayers = players.Count;
            players = players.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // 11) Construct and return the ViewModel
            return new AddPlayersViewModel
            {
                PlayerNameFilter = playerNameFilter,
                PositionFilter = positionFilter,
                Players = players,
                Positions = GetPlayerPositions(), // Retrieves the list of available player positions
                CurrentPlayerImg = userPicks.FirstOrDefault(x => x.Position == positionFilter)?.PlayerPictureImageUrl,
                CurrentPlayerName = userPicks.FirstOrDefault(x => x.Position == positionFilter)?.FullName,
                GameTypeId = gameTypeId,
                TotalPlayers = totalPlayers, // Used for pagination calculations
                PageSize = pageSize,
                CurrentPage = pageNumber
            };
        }

        /// <summary>
        /// Helper method to retrieve a property value dynamically from a <see cref="PlayerModel"/>.
        /// This is used to support dynamic sorting based on the property name.
        /// </summary>
        /// <param name="player">The <see cref="PlayerModel"/> instance.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <returns>The value of the specified property if found; otherwise, an empty string.</returns>
        private object GetPropertyValue(PlayerModel player, string propertyName)
        {
            // Retrieve the property from the PlayerModel type using reflection
            var property = typeof(PlayerModel).GetProperty(propertyName);

            // Return the property value if found, otherwise return an empty string
            return property?.GetValue(player) ?? "";
        }

        /// <summary>
        /// Asynchronously adds a player to the user's team based on the specified game type.
        /// </summary>
        /// <param name="userId">The ID of the user adding the player.</param>
        /// <param name="playerId">The ID of the player being added.</param>
        /// <param name="week">The current week in the season.</param>
        /// <param name="year">The current year of the game.</param>
        /// <param name="gameTypeId">The type of game (e.g., Weekly = 1, Survivor = 2).</param>
        /// <returns>Returns <c>true</c> if the player was successfully added; otherwise, <c>false</c> in case of an error.</returns>
        public async Task<bool> AddPlayerAsync(string userId, int playerId, int week, int year, int gameTypeId)
        {
            try
            {
                switch (gameTypeId)
                {
                    case 1: // Weekly Game Mode
                        await AddWeeklyUserPickAsync(playerId, week, year, userId);
                        break;
                    case 2: // Survivor Game Mode
                        await AddSurvivorUserPicksAsync(playerId, week, year, userId);
                        break;
                }

                return true; // Indicate successful operation
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Log the error message for debugging purposes
                return false; // Return false to indicate failure
            }
        }

        /// <summary>
        /// Updates player lock status and assigns opponent teams for each player.
        /// </summary>
        /// <param name="players">The list of players to update.</param>
        /// <param name="lockedTeams">A list of team IDs that are locked.</param>
        /// <param name="nflTeams">A list of all NFL teams.</param>
        /// <param name="teamLockStatuses">A list of team lock statuses, including opponent mappings.</param>
        private static void GetPlayerLockStatusAndOpponent(
            List<PlayerModel> players,
            List<int> lockedTeams,
            List<Team> nflTeams,
            List<TeamLockStatusModel> teamLockStatuses)
        {
            foreach (var player in players)
            {
                // Determine if the player's team is locked
                player.Locked = lockedTeams.Contains(player?.TeamId ?? 0);

                // Get the opponent ID from the team lock status list
                var opponentId = teamLockStatuses
                    .FirstOrDefault(x => x.TeamId == player.TeamId)
                    ?.OpponentId;

                // Retrieve the opponent's team code; default to "BYE" if no match is found
                var opponentTeam = nflTeams.FirstOrDefault(t => t.TeamId == opponentId)?.Code;
                player.Opponent = opponentTeam ?? "BYE";
            }
        }

        /// <summary>
        /// Asynchronously fetches players and their stats for a given year, with optional filtering by position.
        /// </summary>
        /// <param name="positionFilter">The position to filter players by (e.g., "QB", "RB", etc.). If null or empty, no position filtering is applied.</param>
        /// <param name="year">The year for which stats should be retrieved.</param>
        /// <returns>A list of <see cref="PlayerModel"/> objects containing player details and their filtered stats.</returns>
        private async Task<List<PlayerModel>> GetPlayersWithStatsAsync(string positionFilter, int year)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Step 1: Query all players with optional position filtering
            var playersQuery = context.Players
                .Include(p => p.WeeklyStats) // ✅ Include weekly stats but filter later in-memory
                .AsNoTracking()
                .Where(x =>
                    x.LastUpdatedDateTime != null &&
                    (string.IsNullOrEmpty(positionFilter) || x.Position == positionFilter) &&
                    x.TeamId > 0); // Ensure players are assigned to a valid team

            // Step 2: Fetch players into memory first (allows LINQ-to-Objects filtering)
            var playersList = await playersQuery.ToListAsync();

            // Step 3: Process players and filter stats in-memory
            var players = playersList
                .Select(x => new PlayerModel
                {
                    PlayerId = x.PlayerId,
                    Position = x.Position,
                    First = x.First,
                    FullName = x.FullName,
                    Last = x.Last,
                    TeamId = x.TeamId,

                    // ✅ Apply filtering on WeeklyStats AFTER loading into memory (EF optimization)
                    PlayerStats = x.WeeklyStats
                        .Where(s => s.Year == year) // ✅ Only include stats for the selected year
                        .Select(s => new WeeklyStatModel
                        {
                            PassingYards = s.PassingYards,
                            PassingTouchdowns = s.PassingTouchdowns,
                            PassingInterceptions = s.PassingInterceptions,
                            PassingSacks = s.PassingSacks,
                            RushingYards = s.RushingYards,
                            RushingTouchdowns = s.RushingTouchdowns,
                            ReceptionYards = s.ReceptionYards,
                            ReceptionTouchdowns = s.ReceptionTouchdowns,
                            ReturnYards = s.ReturnYards,
                            ReturnTouchdowns = s.ReturnTouchdowns,
                            DefenseSacks = s.DefenseSacks,
                            DefenseInterceptions = s.DefenseInterceptions,
                            DefenseFumbleRecoveries = s.DefenseFumbleRecoveries,
                            DefensePointsAllowed = s.DefensePointsAllowed,
                            DefenseSafeties = s.DefenseSafeties,
                            DefenseTouchdowns = s.DefenseTouchdowns,
                            DefenseBlockedKicks = s.DefenseBlockedKicks,
                            FieldGoal1 = s.FieldGoal1,
                            FieldGoal2 = s.FieldGoal2,
                            FieldGoal3 = s.FieldGoal3,
                            FieldGoal4 = s.FieldGoal4,
                            FieldGoal5 = s.FieldGoal5,
                            FieldGoalMiss1 = s.FieldGoalMiss1,
                            FieldGoalMiss2 = s.FieldGoalMiss2,
                            PointsAfterMade = s.PointsAfterMade,
                            PointsAfterMiss = s.PointsAfterMiss,
                            TwoPointConversions = s.TwoPointConversions,
                            TotalPoints = s.TotalPoints,
                            Week = s.Week,
                            Year = s.Year
                        }).ToList(),

                    // ✅ Compute TotalPoints in-memory to avoid EF query conversion issues
                    TotalPoints = x.WeeklyStats
                        .Where(ws => ws.Year == year)
                        .Sum(ws => ws.TotalPoints ?? 0)
                })
                .OrderByDescending(x => x.TotalPoints) // Sort players by total points
                .ToList();

            return players;
        }

        /// <summary>
        /// Retrieves a list of available player positions for dropdown selection.
        /// </summary>
        /// <returns>A list of <see cref="DropdownModel"/> containing position names and values.</returns>
        private static List<DropdownModel> GetPlayerPositions()
        {
            return new List<DropdownModel>
            {
                new DropdownModel {Text = "QB", Value = "QB"},   // Quarterback
                new DropdownModel {Text = "RB", Value = "RB"},   // Running Back
                new DropdownModel {Text = "WR", Value = "WR"},   // Wide Receiver
                new DropdownModel {Text = "TE", Value = "TE"},   // Tight End
                new DropdownModel {Text = "K", Value = "K"},     // Kicker
                new DropdownModel {Text = "DEF", Value = "DEF"}  // Defense
            };
        }
        /// <summary>
        /// Asynchronously adds or updates the user's weekly pick for a given position.
        /// </summary>
        /// <param name="playerId">The ID of the player to be selected.</param>
        /// <param name="week">The week for which the selection is made.</param>
        /// <param name="year">The year for which the selection is made.</param>
        /// <param name="userId">The ID of the user making the selection.</param>
        private async Task AddWeeklyUserPickAsync(int playerId, int week, int year, string userId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Retrieve player details
            var player = await context.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PlayerId == playerId);

            // Fetch team lock statuses for the current week and year
            var teamLockStatuses = await _commonDataService.GetTeamLockStatusListAsync(week, year);
            if (player == null || teamLockStatuses == null)
                return;

            // Check if the player's team is locked
            var playerTeamStatus = teamLockStatuses
                .FirstOrDefault(x => x.TeamId == player.TeamId)
                ?.IsLocked ?? true;

            // Only proceed if the player's team is not locked
            if (!playerTeamStatus)
            {
                // Retrieve existing user pick for the given week, year, and position
                var userPick = await context.WeeklyUserTeams
                    .FirstOrDefaultAsync(x =>
                        x.Year == year &&
                        x.Week == week &&
                        x.UserId == userId &&
                        x.GameTypeId == 1 &&
                        x.Position == player.Position
                    );

                if (userPick != null)
                {
                    // Retrieve the team ID of the existing pick
                    var oldPickTeamId = await context.Players
                        .AsNoTracking()
                        .Where(y => y.PlayerId == userPick.PlayerId)
                        .Select(y => y.TeamId)
                        .FirstOrDefaultAsync();

                    // Determine if the existing pick's team is locked
                    var userPickLocked = teamLockStatuses
                        .FirstOrDefault(x => x.TeamId == oldPickTeamId)
                        ?.IsLocked ?? true;

                    if (!userPickLocked)
                    {
                        // Remove the existing pick and add the new one
                        context.WeeklyUserTeams.Remove(userPick);
                        await context.SaveChangesAsync();
                        await InsertNewPickAsync(playerId, week, year, userId, player, 1);
                    }
                }
                else
                {
                    // No existing pick for this position, directly add the new one
                    await InsertNewPickAsync(playerId, week, year, userId, player, 1);
                }
            }
        }

        /// <summary>
        /// Inserts a new weekly user pick into the database.
        /// </summary>
        /// <param name="playerId">The ID of the player being selected.</param>
        /// <param name="week">The week for which the selection is being made.</param>
        /// <param name="year">The year for which the selection is being made.</param>
        /// <param name="userId">The ID of the user making the selection.</param>
        /// <param name="player">The player entity containing additional information.</param>
        /// <param name="gameTypeId">The type of game mode (e.g., 1 for Weekly, 2 for Survivor).</param>
        private async Task InsertNewPickAsync(int playerId, int week, int year, string userId, Player player, int gameTypeId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Create a new WeeklyUserTeam entry for the selected player
            var pick = new WeeklyUserTeam
            {
                PlayerId = playerId,
                Position = player?.Position,
                UserId = userId,
                Year = year,
                Week = week,
                GameTypeId = gameTypeId
            };

            // Add the new pick to the database
            await context.WeeklyUserTeams.AddAsync(pick);
            await context.SaveChangesAsync(); // Persist changes
        }

        /// <summary>
        /// Asynchronously adds or updates a user's survivor picks if the week is 1.
        /// </summary>
        /// <param name="playerId">The ID of the player being selected.</param>
        /// <param name="week">The current week; picks are only allowed for week 1.</param>
        /// <param name="year">The year for which the selection is being made.</param>
        /// <param name="userId">The ID of the user making the selection.</param>
        private async Task AddSurvivorUserPicksAsync(int playerId, int week, int year, string userId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Survivor picks can only be added for week 1
            if (week != 1)
                return;

            // Fetch player details
            var player = await context.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PlayerId == playerId);

            var teamLockStatuses = await _commonDataService.GetTeamLockStatusListAsync(week, year);
            if (player == null || teamLockStatuses == null)
                return;

            // ✅ Check if the player's team is locked
            var playerTeamStatus = teamLockStatuses
                .FirstOrDefault(x => x.TeamId == player.TeamId)
                ?.IsLocked ?? true;

            if (!playerTeamStatus)
            {
                // ✅ Check if the user has already picked a player for this position
                var userPick = await context.WeeklyUserTeams
                    .FirstOrDefaultAsync(x =>
                        x.Year == year &&
                        x.Week == week &&
                        x.UserId == userId &&
                        x.GameTypeId == 2 &&
                        x.Position == player.Position
                    );

                if (userPick != null)
                {
                    // ✅ Check if the old pick's team is locked
                    var oldPickTeamId = await context.Players
                        .AsNoTracking()
                        .Where(p => p.PlayerId == userPick.PlayerId)
                        .Select(p => p.TeamId)
                        .FirstOrDefaultAsync();

                    var userPickLocked = teamLockStatuses
                        .FirstOrDefault(x => x.TeamId == oldPickTeamId)
                        ?.IsLocked ?? true;

                    if (!userPickLocked)
                    {
                        // ✅ Remove existing survivor picks for this position
                        await DeleteSurvivorPicksAsync(year, userId, player.Position);

                        // ✅ Add new picks for all 18 weeks
                        for (int i = 1; i <= 18; i++)
                        {
                            await InsertNewPickAsync(playerId, i, year, userId, player, 2);
                        }
                    }
                }
                else
                {
                    // ✅ Ensure any old survivor picks for this position are removed
                    await DeleteSurvivorPicksAsync(year, userId, player.Position);

                    // ✅ Add new picks for all 18 weeks
                    for (int i = 1; i <= 18; i++)
                    {
                        await InsertNewPickAsync(playerId, i, year, userId, player, 2);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously removes all survivor picks for a given user, year, and position.
        /// </summary>
        /// <param name="year">The year for which survivor picks should be deleted.</param>
        /// <param name="userId">The ID of the user whose picks are being removed.</param>
        /// <param name="position">The position (e.g., QB, RB, WR) for which the picks should be deleted.</param>
        private async Task DeleteSurvivorPicksAsync(int year, string userId, string position)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Find all survivor picks matching the user, year, and position
            var removeSurvivorPicks = context.WeeklyUserTeams
                .Where(x => x.UserId == userId &&
                            x.GameTypeId == 2 && // ✅ Only delete picks for the Survivor game type
                            x.Year == year &&
                            x.Position == position);

            // ✅ Remove the matched survivor picks from the database
            context.WeeklyUserTeams.RemoveRange(removeSurvivorPicks);
            await context.SaveChangesAsync();
        }

    }
}