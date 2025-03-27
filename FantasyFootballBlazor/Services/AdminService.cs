using FantasyFootball.Shared;
using FantasyFootball.Shared.Services;
using FantasyFootballBlazor.Data;
using FantasyFootballBlazor.Data.Models.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FantasyFootballBlazor.Services
{
    public class AdminService : IAdminService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommonDataService _commonDataService;
        private readonly IUserService _userService;
        private readonly NavigationManager _navigationManager;

        public AdminService(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            ICommonDataService commonDataService,
            IUserService userService,
            NavigationManager navigationManager)
        {
            _dbContextFactory = dbContextFactory;
            _commonDataService = commonDataService;
            _userService = userService;
            _navigationManager = navigationManager;
        }

        /// <summary>
        /// Checks if the current user is an admin. If not, redirects them to the home page.
        /// </summary>
        /// <returns>True if the user is an admin, false otherwise.</returns>
        public async Task<bool> EnsureAdminAccessAsync()
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null || user.Admin != true)
            {
                _navigationManager.NavigateTo("/", forceLoad: true);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Asynchronously creates a new NFL season with 19 weeks starting from the given date.
        /// </summary>
        /// <param name="startDate">The start date for the first week of the NFL season.</param>
        /// <returns>A string indicating the operation was successful.</returns>
        public async Task<string> CreateNflYearAsync(DateTime startDate)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            int currentYear = DateTime.Now.Year;

            // ✅ Loop through 19 weeks, creating an NFL week entry for each
            for (int week = 0; week < 19; week++)
            {
                context.NflWeeks.Add(new NflWeek
                {
                    Year = currentYear,
                    StartDate = startDate.AddDays(week * 7), // ✅ Each week starts 7 days after the previous one
                    Name = week + 1 // ✅ Week numbers start from 1 up to 19
                });
            }

            await context.SaveChangesAsync(); // ✅ Save all new weeks to the database
            return "Done"; // ✅ Return confirmation of successful execution
        }


        /// <summary>
        /// Asynchronously retrieves the yearly user rankings and statistics.
        /// </summary>
        /// <returns>
        /// A <see cref="YearlyUserDataViewModel"/> containing a list of users' rankings,
        /// earnings, points, and other relevant data.
        /// </returns>
        public async Task<YearlyUserDataViewModel> GetYearlyUserDataViewModelAsync()
        {
            await using var context = _dbContextFactory.CreateDbContext();

            try
            {
                // ✅ Fetch all yearly end results and map them to YearlyUserRankingModel
                var yearlyList = await context.YearEndResults
                    .Include(x => x.UserProfile) // ✅ Include user profile data
                    .Include(x => x.GameType) // ✅ Include game type data
                    .OrderByDescending(x => x.YearEndResultId) //✅ Order by descending id to see latest entered
                    .AsNoTracking() // ✅ Improves performance since we are only reading data
                    .Select(x => new YearlyUserRankingModel
                    {
                        Rank = x.Rank,
                        Year = x.Year,
                        TotalEarned = x.TotalEarned,
                        TotalPoints = x.TotalPoints,
                        UserName = !string.IsNullOrEmpty(x.UserProfile.UserTeamName)
                            ? $"{x.UserProfile.UserName} ({x.UserProfile.UserTeamName})"
                            : x.UserProfile.UserName, // ✅ Append team name if available
                        GameMode = x.GameType.Name, // ✅ Game type for the ranking
                        GameTypeId = x.GameTypeId, // ✅ GameTypeId
                        UserId = x.UserId
                    }).ToListAsync();

                // ✅ Fetch available game types for dropdown
                var gameTypeList = await context.GameTypes
                    .AsNoTracking()
                    .Select(x => new DropdownModel
                    {
                        Text = x.Name,
                        Value = x.GameTypeId.ToString()
                    }).ToListAsync();

                // ✅ Fetch paid users for dropdown selection
                var userList = await context.ApplicationUsers
                    .AsNoTracking()
                    .Where(x => x.Paid == true)
                    .OrderBy(x => x.UserName)
                    .Select(x => new DropdownModel
                    {
                        Text = x.UserTeamName == null ? x.UserName : $"{x.UserName} ({x.UserTeamName})",
                        Value = x.Id
                    }).ToListAsync();

                // ✅ Fetch the latest NFL year
                var latestYear = await context.NflYears
                    .AsNoTracking()
                    .OrderByDescending(x => x.NflYearId)
                    .Select(x => x.NflYearId)
                    .FirstOrDefaultAsync();

                // ✅ Ensure there's a valid NFL year, otherwise default to the current year
                if (latestYear == 0)
                {
                    latestYear = DateTime.UtcNow.Year;
                }

                // ✅ Return the formatted data inside a view model
                return new YearlyUserDataViewModel
                {
                    AddPlayer = new AddYearlyPlayerRankingModel
                    {
                        GameTypeList = gameTypeList ?? new List<DropdownModel>(),
                        UserList = userList ?? new List<DropdownModel>(),
                        Year = latestYear
                    },
                    Users = yearlyList ?? new List<YearlyUserRankingModel>()
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR] GetYearlyUserDataViewModelAsync: {ex.Message}");
                return new YearlyUserDataViewModel
                {
                    AddPlayer = new AddYearlyPlayerRankingModel
                    {
                        GameTypeList = new List<DropdownModel>(),
                        UserList = new List<DropdownModel>(),
                        Year = DateTime.UtcNow.Year
                    },
                    Users = new List<YearlyUserRankingModel>()
                };
            }
        }

        /// <summary>
        /// Saves or updates yearly user data in the database.
        /// </summary>
        /// <param name="data">
        /// A <see cref="YearlyUserDataViewModel"/> containing the user's ranking,
        /// earnings, points, and game type for a specific year.
        /// </param>
        /// <returns>Returns <c>true</c> if the save operation was successful; otherwise, <c>false</c>.</returns>
        public async Task<bool> SaveYearlyUserDataAsync(YearlyUserDataViewModel data)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            try
            {
                // ✅ Check if an entry already exists for the given year, game type, and user
                var existingRecord = await context.YearEndResults
                    .FirstOrDefaultAsync(x => x.Year == data.AddPlayer.Year &&
                                              x.GameTypeId == data.AddPlayer.GameTypeId &&
                                              x.UserId == data.AddPlayer.UserId);

                if (existingRecord != null)
                {
                    // ✅ Update existing record
                    existingRecord.Rank = data.AddPlayer.Rank;
                    existingRecord.TotalEarned = data.AddPlayer.TotalEarned;
                    existingRecord.TotalPoints = data.AddPlayer.TotalPoints;
                }
                else
                {
                    // ✅ Insert new record if no existing entry is found
                    context.YearEndResults.Add(new YearEndResults
                    {
                        Rank = data.AddPlayer.Rank,
                        Year = data.AddPlayer.Year,
                        GameTypeId = data.AddPlayer.GameTypeId,
                        TotalEarned = data.AddPlayer.TotalEarned,
                        UserId = data.AddPlayer.UserId,
                        TotalPoints = data.AddPlayer.TotalPoints
                    });
                }

                // ✅ Save changes to the database
                await context.SaveChangesAsync();
                return true; // Success
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR] Failed to save player data: {ex.Message}");
                return false; // Failure
            }
        }

        /// <summary>
        /// Deletes a specific yearly user record based on user ID, year, and game type.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="year">The year of the record.</param>
        /// <param name="gameTypeId">The game type ID associated with the record.</param>
        /// <returns>Returns <c>true</c> if the deletion was successful; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteYearlyUserDataAsync(string userId, int year, int gameTypeId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            try
            {
                // ✅ Find the record to delete
                var record = await context.YearEndResults
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.Year == year && x.GameTypeId == gameTypeId);

                if (record == null)
                {
                    Console.Error.WriteLine($"[WARNING] No record found for UserId: {userId}, Year: {year}, GameTypeId: {gameTypeId}");
                    return false; // No record to delete
                }

                // ✅ Remove the record from the database
                context.YearEndResults.Remove(record);
                await context.SaveChangesAsync();

                Console.WriteLine($"[INFO] Successfully deleted record for UserId: {userId}, Year: {year}, GameTypeId: {gameTypeId}");
                return true; // Deletion successful
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR] Failed to delete player data: {ex.Message}");
                return false; // Deletion failed
            }
        }

        /// <summary>
        /// Retrieves a list of users and their details asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="UserListViewModel"/> containing a collection of users
        /// with their associated details such as email, payment status, and team name.
        /// </returns>
        public async Task<UserListViewModel> GetUserListViewModelAsync()
        {
            try
            {
                await using var context = _dbContextFactory.CreateDbContext();

                // ✅ Fetch users from the database, ensuring no tracking for performance optimization
                var users = await context.ApplicationUsers
                    .AsNoTracking()
                    .Select(x => new UserModel
                    {
                        Email = x.Email,                  // ✅ User's registered email
                        Paid = x.Paid == true,            // ✅ Payment status (true if paid)
                        PaypalEmail = x.PaypalEmail,      // ✅ User's PayPal email
                        Survival = x.Survival == true,    // ✅ Indicates if the user is in survival mode
                        UserId = x.Id,                    // ✅ Unique user ID
                        UserName = x.UserName,            // ✅ Username
                        UserTeamName = x.UserTeamName     // ✅ User's chosen team name
                    })
                    .ToListAsync();

                // ✅ Construct the view model
                var list = new UserListViewModel
                {
                    Users = users
                };

                return list;
            }
            catch (Exception ex)
            {
                // ✅ Log the error and rethrow for better debugging
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates and returns a view model for the weekly data update page.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="UpdateWeeklyDataViewModel"/> containing
        /// a dropdown list of weeks (1-18) for selection.
        /// </returns>
        public UpdateWeeklyDataViewModel CreateUpdateWeeklyDataViewModel()
        {
            // ✅ Predefined list of weeks (1-18) as dropdown options
            var weeks = new List<DropdownModel>
            {
                new DropdownModel {Text = "1", Value = "1"},
                new DropdownModel {Text = "2", Value = "2"},
                new DropdownModel {Text = "3", Value = "3"},
                new DropdownModel {Text = "4", Value = "4"},
                new DropdownModel {Text = "5", Value = "5"},
                new DropdownModel {Text = "6", Value = "6"},
                new DropdownModel {Text = "7", Value = "7"},
                new DropdownModel {Text = "8", Value = "8"},
                new DropdownModel {Text = "9", Value = "9"},
                new DropdownModel {Text = "10", Value = "10"},
                new DropdownModel {Text = "11", Value = "11"},
                new DropdownModel {Text = "12", Value = "12"},
                new DropdownModel {Text = "13", Value = "13"},
                new DropdownModel {Text = "14", Value = "14"},
                new DropdownModel {Text = "15", Value = "15"},
                new DropdownModel {Text = "16", Value = "16"},
                new DropdownModel {Text = "17", Value = "17"},
                new DropdownModel {Text = "18", Value = "18"},
            };

            // ✅ Return the view model with the available weeks
            return new UpdateWeeklyDataViewModel
            {
                Weeks = weeks
            };
        }

        /// <summary>
        /// Creates a schedule admin view model for the specified week.
        /// </summary>
        /// <param name="week">The week number for which the schedule is being retrieved.</param>
        /// <returns>
        /// A <see cref="ScheduleAdminViewModel"/> containing schedule data, 
        /// including teams, scheduled games, and available home/away teams.
        /// </returns>
        public async Task<ScheduleAdminViewModel> GetScheduleAdminViewModelAsync(int week)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Fetch the current NFL year from a shared service
            var year = await _commonDataService.GetCurrentYearAsync();

            // ✅ Retrieve all NFL teams
            var teams = await context.Teams
                .AsNoTracking()
                .ToListAsync();

            // ✅ Retrieve data for the specified week and year
            var weekData = await context.NflWeeks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == week && x.Year == year);

            // ✅ Get all scheduled games for the given week and year
            var scheduledGames = await context.TeamSchedules
                .Where(x => x.Week == week && x.Year == year)
                .AsNoTracking()
                .ToListAsync();

            // ✅ Determine which teams are already scheduled
            var scheduledTeamIds = scheduledGames
                .SelectMany(x => new[] { x.HomeTeamId, x.AwayTeamId })
                .Distinct()
                .ToList();

            // ✅ Create dropdown lists for teams that have not yet been scheduled
            var unscheduledTeams = teams
                .Where(t => !scheduledTeamIds.Contains(t.TeamId))
                .Select(t => new DropdownModel
                {
                    Text = t.Name,
                    Value = t.TeamId.ToString()
                })
                .OrderBy(t => t.Text)
                .ToList();

            // ✅ Assign the unscheduled teams list for both home and away team selection
            var homeTeams = unscheduledTeams;
            var awayTeams = unscheduledTeams;

            // ✅ Build the list of already scheduled games
            var scheduledGamesList = scheduledGames
                .Select(x => new ScheduleAdminModel
                {
                    AwayTeamId = x.AwayTeamId,
                    HomeTeamId = x.HomeTeamId,
                    ScheduleDate = x.GameStartDateTime,
                    ScheduleId = x.TeamScheduleId,
                    WeekId = x.Week,
                    TiebreakGame = x.TieBreakGame ?? false,
                    AwayTeamScore = x.AwayTeamScore,
                    HomeTeamScore = x.HomeTeamScore,
                    HomeTeam = teams.FirstOrDefault(t => t.TeamId == x.HomeTeamId)?.Name,
                    AwayTeam = teams.FirstOrDefault(t => t.TeamId == x.AwayTeamId)?.Name
                })
                .OrderBy(x => x.ScheduleId)
                .ToList();

            // ✅ Construct and return the view model
            return new ScheduleAdminViewModel
            {
                Week = week,
                Year = year,
                HomeTeams = homeTeams,
                AwayTeams = awayTeams,
                ScheduleDate = weekData?.StartDate ?? DateTime.Now, // Default to current date if not found
                ScheduleTime = weekData?.StartDate != null
                    ? weekData.StartDate.Date.Add(new TimeSpan(19, 15, 0)) // Default game time: 7:15 PM
                    : DateTime.Today.Add(new TimeSpan(19, 15, 0)),
                ScheduledGames = scheduledGamesList,
                TieBreakGame = false // Default value
            };
        }

        /// <summary>
        /// Creates a schedule admin list view model for the current year.
        /// This method retrieves all scheduled games and weeks for the current NFL year,
        /// then constructs a summary of games per week.
        /// </summary>
        /// <returns>
        /// An asynchronous <see cref="Task{ScheduleAdminListViewModel}"/> containing 
        /// a list of weeks with their respective game counts and start dates.
        /// </returns>
        public async Task<ScheduleAdminListViewModel> CreateScheduleListViewModelAsync()
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Fetch the current NFL year from a shared service
            var year = await _commonDataService.GetCurrentYearAsync();

            // ✅ Retrieve all scheduled games for the given year asynchronously
            var schedules = await context.TeamSchedules
                .AsNoTracking()
                .Where(x => x.Year == year)
                .ToListAsync();

            // ✅ Retrieve all NFL weeks for the given year asynchronously
            var weeks = await context.NflWeeks
                .AsTracking()
                .Where(x => x.Year == year)
                .ToListAsync();

            // ✅ Initialize a list to store weekly schedule summaries
            var returnWeek = new List<ScheduleAdminWeekModel>();

            // ✅ Iterate through each week and count the scheduled games
            foreach (var week in weeks)
            {
                returnWeek.Add(new ScheduleAdminWeekModel
                {
                    Week = week.Name, // Week number
                    Year = year, // Current year
                    Games = schedules.Count(x => x.Week == week.Name), // Number of games in this week
                    StartDate = week.StartDate.ToShortDateString() // Start date of the week
                });
            }

            // ✅ Construct and return the final view model
            return new ScheduleAdminListViewModel
            {
                Week = returnWeek
            };
        }

        /// <summary>
        /// Asynchronously processes and saves a new game schedule to the database.
        /// </summary>
        /// <param name="model">
        /// A <see cref="ScheduleAdminViewModel"/> containing the week, year, teams, game time, and tiebreaker status.
        /// </param>
        /// <returns>
        /// A <see cref="Task{bool}"/> indicating whether the schedule was successfully saved (true) or failed (false).
        /// </returns>
        public async Task<bool> SaveScheduleAsync(ScheduleAdminViewModel model)
        {
            try
            {
                await using var context = _dbContextFactory.CreateDbContext();

                // ✅ Create a new TeamSchedule entity with data from the provided model
                var game = new TeamSchedule
                {
                    Week = model.Week, // The week number of the game
                    Year = model.Year, // The season year
                    GameStartDateTime = DateTime.Parse(
                        model.ScheduleDate.ToShortDateString() + " " +
                        model.ScheduleTime.ToShortTimeString()), // Combining date and time into DateTime
                    HomeTeamId = model.HomeTeamId, // Home team ID
                    AwayTeamId = model.AwayTeamId, // Away team ID
                    TieBreakGame = model.TieBreakGame // Indicates if this game is a tiebreaker
                };

                // ✅ Add the new game to the database and persist changes asynchronously
                context.TeamSchedules.Add(game);
                await context.SaveChangesAsync();

                return true; // ✅ Success: Schedule saved successfully
            }
            catch (Exception ex)
            {
                // ❌ Log any exceptions encountered while saving the schedule
                Console.Error.WriteLine($"Error saving schedule: {ex.Message}");
                return false; // ❌ Failure: An error occurred
            }
        }

        /// <summary>
        /// Asynchronously retrieves a user’s editable profile details for editing.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose information is being retrieved.
        /// </param>
        /// <returns>
        /// A <see cref="Task{UserModel}"/> representing the asynchronous operation, 
        /// containing the user’s editable details if found; otherwise, null.
        /// </returns>
        public async Task<UserModel> CreateEditModelAsync(string userId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Query the database for the specified user while ensuring no tracking for performance
            var user = await context.ApplicationUsers
                .AsNoTracking()
                .Select(x => new UserModel
                {
                    Email = x.Email,             // User's email address
                    UserTeamName = x.UserTeamName, // User's team name (if applicable)
                    Paid = x.Paid == true,        // Indicates if the user has completed payment
                    PaypalEmail = x.PaypalEmail,  // PayPal email address for transactions
                    Survival = x.Survival == true,// Indicates participation in survival mode
                    UserId = x.Id,                // Unique identifier of the user
                    UserName = x.UserName         // Display name of the user
                })
                .FirstOrDefaultAsync(x => x.UserId == userId); // Retrieves the user matching the given ID

            return user; // ✅ Returns the user model or null if not found
        }

        /// <summary>
        /// Asynchronously updates and saves administrative user profile properties.
        /// </summary>
        /// <param name="model">
        /// The <see cref="UserModel"/> containing the user's updated profile information.
        /// </param>
        /// <returns>
        /// A <see cref="Task{bool}"/> indicating whether the update was successful.
        /// Returns <c>true</c> if the update was successful; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ProcessEditModelAsync(UserModel model)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Fetch the user from the database based on the provided UserId
            var user = await context.ApplicationUsers
                .FirstOrDefaultAsync(m => m.Id == model.UserId);

            // ✅ If the user is not found, return false
            if (user == null)
                return false;

            // ✅ Update user properties with new values
            user.PaypalEmail = model.PaypalEmail; // Update PayPal email
            user.Survival = model.Survival;       // Update survival mode participation
            user.Paid = model.Paid;               // Update payment status

            // ✅ Mark the entity as modified and save changes asynchronously
            context.ApplicationUsers.Update(user);
            await context.SaveChangesAsync();

            return true; // ✅ Indicate success
        }

        /// <summary>
        /// Asynchronously updates a user's 'paid' status in the database.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose payment status is being updated.
        /// </param>
        /// <param name="paid">
        /// The new 'paid' status to be set (true = paid, false = unpaid).
        /// </param>
        /// <returns>
        /// A <see cref="Task{bool}"/> indicating whether the update was successful.
        /// Returns <c>true</c> if the update was successful; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> UpdateUserPaidAsync(string userId, bool paid)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Retrieve the user based on the provided UserId
            var user = await context.ApplicationUsers.FirstOrDefaultAsync(m => m.Id == userId);

            // ✅ Return false if the user is not found
            if (user == null)
            {
                return false;
            }

            // ✅ Update the user's 'Paid' status
            user.Paid = paid;
            context.ApplicationUsers.Update(user);

            // ✅ Save changes asynchronously and return true upon success
            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Asynchronously updates a user's 'survivor' status in the database.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose survivor status is being updated.
        /// </param>
        /// <param name="survival">
        /// The new 'survivor' status to be set (true = enrolled in survivor mode, false = not enrolled).
        /// </param>
        /// <returns>
        /// A <see cref="Task{bool}"/> indicating whether the update was successful.
        /// Returns <c>true</c> if the update was successful; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> UpdateUserSurvivorAsync(string userId, bool survival)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Retrieve the user based on the provided UserId
            var user = await context.ApplicationUsers.FirstOrDefaultAsync(m => m.Id == userId);

            // ✅ Return false if the user is not found
            if (user == null)
            {
                return false;
            }

            // ✅ Update the user's 'Survival' status
            user.Survival = survival;
            context.ApplicationUsers.Update(user);

            // ✅ Save changes asynchronously and return true upon success
            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Asynchronously retrieves a list of weekly winners and prepares a view model
        /// containing relevant data for a given year and week.
        /// </summary>
        /// <param name="year">
        /// The year for which to retrieve weekly winners.
        /// </param>
        /// <param name="week">
        /// The week for which to retrieve weekly winners.
        /// </param>
        /// <returns>
        /// A <see cref="Task{WeeklyWinnersViewModel}"/> containing:
        /// - A list of winners.
        /// - The next available place number.
        /// - A dropdown list of paid users.
        /// </returns>
        public async Task<WeeklyWinnersViewModel> GetWeeklyWinnersViewModelAsync(int year, int week)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Retrieve weekly winners for the specified year and week
            var weeklyWinners = await context.WeeklyWinners
                .AsNoTracking()
                .Where(x => x.Year == year && x.Week == week)
                .Select(x => new WeeklyWinnerModel
                {                    
                    UserName = string.IsNullOrWhiteSpace(x.UserProfile.UserTeamName) ? x.UserProfile.UserName : x.UserProfile.UserTeamName,
                    Place = x.Place ?? 0, // Default place to 0 if null
                    Week = x.Week,
                    Year = x.Year
                })
                .ToListAsync();

            // ✅ Retrieve a list of users who have paid, formatted for dropdown selection
            var paidUsers = await context.ApplicationUsers
                .AsNoTracking()
                .Where(x => x.Paid == true)
                .OrderBy(x => x.UserTeamName ?? x.UserName) // Sort at database level
                .Select(x => new DropdownModel
                {
                    Text = string.IsNullOrWhiteSpace(x.UserTeamName) ? x.UserName : x.UserTeamName, // Use team name if available
                    Value = x.Id
                })
                .ToListAsync();

            // ✅ Construct and return the view model
            var vm = new WeeklyWinnersViewModel
            {
                Place = weeklyWinners.Count + 1, // Next available place number
                Week = week,
                Year = year,
                Winners = weeklyWinners, // List of winners
                UserList = paidUsers // Dropdown list of paid users
            };

            return vm;
        }

        /// <summary>
        /// Asynchronously updates or inserts a weekly winner record.
        /// </summary>
        /// <param name="model">
        /// The <see cref="WeeklyWinnersViewModel"/> containing the winner's data.
        /// </param>
        /// <returns>
        /// A <see cref="Task{bool}"/> indicating success (<c>true</c>) or failure (<c>false</c>).
        /// </returns>
        public async Task<bool> SaveWeeklyWinnerAsync(WeeklyWinnersViewModel model)
        {
            try
            {
                await using var context = _dbContextFactory.CreateDbContext();

                // ✅ Check if a winner already exists for this week, year, and place
                var existingWinner = await context.WeeklyWinners
                    .FirstOrDefaultAsync(w => w.Year == model.Year && w.Week == model.Week && w.Place == model.Place);

                if (existingWinner != null)
                {
                    // ✅ Update existing record with the new user ID
                    existingWinner.UserId = model.UserId;
                }
                else
                {
                    // ✅ Insert a new record if no existing winner is found
                    var winner = new WeeklyWinner
                    {
                        Year = model.Year,
                        Week = model.Week,
                        UserId = model.UserId,
                        Place = model.Place
                    };

                    context.WeeklyWinners.Add(winner);
                }

                // ✅ Save changes asynchronously
                await context.SaveChangesAsync();
                return true; // ✅ Success
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving weekly winner: {ex.Message}");
                return false; // ❌ Failure
            }
        }

        /// <summary>
        /// Asynchronously deletes a scheduled game by its unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the scheduled game to be deleted.
        /// </param>
        /// <returns>
        /// A <see cref="Task{bool}"/> indicating whether the deletion was successful.
        /// Returns <c>true</c> if the game was found and deleted, otherwise <c>false</c>.
        /// </returns>
        public async Task<bool> DeleteScheduledGameAsync(int id)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Attempt to find the scheduled game by ID
            var game = await context.TeamSchedules
                .FirstOrDefaultAsync(x => x.TeamScheduleId == id);

            if (game == null)
                return false; // ❌ Game not found, return false

            // ✅ Remove the game from the database
            context.TeamSchedules.Remove(game);

            // ✅ Save changes asynchronously
            await context.SaveChangesAsync();
            return true; // ✅ Success
        }

        /// <summary>
        /// Asynchronously toggles the <see cref="TieBreakGame"/> property for a scheduled game.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the scheduled game to update.
        /// </param>
        /// <returns>
        /// A <see cref="Task{bool}"/> indicating whether the update was successful.
        /// Returns <c>true</c> if the game was found and updated, otherwise <c>false</c>.
        /// </returns>
        public async Task<bool> UpdateGameTiebreakAsync(int id)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Attempt to find the scheduled game by ID
            var game = await context.TeamSchedules
                .FirstOrDefaultAsync(x => x.TeamScheduleId == id);

            if (game == null)
                return false; // ❌ Game not found, return false

            // ✅ Toggle the TieBreakGame property
            game.TieBreakGame = !game.TieBreakGame;

            // ✅ Update the game entity in the database
            context.TeamSchedules.Update(game);

            // ✅ Save changes asynchronously
            await context.SaveChangesAsync();
            return true; // ✅ Success
        }

        /// <summary>
        /// Asynchronously sends an email to league members, optionally filtering by paid-only users.
        /// </summary>
        /// <param name="model">
        /// The <see cref="EmailLeagueViewModel"/> containing email subject, message, and other settings.
        /// </param>
        /// <returns>
        /// An <see cref="EmailLeagueViewModel"/> with status messages appended for each email sent.
        /// </returns>
        public async Task<EmailLeagueViewModel> SendEmailToLeagueAsync(EmailLeagueViewModel model)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Retrieve users (filter if PaidOnly is true)
            var userQuery = context.ApplicationUsers.AsNoTracking();

            if (model.PaidOnly)
            {
                userQuery = userQuery.Where(x => x.Paid == true);
            }

            var users = await userQuery.ToListAsync();

            // ✅ Send email to each user
            foreach (var user in users)
            {
                try
                {
                    // ✅ Prepare the email message
                    using var eMail = new System.Net.Mail.MailMessage
                    {
                        IsBodyHtml = true,
                        Body = model.EmailMessage,
                        From = new System.Net.Mail.MailAddress("mail@philphan.com"), // TODO: ⚠️ REPLACE WITH CONFIGURED EMAIL
                        Subject = model.EmailSubject
                    };

                    eMail.To.Add(user.Email);

                    // ✅ Configure SMTP client 
                    using var smtpClient = new System.Net.Mail.SmtpClient("lilac.arvixe.com")
                    {
                        Credentials = new System.Net.NetworkCredential(
                            "mail@philphan.com", // TODO: ⚠️ REPLACE WITH CONFIGURED EMAIL
                            "XXX"   //TODO: move to appsettings.json
                        )
                    };

                    // ✅ Send email asynchronously
                    await smtpClient.SendMailAsync(eMail);

                    model.Message += $"✅ Email sent to: {user.Email}<br>";
                }
                catch (Exception ex)
                {
                    // ❌ Log error and append to model message
                    model.Message += $"❌ Error sending to {user.Email}: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        model.Message += $": {ex.InnerException.Message}";
                    }
                    model.Message += "<br>";
                }
            }

            return model;
        }

        /// <summary>
        /// Asynchronously updates the tie-breaker scores for a given schedule.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule record to update.</param>
        /// <param name="homeScore">The new home team score (if provided).</param>
        /// <param name="awayScore">The new away team score (if provided).</param>
        /// <returns>A <see cref="Task{bool}"/> indicating success or failure.</returns>
        public async Task<bool> UpdateTieBreakScoresAsync(int scheduleId, int? homeScore, int? awayScore)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // ✅ Retrieve the game record (tracked for update)
            var game = await context.TeamSchedules.FirstOrDefaultAsync(x => x.TeamScheduleId == scheduleId);

            if (game == null)
            {
                Console.WriteLine($"❌ Update Failed: Game ID {scheduleId} not found.");
                return false;
            }

            // ✅ Ensure scores are changing before saving
            bool isUpdated = false;

            if (homeScore.HasValue && game.HomeTeamScore != homeScore)
            {
                game.HomeTeamScore = homeScore.Value;
                isUpdated = true;
            }

            if (awayScore.HasValue && game.AwayTeamScore != awayScore)
            {
                game.AwayTeamScore = awayScore.Value;
                isUpdated = true;
            }

            // ✅ If no changes, avoid unnecessary DB calls
            if (!isUpdated)
            {
                Console.WriteLine($"ℹ️ No score changes detected for Game ID {scheduleId}.");
                return true;
            }

            // ✅ Save changes
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Scores updated for Game ID {scheduleId}: Home {game.HomeTeamScore}, Away {game.AwayTeamScore}");
            return true;
        }

        /// <summary>
        /// Asynchronously retrieves the first alert record and maps it to an UpdateAlertsViewModel.
        /// </summary>
        /// <returns>A <see cref="Task{UpdateAlertsViewModel}"/> with the current alert messages.</returns>
        public async Task<UpdateAlertsViewModel> GetAlertsViewModelAsync()
        {
            await using var context = _dbContextFactory.CreateDbContext();

            var alert = await context.Alerts.FirstOrDefaultAsync(); // ✅ Handles empty table gracefully

            if (alert == null)
            {
                Console.WriteLine("⚠️ No alert record found in the database. Returning default values.");
                return new UpdateAlertsViewModel
                {
                    IndexPageAlert = string.Empty,  // ✅ Prevents null references in UI
                    MyTeamPageAlert = string.Empty
                };
            }

            return new UpdateAlertsViewModel
            {
                IndexPageAlert = alert.IndexPageAlert,
                MyTeamPageAlert = alert.MyTeamPageAlert
            };
        }

        /// <summary>
        /// Asynchronously updates the alert messages for the Index and MyTeam pages.
        /// If no alert exists, a new record is inserted.
        /// </summary>
        /// <param name="indexPageAlert">The new alert message for the Index page.</param>
        /// <param name="myTeamPageAlert">The new alert message for the MyTeam page.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UpdateAlertAsync(string indexPageAlert, string myTeamPageAlert)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            var alert = await context.Alerts.FirstOrDefaultAsync(); // ✅ Handles empty table safely

            if (alert != null)
            {
                // ✅ Update existing alert record
                alert.IndexPageAlert = indexPageAlert;
                alert.MyTeamPageAlert = myTeamPageAlert;
                context.Alerts.Update(alert);
            }
            else
            {
                // ✅ Insert new alert if none exist
                Console.WriteLine("⚠️ No existing alert found. Creating a new alert record.");
                alert = new Alert
                {
                    IndexPageAlert = indexPageAlert,
                    MyTeamPageAlert = myTeamPageAlert
                };
                await context.Alerts.AddAsync(alert);
            }

            await context.SaveChangesAsync();
        }
    }
}

