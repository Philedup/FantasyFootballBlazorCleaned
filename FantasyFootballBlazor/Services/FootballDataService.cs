using FantasyFootball.Shared;
using FantasyFootball.Shared.Services;
using FantasyFootballBlazor.Data;
using FantasyFootballBlazor.Data.Models.Entities;
using FantasyFootballBlazor.Factories;
using Microsoft.EntityFrameworkCore;

namespace FantasyFootballBlazor.Services
{
    public class FootballDataService : IFootballDataService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        private readonly ICommonDataService _commonDataService;
        private readonly IWeeklyPickFactory _weeklyPickFactory;
        private readonly ITimeProvider _datetime;
        private readonly IPlayerPositionStatFactory _playerPositionStatFactory;

        public FootballDataService(IDbContextFactory<ApplicationDbContext> dbContextFactory, ICommonDataService commonDataService, IWeeklyPickFactory weeklyPickFactory, ITimeProvider datetime, IPlayerPositionStatFactory playerPositionStatFactory)
        {
            _dbContextFactory = dbContextFactory;
            _commonDataService = commonDataService;
            _weeklyPickFactory = weeklyPickFactory;
            _datetime = datetime;
            _playerPositionStatFactory = playerPositionStatFactory;
        }

        /// <summary>
        /// Asynchronously creates the Index page view model, including weekly and survivor picks,
        /// tie-breaker games, prize pots, alerts, and available week selections.
        /// </summary>
        /// <param name="currentWeek">The current active week.</param>
        /// <param name="selectedWeek">The user-selected week for viewing data.</param>
        /// <param name="year">The NFL season year.</param>
        /// <returns>
        /// A <see cref="Task{IndexViewModel}"/> containing the compiled data for the index page.
        /// </returns>
        public async Task<IndexViewModel> GetIndexViewModelAsync(int currentWeek, int selectedWeek, int year)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Retrieve tie-breaker games for the selected week and year
            var tieBreakers = await context.TeamSchedules
                .AsNoTracking()
                .Where(x => x.Week == selectedWeek && x.Year == year && x.TieBreakGame == true)
                .ToListAsync();

            // Fetch weekly user picks and survivor picks for the selected week
            var weeklyPicks = await GetAllPlayersWeeklyRosterAsync(selectedWeek, year, 1, tieBreakers);
            var survivorPicks = await GetAllPlayersWeeklyRosterAsync(selectedWeek, year, 2, tieBreakers);

            // Retrieve any active alert messages for the index page
            var alert = await context.Alerts.AsNoTracking().FirstOrDefaultAsync();

            // Construct and return the IndexViewModel with gathered data
            return new IndexViewModel
            {
                PrizePotWeekly = await GetPrizePotAsync(1), // Weekly prize pool
                PrizePotSurvivor = await GetPrizePotAsync(2), // Survivor prize pool
                PrizePotTotalPoints = await GetPrizePotAsync(3), // Total points-based prize pool
                CurrentWeekUserPicks = weeklyPicks,
                SurvivorUserPicks = survivorPicks,
                AlertMessage = alert?.IndexPageAlert, // Set alert message if available
                CurrentWeek = selectedWeek, // Set the selected week
            };
        }

        /// <summary>
        /// Asynchronously creates a rankings view model, retrieving user rankings
        /// and prize pot details for a given NFL season year.
        /// </summary>
        /// <param name="year">The NFL season year.</param>
        /// <returns>
        /// A <see cref="Task{RankingsViewModel}"/> containing weekly rankings,
        /// survival rankings, and prize pool amounts.
        /// </returns>
        public async Task<RankingsViewModel> CreateRankingsViewModelAsync(int year)
        {
            // Execute ranking and prize pot retrieval tasks concurrently for better performance
            var rankingTask = await GetUserRankingsAsync(year, 1); // Weekly rankings
            var survivorRankingTask = await GetUserRankingsAsync(year, 2); // Survival rankings
            var prizePotTotalPointsTask = await GetPrizePotAsync(3); // Total points-based prize pool
            var prizePotSurvivorTask = await GetPrizePotAsync(2); // Survivor prize pool

            // Construct and return the RankingsViewModel with retrieved data
            return new RankingsViewModel
            {
                WeeklyRanks = rankingTask, // Assign weekly rankings
                SurvivalRanks = survivorRankingTask, // Assign survival rankings
                PrizePotSurvivor = prizePotSurvivorTask, // Assign survivor prize pot
                PrizePotTotalPoints = prizePotTotalPointsTask // Assign total points-based prize pot
            };
        }

        /// <summary>
        /// Asynchronously retrieves and constructs the "My Team" view model for a given user, week, and year.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="selectedWeek">The week for which to retrieve the user's team.</param>
        /// <param name="year">The NFL season year.</param>
        /// <param name="currentWeek">The current week of the season.</param>
        /// <returns>
        /// A <see cref="Task{MyTeamViewModel}"/> containing the user's selected players,
        /// survivor picks, lock statuses, and additional team-related data.
        /// </returns>
        public async Task<MyTeamViewModel> GetMyTeamViewModelAsync(string userId, int selectedWeek, int year, int currentWeek)
        {
            try
            {
                await using var context = _dbContextFactory.CreateDbContext();

                // Fetch global alerts and user data
                var alert = await context.Alerts.AsNoTracking().FirstOrDefaultAsync();
                var user = await context.ApplicationUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);

                // Ensure the user is a paid participant before proceeding
                if (user?.Paid != true)
                    return new MyTeamViewModel { Paid = false };

                // Fetch the user's weekly and survivor picks
                var players = await _commonDataService.GetUsersWeeklyPicksAsync(userId, selectedWeek, year, 1);
                var playersSurvivor = await _commonDataService.GetUsersWeeklyPicksAsync(userId, selectedWeek, year, 2);

                // Retrieve team lock statuses for the given week and year
                var teamLockStatuses = await _commonDataService.GetTeamLockStatusListAsync(selectedWeek, year);

                // Extract locked team IDs and update player lock statuses
                var lockedTeams = teamLockStatuses.Where(x => x.IsLocked).Select(x => x.TeamId).ToList();
                UpdateLockStatus(players, lockedTeams, selectedWeek, currentWeek);
                UpdateSurvivorLockStatus(playersSurvivor, lockedTeams, selectedWeek);

                // Construct the MyTeamViewModel with player details
                var vm = new MyTeamViewModel
                {
                    // Populate the user's weekly picks
                    QuarterBack = await CreateAndPopulatePlayerDetailsAsync(players, "QB", year),
                    RunningBack = await CreateAndPopulatePlayerDetailsAsync(players, "RB", year),
                    WideReceiver = await CreateAndPopulatePlayerDetailsAsync(players, "WR", year),
                    TightEnd = await CreateAndPopulatePlayerDetailsAsync(players, "TE", year),
                    Kicker = await CreateAndPopulatePlayerDetailsAsync(players, "K", year),
                    Defense = await CreateAndPopulatePlayerDetailsAsync(players, "DEF", year),

                    // Populate the user's survivor picks
                    SurvivorQuarterBack = await CreateAndPopulatePlayerDetailsAsync(playersSurvivor, "QB", year),
                    SurvivorRunningBack = await CreateAndPopulatePlayerDetailsAsync(playersSurvivor, "RB", year),
                    SurvivorWideReceiver = await CreateAndPopulatePlayerDetailsAsync(playersSurvivor, "WR", year),
                    SurvivorTightEnd = await CreateAndPopulatePlayerDetailsAsync(playersSurvivor, "TE", year),
                    SurvivorKicker = await CreateAndPopulatePlayerDetailsAsync(playersSurvivor, "K", year),
                    SurvivorDefense = await CreateAndPopulatePlayerDetailsAsync(playersSurvivor, "DEF", year),

                    // Additional metadata
                    SelectedWeek = selectedWeek,
                    CurrentWeek = currentWeek,
                    WeeklyPicksLeft = Math.Max(0, 6 - players.Count), // Max 6 picks allowed
                    SurvivorPicksLeft = Math.Max(0, 6 - playersSurvivor.Count), // Max 6 picks for survivor mode
                    Paid = true, // User is paid
                    Survivor = user.Survival == true, // User participates in survivor mode
                    AlertMessage = alert?.MyTeamPageAlert, // Display any active alerts
                };

                // Load and assign the user's tie-breaker selection asynchronously
                await LoadUserTieBreakerAsync(user, vm, selectedWeek, year);

                return vm;
            }
            catch (Exception ex)
            {
                // Log errors for debugging purposes
                Console.WriteLine($"[ERROR] {ex.Message}\nStackTrace:\n{ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously saves or updates a user's tie-breaker score for a given schedule.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="scheduleId">The unique identifier of the scheduled game.</param>
        /// <param name="usersPredictedScore">The user's predicted total score for the game.</param>
        /// <returns>
        /// A <see cref="Task{bool}"/> indicating whether the operation was successful.
        /// Returns <c>true</c> if the tie-breaker was saved successfully; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> SaveTieBreakerScoreAsync(string userId, int scheduleId, int usersPredictedScore)
        {
            try
            {
                await using var context = _dbContextFactory.CreateDbContext();

                // Retrieve existing tie-breaker entry for the user and schedule
                var userTieBreaker = await context.UserTieBreakers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.ScheduleId == scheduleId);

                if (userTieBreaker == null)
                {
                    // Create a new tie-breaker entry if none exists
                    await context.UserTieBreakers.AddAsync(new UserTieBreaker
                    {
                        ScheduleId = scheduleId,
                        UserId = userId,
                        TotalScore = usersPredictedScore
                    });
                }
                else
                {
                    // Update the existing tie-breaker entry
                    userTieBreaker.TotalScore = usersPredictedScore;
                    context.UserTieBreakers.Update(userTieBreaker);
                }

                // Persist changes to the database
                await context.SaveChangesAsync();
                return true; // Operation successful
            }
            catch (Exception e)
            {
                // Log any errors encountered during the operation
                Console.Error.WriteLine($"Error: {e.Message} {(e.InnerException != null ? e.InnerException.Message : string.Empty)}");
                return false; // Operation failed
            }
        }

        /// <summary>
        /// Asynchronously loads the user's tie-breaker scores for the given week and year.
        /// </summary>
        /// <param name="user">The <see cref="ApplicationUser"/> whose tie-breaker scores are being loaded.</param>
        /// <param name="vm">The <see cref="MyTeamViewModel"/> that will be populated with tie-breaker data.</param>
        /// <param name="week">The week for which the tie-breaker scores are being retrieved.</param>
        /// <param name="year">The year for which the tie-breaker scores are being retrieved.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task LoadUserTieBreakerAsync(ApplicationUser user, MyTeamViewModel vm, int week, int year)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Fetch required data asynchronously
            var nflTeams = await context.Teams.AsNoTracking().ToListAsync();
            var weeksActualScores = await context.TeamSchedules.AsNoTracking()
                .Where(x => x.Week == week && x.Year == year && x.TieBreakGame == true)
                .ToListAsync();
            var now = _datetime.Now.AddHours(_commonDataService.DaylightSavingsAdjustment());

            int game1ScheduleId = 0;
            int game2ScheduleId = 0;

            // Process up to 2 tie-breaker games
            for (int count = 1; count <= weeksActualScores.Count && count <= 2; count++)
            {
                var schedule = weeksActualScores[count - 1];

                // Generate score label using team abbreviations
                string scoreLabel = $"{ConvertTeamIdToShortName(schedule.AwayTeamId, nflTeams)}@" +
                                    $"{ConvertTeamIdToShortName(schedule.HomeTeamId, nflTeams)}";

                bool scoreLocked = now >= schedule.GameStartDateTime;
                int actualScore = (schedule.AwayTeamScore ?? 0) + (schedule.HomeTeamScore ?? 0);

                // Assign values based on whether it's the first or second tie-breaker game
                if (count == 1)
                {
                    vm.Game1ScoreLabel = scoreLabel;
                    game1ScheduleId = schedule.TeamScheduleId;
                    vm.Game1ActualScore = actualScore;
                    vm.Game1ScoreLocked = scoreLocked;
                    vm.ScheduleIdGame1 = schedule.TeamScheduleId;
                }
                else if (count == 2)
                {
                    vm.Game2ScoreLabel = scoreLabel;
                    game2ScheduleId = schedule.TeamScheduleId;
                    vm.Game2ActualScore = actualScore;
                    vm.Game2ScoreLocked = scoreLocked;
                    vm.ScheduleIdGame2 = schedule.TeamScheduleId;
                }
            }

            // Fetch tie-breaker scores for the user asynchronously
            var tiebreakersForUser = await context.UserTieBreakers.AsNoTracking()
                .Where(x => x.UserId == user.Id && x.TeamSchedule.Week == week && x.TeamSchedule.Year == year)
                .OrderBy(x => x.ScheduleId)
                .ToListAsync();

            // Assign user's predicted scores if available
            vm.Game1Score = tiebreakersForUser.FirstOrDefault(x => x.ScheduleId == game1ScheduleId)?.TotalScore;
            vm.Game2Score = tiebreakersForUser.FirstOrDefault(x => x.ScheduleId == game2ScheduleId)?.TotalScore;
        }

        /// <summary>
        /// Asynchronously retrieves a player's statistics for a given year.
        /// </summary>
        /// <param name="playerId">The unique identifier of the player.</param>
        /// <param name="year">The year for which statistics are being retrieved.</param>
        /// <returns>
        /// A <see cref="PlayerModel"/> containing the player's stats for the specified year.
        /// Returns <c>null</c> if the player does not exist.
        /// </returns>
        public async Task<PlayerModel> GetPlayerStatAsync(int playerId, int year)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            //Fetch all weekly statistics for the given player and year
            var stats = await context.WeeklyStats
                .AsNoTracking()
                .Where(x => x.PlayerId == playerId && x.Year == year)
                .Select(p => new WeeklyStatModel
                {
                    Week = p.Week,
                    TeamId = p.TeamId,
                    Year = p.Year,
                    DefenseBlockedKicks = p.DefenseBlockedKicks,
                    DefenseFumbleRecoveries = p.DefenseFumbleRecoveries,
                    DefenseInterceptions = p.DefenseInterceptions,
                    DefensePointsAllowed = p.DefensePointsAllowed,
                    DefenseSacks = p.DefenseSacks,
                    DefenseSafeties = p.DefenseSafeties,
                    DefenseTouchdowns = p.DefenseTouchdowns,
                    FieldGoal1 = p.FieldGoal1,
                    FieldGoal2 = p.FieldGoal2,
                    FieldGoal3 = p.FieldGoal3,
                    FieldGoal4 = p.FieldGoal4,
                    FieldGoal5 = p.FieldGoal5,
                    FieldGoalMiss1 = p.FieldGoalMiss1,
                    FieldGoalMiss2 = p.FieldGoalMiss2,
                    FumblesLost = p.FumblesLost,
                    LastUpdated = p.LastUpdated,
                    PassingInterceptions = p.PassingInterceptions,
                    PassingSacks = p.PassingSacks,
                    PassingTouchdowns = p.PassingTouchdowns,
                    PassingYards = p.PassingYards,
                    PlayerId = p.PlayerId,
                    TotalPoints = p.TotalPoints,
                    PointsAfterMade = p.PointsAfterMade,
                    PointsAllowed0 = p.PointsAllowed0,
                    PointsAllowed1 = p.PointsAllowed1,
                    PointsAllowed2 = p.PointsAllowed2,
                    PointsAllowed3 = p.PointsAllowed3,
                    PointsAllowed4 = p.PointsAllowed4,
                    PointsAllowed5 = p.PointsAllowed5,
                    PointsAllowed6 = p.PointsAllowed6,
                    PointsAfterMiss = p.PointsAfterMiss,
                    ReceptionTouchdowns = p.ReceptionTouchdowns,
                    ReceptionYards = p.ReceptionYards,
                    ReturnTouchdowns = p.ReturnTouchdowns,
                    ReturnYards = p.ReturnYards,
                    RushingTouchdowns = p.RushingTouchdowns,
                    RushingYards = p.RushingYards,
                    TwoPointConversions = p.TwoPointConversions
                })
                .OrderBy(x => x.Week)
                .ToListAsync(); //Use async query execution to fetch data efficiently

            //Fetch the player's core details and attach the fetched stats
            var player = await context.Players
                .AsNoTracking()
                .Where(x => x.PlayerId == playerId)
                .Select(x => new PlayerModel
                {
                    TeamId = x.TeamId,
                    PlayerId = x.PlayerId,
                    Position = x.Position,
                    PlayerStats = stats, //Pass pre-fetched stats
                    Locked = true, // This field indicates whether the player is locked for edits
                    First = x.First,
                    FullName = x.FullName,
                    Last = x.Last,
                    PlayerPictureImageUrl = x.PlayerPictureImageUrl
                })
                .FirstOrDefaultAsync(); //Use async query execution to return the first match or null

            return player; // 🔹 Returns the player model or null if no player is found
        }

        /// <summary>
        /// Asynchronously creates an earnings view model for the specified year.
        /// </summary>
        /// <param name="year">The year for which earnings data is retrieved.</param>
        /// <returns>
        /// A <see cref="EarningsViewModel"/> containing a list of users and their total earnings.
        /// </returns>
        public async Task<EarningsViewModel> CreateEarningsViewModelAsync(int year)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            //Retrieve prize amounts for places 1 to 4 for GameType 1 (Weekly)
            var prizes = await context.PrizePots
                .AsNoTracking()
                .Where(x => x.GameTypeId == 1 && x.Place >= 1 && x.Place <= 4) // 🔍 Efficient filtering in query
                .ToListAsync(); // 🟢 Async query execution

            var prizeAmounts = prizes.ToDictionary(x => x.Place, x => x.Prize);

            //Ensure all places have a default prize value (0 if not found)
            var firstPlacePrize = prizeAmounts.GetValueOrDefault(1, 0);
            var secondPlacePrize = prizeAmounts.GetValueOrDefault(2, 0);
            var thirdPlacePrize = prizeAmounts.GetValueOrDefault(3, 0);
            var fourthPlacePrize = prizeAmounts.GetValueOrDefault(4, 0);

            //Query weekly winners for the given year (only paid users)
            var data = await context.WeeklyWinners
                .AsNoTracking()
                .Where(x => x.Year == year && x.UserProfile.Paid == true) // 🔍 Filter in query
                .GroupBy(x => new { x.UserProfile.UserName, x.UserProfile.UserTeamName, x.UserId }) //Group by user
                .Select(group => new EarnedModel
                {
                    UserId = group.Key.UserId,
                    UserTeamName = string.IsNullOrWhiteSpace(group.Key.UserTeamName) ? group.Key.UserName : group.Key.UserTeamName, //Fallback to username if team name is null

                    //Calculate total earnings based on the count of placements
                    TotalEarned = group.Count(x => x.Place == 1) * firstPlacePrize
                                  + group.Count(x => x.Place == 2) * secondPlacePrize
                                  + group.Count(x => x.Place == 3) * thirdPlacePrize
                                  + group.Count(x => x.Place == 4) * fourthPlacePrize
                })
                .OrderByDescending(x => x.TotalEarned) // 🔽 Sort users by total earnings (highest first)
                .ToListAsync(); // 🟢 Async query execution

            //Construct and return the earnings view model
            return new EarningsViewModel
            {
                Users = data
            };
        }


        /// <summary>
        /// Asynchronously creates a weekly winners view model for the specified year.
        /// </summary>
        /// <param name="year">The year for which weekly winners data is retrieved.</param>
        /// <returns>
        /// A <see cref="WeeklyWinnersViewModel"/> containing a list of weekly winners, their placements, 
        /// and total earnings based on the prize distribution.
        /// </returns>
        public async Task<WeeklyWinnersViewModel> CreateWeeklyWinnersViewModelAsync(int year)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            //Retrieve prize amounts for places 1 to 4 for GameType 1 (Weekly mode)
            var prizes = await context.PrizePots
                .AsNoTracking()
                .Where(x => x.GameTypeId == 1 && x.Place >= 1 && x.Place <= 4) // 🔍 Efficient filtering
                .ToListAsync(); // 🟢 Async data fetching

            var prizeAmounts = prizes.ToDictionary(x => x.Place, x => x.Prize);

            //Fetch weekly winners for the given year
            var weeklyWinners = await context.WeeklyWinners
                .AsNoTracking()
                .Where(x => x.Year == year) // 🔍 Filter winners by year
                .Select(x => new WeeklyWinnerModel
                {
                    UserName = string.IsNullOrWhiteSpace(x.UserProfile.UserTeamName) ? x.UserProfile.UserName : x.UserProfile.UserTeamName, //Default to username if no team name
                    Place = x.Place ?? 0, //Handle null places by defaulting to 0
                    Id = x.UserId,
                    Week = x.Week,
                    Year = x.Year,

                    //Calculate total earnings for the winner based on their placement
                    Total = prizeAmounts.GetValueOrDefault(x.Place ?? 0, 0) // 🟢 Safe lookup to prevent key errors
                })
                .ToListAsync(); // 🟢 Async execution

            //Construct and return the Weekly Winners view model
            return new WeeklyWinnersViewModel
            {
                Winners = weeklyWinners
            };
        }

        /// <summary>
        /// Asynchronously retrieves and constructs a weekly picks dialog view model for a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="currentWeek">The current week in the NFL season.</param>
        /// <param name="year">The NFL season year.</param>
        /// <param name="gameTypeId">The game type ID (e.g., weekly, survivor).</param>
        /// <returns>
        /// A <see cref="UserWeeklyPicksDialogViewModel"/> containing the user's weekly picks and total points for each week.
        /// </returns>
        public async Task<UserWeeklyPicksDialogViewModel> GetUserWeeklyPicksDialogViewModelAsync(
            string userId, int currentWeek, int year, int gameTypeId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            var picks = new List<WeeklyPicksModel>();

            //Loop through all past weeks (up to currentWeek - 1)
            for (var i = 1; i < currentWeek; i++)
            {
                //Fetch user’s weekly picks for the given week and game type
                var weekPicks = await _commonDataService.GetUsersWeeklyPicksAsync(userId, i, year, gameTypeId); // 🟢 Async call

                //Create a WeeklyPicksModel and assign positions
                var pick = new WeeklyPicksModel
                {
                    Week = i,
                    QuarterBackName = weekPicks.FirstOrDefault(x => x.Position == "QB")?.FullName,
                    QuarterBackTotalPoints = weekPicks.FirstOrDefault(x => x.Position == "QB")?.TotalPoints ?? 0,

                    RunningBackName = weekPicks.FirstOrDefault(x => x.Position == "RB")?.FullName,
                    RunningBackTotalPoints = weekPicks.FirstOrDefault(x => x.Position == "RB")?.TotalPoints ?? 0,

                    WideReceiverName = weekPicks.FirstOrDefault(x => x.Position == "WR")?.FullName,
                    WideReceiverTotalPoints = weekPicks.FirstOrDefault(x => x.Position == "WR")?.TotalPoints ?? 0,

                    TightEndName = weekPicks.FirstOrDefault(x => x.Position == "TE")?.FullName,
                    TightEndTotalPoints = weekPicks.FirstOrDefault(x => x.Position == "TE")?.TotalPoints ?? 0,

                    KickerName = weekPicks.FirstOrDefault(x => x.Position == "K")?.FullName,
                    KickerTotalPoints = weekPicks.FirstOrDefault(x => x.Position == "K")?.TotalPoints ?? 0,

                    DefenseName = weekPicks.FirstOrDefault(x => x.Position == "DEF")?.FullName,
                    DefenseTotalPoints = weekPicks.FirstOrDefault(x => x.Position == "DEF")?.TotalPoints ?? 0
                };

                //Calculate total points for all positions
                pick.PlayerTotalPoints = pick.QuarterBackTotalPoints + pick.RunningBackTotalPoints +
                                         pick.WideReceiverTotalPoints + pick.TightEndTotalPoints +
                                         pick.KickerTotalPoints + pick.DefenseTotalPoints;

                //Add to the list of weekly picks
                picks.Add(pick);
            }

            //Fetch the user's display name (team name or username)
            var user = await context.ApplicationUsers
                .AsNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => new { x.UserTeamName, x.UserName }) // 🟢 Only retrieve necessary fields
                .FirstOrDefaultAsync(); // 🟢 Async execution

            //Construct and return the view model
            return new UserWeeklyPicksDialogViewModel
            {
                WeeklyPicks = picks,
                UserName = user?.UserTeamName ?? user?.UserName // Default to UserName if no team name is set
            };
        }

        /// <summary>
        /// Updates the lock status of players based on the selected week, current week, and locked teams.
        /// </summary>
        /// <param name="players">The list of players to update.</param>
        /// <param name="lockedTeams">A list of team IDs that are locked.</param>
        /// <param name="selectedWeek">The week the user is viewing.</param>
        /// <param name="currentWeek">The current active week.</param>
        private void UpdateLockStatus(List<PlayerModel> players, List<int> lockedTeams, int selectedWeek, int currentWeek)
        {
            foreach (var player in players)
            {
                //If the selected week is in the past, the player is always locked.
                if (selectedWeek < currentWeek)
                {
                    player.Locked = true;
                }
                else
                {
                    //Otherwise, check if the player's team is locked.
                    player.Locked = lockedTeams.Contains(player?.TeamId ?? 0);
                }
            }
        }

        /// <summary>
        /// Updates the lock status of players in the Survivor game mode.
        /// </summary>
        /// <param name="playersSurvivor">The list of Survivor players to update.</param>
        /// <param name="lockedTeams">A list of team IDs that are locked.</param>
        /// <param name="selectedWeek">The week the user is viewing.</param>
        private void UpdateSurvivorLockStatus(List<PlayerModel> playersSurvivor, List<int> lockedTeams, int selectedWeek)
        {
            foreach (var player in playersSurvivor)
            {
                //If it's Week 1, lock status is based on the locked teams.
                //For all other weeks, Survivor picks are permanently locked.
                player.Locked = selectedWeek == 1
                    ? lockedTeams.Contains(player?.TeamId ?? 0)
                    : true;
            }
        }

        /// <summary>
        /// Creates a player model for the specified position and populates it with relevant stats.
        /// </summary>
        /// <param name="players">The list of players to search through.</param>
        /// <param name="position">The position for which details should be retrieved.</param>
        /// <param name="year">The year for which player stats should be populated.</param>
        /// <returns>A <see cref="PlayerModel"/> containing the player's details and statistics.</returns>
        private async Task<PlayerModel> CreateAndPopulatePlayerDetailsAsync(List<PlayerModel> players, string position, int year)
        {
            //Uses a factory to create a position-specific player model.
            var positionDetails = _playerPositionStatFactory.CreatePlayerPositionDetails(players, position);

            //Asynchronously fetches and fills stats for the given year.
            await PopulateStatsAsync(year, positionDetails);

            return positionDetails;
        }

        /// <summary>
        /// Asynchronously populates a player's statistical data for a given year.
        /// </summary>
        /// <param name="year">The year for which the player's statistics should be retrieved.</param>
        /// <param name="playerModel">The <see cref="PlayerModel"/> to be populated with stats.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task PopulateStatsAsync(int year, PlayerModel playerModel)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            //Asynchronously fetch and assign weekly stats for the specified player and year
            playerModel.PlayerStats = await context.WeeklyStats
                .AsNoTracking()
                .Where(x => x.PlayerId == playerModel.PlayerId && x.Year == year)
                .Select(p => new WeeklyStatModel
                {
                    Week = p.Week,
                    TeamId = p.TeamId,
                    Year = p.Year,
                    DefenseBlockedKicks = p.DefenseBlockedKicks,
                    DefenseFumbleRecoveries = p.DefenseFumbleRecoveries,
                    DefenseInterceptions = p.DefenseInterceptions,
                    DefensePointsAllowed = p.DefensePointsAllowed,
                    DefenseSacks = p.DefenseSacks,
                    DefenseSafeties = p.DefenseSafeties,
                    DefenseTouchdowns = p.DefenseTouchdowns,
                    FieldGoal1 = p.FieldGoal1,
                    FieldGoal2 = p.FieldGoal2,
                    FieldGoal3 = p.FieldGoal3,
                    FieldGoal4 = p.FieldGoal4,
                    FieldGoal5 = p.FieldGoal5,
                    FieldGoalMiss1 = p.FieldGoalMiss1,
                    FieldGoalMiss2 = p.FieldGoalMiss2,
                    FumblesLost = p.FumblesLost,
                    LastUpdated = p.LastUpdated,
                    PassingInterceptions = p.PassingInterceptions,
                    PassingSacks = p.PassingSacks,
                    PassingTouchdowns = p.PassingTouchdowns,
                    PassingYards = p.PassingYards,
                    PlayerId = p.PlayerId,
                    TotalPoints = p.TotalPoints,
                    PointsAfterMade = p.PointsAfterMade,
                    PointsAllowed0 = p.PointsAllowed0,
                    PointsAllowed1 = p.PointsAllowed1,
                    PointsAllowed2 = p.PointsAllowed2,
                    PointsAllowed3 = p.PointsAllowed3,
                    PointsAllowed4 = p.PointsAllowed4,
                    PointsAllowed5 = p.PointsAllowed5,
                    PointsAllowed6 = p.PointsAllowed6,
                    PointsAfterMiss = p.PointsAfterMiss,
                    ReceptionTouchdowns = p.ReceptionTouchdowns,
                    ReceptionYards = p.ReceptionYards,
                    ReturnTouchdowns = p.ReturnTouchdowns,
                    ReturnYards = p.ReturnYards,
                    RushingTouchdowns = p.RushingTouchdowns,
                    RushingYards = p.RushingYards,
                    TwoPointConversions = p.TwoPointConversions
                })
                .ToListAsync(); // 🟢 Async execution for performance optimization
        }

        /// <summary>
        /// Retrieves the short team name (code) for a given team ID.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="teams">A list of <see cref="Team"/> objects containing team data.</param>
        /// <returns>The short name (code) of the team if found; otherwise, <c>null</c>.</returns>
        private string ConvertTeamIdToShortName(int teamId, List<Team> teams)
        {
            return teams.FirstOrDefault(x => x.TeamId == teamId)?.Code;
        }

        /// <summary>
        /// Asynchronously retrieves user rankings for a given year and game type.
        /// </summary>
        /// <param name="year">The year for which to fetch rankings.</param>
        /// <param name="gameTypeId">
        /// The game type ID:
        /// 1 = Weekly rankings,
        /// 2 = Survivor rankings.
        /// </param>
        /// <returns>A list of <see cref="RankingItemModel"/> containing ranked users and their total points.</returns>
        public async Task<List<RankingItemModel>> GetUserRankingsAsync(int year, int gameTypeId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Fetch users who are eligible for the given game type
            var users = await context.ApplicationUsers
                .AsNoTracking()
                .Where(x => x.Paid == true && (gameTypeId == 1 || gameTypeId == 2 && x.Survival == true))
                .ToListAsync();

            var rankList = new List<RankingItemModel>();

            // Fetch all user picks for the given game type and year
            var allUserPicks = await context.WeeklyUserTeams
                .AsNoTracking()
                .Where(x => x.Year == year && x.GameTypeId == gameTypeId)
                .ToListAsync();

            // Fetch all player stats for the given year
            var allPlayerStats = await context.WeeklyStats
                .AsNoTracking()
                .Where(x => x.Year == year)
                .ToListAsync();

            // Compute total points for each user
            foreach (var user in users)
            {
                var userPicks = allUserPicks.Where(x => x.UserId == user.Id);
                var total = userPicks
                    .SelectMany(pick => allPlayerStats.Where(stat => stat.PlayerId == pick.PlayerId && stat.Week == pick.Week))
                    .Sum(stat => stat.TotalPoints ?? 0);

                rankList.Add(new RankingItemModel
                {
                    Id = user.Id,
                    DisplayText = user.UserTeamName ?? user.UserName,
                    Total = total
                });
            }

            // Rank users based on total points
            rankList = rankList.OrderByDescending(x => x.Total)
                               .Select((item, index) =>
                               {
                                   item.Rank = index + 1;
                                   return item;
                               })
                               .ToList();

            return rankList;
        }

        /// <summary>
        /// Asynchronously retrieves all users' weekly player picks, including stats, tie-breakers, and scores.
        /// </summary>
        /// <param name="week">The week for which to retrieve player picks.</param>
        /// <param name="year">The year for which to retrieve player picks.</param>
        /// <param name="gameTypeId">
        /// The game type ID:
        /// 1 = Weekly fantasy football,
        /// 2 = Survivor pool.
        /// </param>
        /// <param name="actualScores">The actual game scores for the given week, used for tie-breaker calculations.</param>
        /// <returns>
        /// A list of <see cref="WeeklyPicksModel"/> containing user-selected players,
        /// their performance statistics, and tie-breaker details.
        /// </returns>
        public async Task<List<WeeklyPicksModel>> GetAllPlayersWeeklyRosterAsync(int week, int year, int gameTypeId, List<TeamSchedule> actualScores)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            var teamLockStatuses = await _commonDataService.GetTeamLockStatusListAsync(week, year);

            // Fetch user picks for the week, year, and game type
            var usersPlayers = await context.WeeklyUserTeams
                .AsNoTracking()
                .Where(x => x.Week == week && x.Year == year && x.GameTypeId == gameTypeId)
                .ToListAsync();

            // Get player IDs from user picks
            var limitedPlayerIdList = usersPlayers.Select(x => x.PlayerId).Distinct();

            // Fetch players based on the selected player IDs
            var limitedPlayersDetail = await context.Players
                .AsNoTracking()
                .Where(x => limitedPlayerIdList.Contains(x.PlayerId))
                .ToListAsync();

            // Fetch player stats for the given week and year
            var playerStats = await context.WeeklyStats
                .AsNoTracking()
                .Where(x => x.Week == week && x.Year == year && limitedPlayerIdList.Contains(x.PlayerId))
                .ToListAsync();

            // Fetch user tie-breakers
            var userTieBreakers = await context.UserTieBreakers
                .AsNoTracking()
                .Where(x => x.TeamSchedule.Week == week && x.TeamSchedule.Year == year)
                .ToListAsync();

            // Fetch eligible users
            var users = await context.ApplicationUsers
                .AsNoTracking()
                .Where(x => x.Paid == true && (gameTypeId == 1 || gameTypeId == 2 && x.Survival == true))
                .ToListAsync();

            // Format and return picks
            var picksList = FormatWeeklyPicks(week, actualScores, users, usersPlayers, userTieBreakers, limitedPlayersDetail, teamLockStatuses, playerStats)
                                .OrderByDescending(x => x.PlayerTotalPoints) // Rank by total points
                                .ThenBy(x => x.TotalDiff) // Sort by tie-breaker difference
                                .ToList();

            return picksList;
        }

        /// <summary>
        /// Formats and returns a list of weekly picks for all users, including selected players, 
        /// tie-breakers, team lock statuses, and player statistics.
        /// </summary>
        /// <param name="week">The week for which to format picks.</param>
        /// <param name="actualScores">The actual game scores for the given week, used for tie-breaker calculations.</param>
        /// <param name="users">The list of users participating in the fantasy league.</param>
        /// <param name="usersPlayers">The list of users' selected players for the given week.</param>
        /// <param name="userTieBreakers">The tie-breaker selections made by each user.</param>
        /// <param name="limitedPlayersDetail">The list of player details relevant to the selected week.</param>
        /// <param name="teamLockStatuses">A list of team lock statuses indicating whether a team is locked for the week.</param>
        /// <param name="playerStats">The statistical performance of players for the given week.</param>
        /// <returns>
        /// A list of <see cref="WeeklyPicksModel"/> objects, each representing a user's weekly fantasy picks,
        /// including player performance data and tie-breaker information.
        /// </returns>
        private List<WeeklyPicksModel> FormatWeeklyPicks(int week, List<TeamSchedule> actualScores,
            List<ApplicationUser> users, List<WeeklyUserTeam> usersPlayers, List<UserTieBreaker> userTieBreakers,
            List<Player> limitedPlayersDetail, List<TeamLockStatusModel> teamLockStatuses, List<WeeklyStat> playerStats)
        {
            return users.Select(user =>
            {
                var usersPicks = usersPlayers.Where(x => x.UserId == user.Id);
                var tiebreakers = userTieBreakers.Where(x => x.UserId == user.Id).OrderBy(x => x.ScheduleId).ToList();

                return _weeklyPickFactory.CreateUserWeeklyPick(week, limitedPlayersDetail, usersPicks, teamLockStatuses, user, playerStats, tiebreakers, actualScores);
            }).ToList();
        }

        /// <summary>
        /// Asynchronously retrieves the prize pool details for a specified game type.
        /// </summary>
        /// <param name="gameTypeId">The game type identifier (e.g., 1 for Weekly, 2 for Survivor).</param>
        /// <returns>
        /// A <see cref="Task{List{PrizePotModel}}"/> representing the asynchronous operation.
        /// The task result contains a list of <see cref="PrizePotModel"/> objects,
        /// each representing a prize distribution for the given game type.
        /// </returns>
        public async Task<List<PrizePotModel>> GetPrizePotAsync(int gameTypeId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            return await context.PrizePots
                .AsNoTracking()
                .Where(x => x.GameTypeId == gameTypeId).Select(x => new PrizePotModel
                {
                    PrizePotId = x.PrizePotId,
                    GameTypeId = x.GameTypeId,
                    Place = x.Place,
                    Prize = x.Prize
                })
                .ToListAsync();
        }

    }
}


