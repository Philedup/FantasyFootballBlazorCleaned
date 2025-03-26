
using FantasyFootballBlazor.Data;
using FantasyFootballBlazor.Data.Models.Entities;
using FantasyFootballBlazor.Factories;
using FantasyFootballBlazor.Services;
using FantasyFootball.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using FantasyFootball.Shared;

namespace FantasyFootballBlazor.Tests.Services
{
    public class FootballDataServiceTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private readonly Mock<ICommonDataService> _commonDataServiceMock;
        private readonly Mock<IWeeklyPickFactory> _weeklyPickFactoryMock;
        private readonly Mock<ITimeProvider> _timeProviderMock;
        private readonly Mock<IPlayerPositionStatFactory> _playerStatFactoryMock;

        public FootballDataServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _commonDataServiceMock = new Mock<ICommonDataService>();
            _weeklyPickFactoryMock = new Mock<IWeeklyPickFactory>();
            _timeProviderMock = new Mock<ITimeProvider>();
            _playerStatFactoryMock = new Mock<IPlayerPositionStatFactory>();
        }

        private FootballDataService CreateService(ApplicationDbContext context)
        {
            var dbFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            dbFactoryMock.Setup(f => f.CreateDbContext()).Returns(context);

            return new FootballDataService(
                dbFactoryMock.Object,
                _commonDataServiceMock.Object,
                _weeklyPickFactoryMock.Object,
                _timeProviderMock.Object,
                _playerStatFactoryMock.Object
            );
        }

        [Fact]
        public async Task GetPrizePotAsync_ReturnsCorrectPrizePots()
        {
            await using var context = new ApplicationDbContext(_dbOptions);

            context.PrizePots.AddRange(new List<PrizePot>
            {
                new PrizePot { PrizePotId = 1, GameTypeId = 1, Place = 1, Prize = 100 },
                new PrizePot { PrizePotId = 2, GameTypeId = 1, Place = 2, Prize = 50 },
                new PrizePot { PrizePotId = 3, GameTypeId = 2, Place = 1, Prize = 75 }
            });

            await context.SaveChangesAsync();
            var service = CreateService(context);

            var result = await service.GetPrizePotAsync(1);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Place == 1 && p.Prize == 100);
            Assert.Contains(result, p => p.Place == 2 && p.Prize == 50);
        }

        [Fact]
        public async Task GetPlayerStatAsync_ReturnsCorrectPlayerStats()
        {
            await using var context = new ApplicationDbContext(_dbOptions);

            context.Players.Add(new Player
            {
                PlayerId = 1,
                First = "Tom",
                Last = "Brady",
                Position = "QB",
                TeamId = 5,
                PlayerPictureImageUrl = "url",
                FullName = "Tom Brady"
            });

            context.WeeklyStats.Add(new WeeklyStat
            {
                PlayerId = 1,
                Week = 1,
                Year = 2023,
                PassingTouchdowns = 3,
                PassingYards = 300,
                TotalPoints = 25
            });

            await context.SaveChangesAsync();
            var service = CreateService(context);

            var result = await service.GetPlayerStatAsync(1, 2023);

            Assert.NotNull(result);
            Assert.Equal("Tom", result.First);
            Assert.Single(result.PlayerStats);
            Assert.Equal(25, result.PlayerStats[0].TotalPoints);
        }

        [Fact]
        public async Task SaveTieBreakerScoreAsync_InsertsNewScore()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            var context = new ApplicationDbContext(options);
            var service = CreateService(context);

            var result = await service.SaveTieBreakerScoreAsync("user1", 100, 45);

            Assert.True(result);

            await using var verifyContext = new ApplicationDbContext(options);
            var saved = await verifyContext.UserTieBreakers.FirstOrDefaultAsync();

            Assert.NotNull(saved);
            Assert.Equal("user1", saved.UserId);
            Assert.Equal(100, saved.ScheduleId);
            Assert.Equal(45, saved.TotalScore);
        }

        [Fact]
        public async Task SaveTieBreakerScoreAsync_UpdatesExistingScore()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            await using (var seedContext = new ApplicationDbContext(options))
            {
                seedContext.UserTieBreakers.Add(new UserTieBreaker
                {
                    UserId = "user1",
                    ScheduleId = 100,
                    TotalScore = 30
                });
                await seedContext.SaveChangesAsync();
            }

            var service = CreateService(new ApplicationDbContext(options));
            var result = await service.SaveTieBreakerScoreAsync("user1", 100, 60);

            Assert.True(result);

            await using var verifyContext = new ApplicationDbContext(options);
            var updated = await verifyContext.UserTieBreakers.FirstOrDefaultAsync();

            Assert.NotNull(updated);
            Assert.Equal("user1", updated.UserId);
            Assert.Equal(100, updated.ScheduleId);
            Assert.Equal(60, updated.TotalScore);
        }

        [Fact]
        public async Task GetIndexViewModelAsync_ReturnsExpectedModel()
        {
            // Arrange: use a named in-memory DB so all contexts share the same data
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            // Seed data
            await using (var seedContext = new ApplicationDbContext(options))
            {
                seedContext.Alerts.Add(new Alert { IndexPageAlert = "Index Alert!", MyTeamPageAlert = "My Team Alert!" });
                seedContext.TeamSchedules.Add(new TeamSchedule { Week = 1, Year = 2025, TieBreakGame = true });
                await seedContext.SaveChangesAsync();
            }

            // Setup factory to always return fresh context
            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(() => new ApplicationDbContext(options));

            _commonDataServiceMock.Setup(x => x.GetUsersWeeklyPicksAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<PlayerModel>());
            _commonDataServiceMock.Setup(x => x.GetTeamLockStatusListAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<TeamLockStatusModel>());

            var service = new FootballDataService(
                factoryMock.Object,
                _commonDataServiceMock.Object,
                _weeklyPickFactoryMock.Object,
                _timeProviderMock.Object,
                _playerStatFactoryMock.Object
            );

            // Act
            var result = await service.GetIndexViewModelAsync(currentWeek: 1, selectedWeek: 1, year: 2025);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CurrentWeek);
            Assert.Equal("Index Alert!", result.AlertMessage);
        }

        [Fact]
        public async Task CreateEarningsViewModelAsync_ReturnsUsersWithEarnings()
        {
            await using var context = new ApplicationDbContext(_dbOptions);

            context.PrizePots.AddRange(
                new PrizePot { GameTypeId = 1, Place = 1, Prize = 100 },
                new PrizePot { GameTypeId = 1, Place = 2, Prize = 50 }
            );

            context.ApplicationUsers.Add(new ApplicationUser { Id = "u1", UserName = "John", Paid = true, PaypalEmail = "test@test.com", UserTeamName = "Team1" });
            context.WeeklyWinners.Add(new WeeklyWinner { Year = 2025, UserId = "u1", Place = 1 });

            await context.SaveChangesAsync();

            var service = CreateService(context);
            var result = await service.CreateEarningsViewModelAsync(2025);

            Assert.NotNull(result);
            Assert.Single(result.Users);
            Assert.Equal(100, result.Users[0].TotalEarned);
        }

        [Fact]
        public async Task CreateWeeklyWinnersViewModelAsync_ReturnsWinnersWithPrizes()
        {
            await using var context = new ApplicationDbContext(_dbOptions);

            context.PrizePots.Add(new PrizePot { GameTypeId = 1, Place = 1, Prize = 100 });
            context.ApplicationUsers.Add(new ApplicationUser { Id = "u1", UserName = "Player1", UserTeamName = "Team1", PaypalEmail = "test@test.com" });
            context.WeeklyWinners.Add(new WeeklyWinner { Year = 2025, UserId = "u1", Place = 1, Week = 1 });

            await context.SaveChangesAsync();

            var service = CreateService(context);
            var result = await service.CreateWeeklyWinnersViewModelAsync(2025);

            Assert.NotNull(result);
            Assert.Single(result.Winners);
            Assert.Equal(100, result.Winners[0].Total);
            Assert.Equal("Team1", result.Winners[0].UserName);
        }

        [Fact]
        public async Task GetUserWeeklyPicksDialogViewModelAsync_ReturnsCorrectData()
        {
            await using var context = new ApplicationDbContext(_dbOptions);

            context.ApplicationUsers.Add(new ApplicationUser { Id = "u1", UserName = "TestUser", UserTeamName = "TestTeam", PaypalEmail = "test@tes.com" });
            await context.SaveChangesAsync();

            _commonDataServiceMock.Setup(x =>
                x.GetUsersWeeklyPicksAsync("u1", It.IsAny<int>(), 2025, 1))
                .ReturnsAsync(new List<PlayerModel>
                {
            new PlayerModel { Position = "QB", FullName = "QB Guy", TotalPoints = 10 },
            new PlayerModel { Position = "RB", FullName = "RB Guy", TotalPoints = 8 }
                });

            var service = CreateService(context);

            // Use currentWeek = 2 to only run for week 1
            var result = await service.GetUserWeeklyPicksDialogViewModelAsync("u1", 2, 2025, 1);

            Assert.NotNull(result);
            Assert.Equal("TestTeam", result.UserName);
            Assert.Single(result.WeeklyPicks);
            Assert.Equal(18, result.WeeklyPicks[0].PlayerTotalPoints);
        }

        [Fact]
        public async Task GetMyTeamViewModelAsync_ReturnsExpectedData()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            // Seed a paid user and an alert
            await using (var seedContext = new ApplicationDbContext(options))
            {
                seedContext.ApplicationUsers.Add(new ApplicationUser
                {
                    Id = "u1",
                    UserName = "TestUser",
                    Paid = true,
                    Survival = true,
                    UserTeamName = "TestTeam",
                    PaypalEmail = "tester@test.com"

                });

                seedContext.Alerts.Add(new Alert
                {
                    MyTeamPageAlert = "Watch out!",
                    IndexPageAlert = "Index Alert!"
                });

                await seedContext.SaveChangesAsync();
            }

            // Mock picks returned by ICommonDataService
            var weeklyPick = new PlayerModel { PlayerId = 10, Position = "QB", TeamId = 1, FullName = "Weekly QB" };
            var survivorPick = new PlayerModel { PlayerId = 20, Position = "QB", TeamId = 2, FullName = "Survivor QB" };

            _commonDataServiceMock.Setup(x =>
                x.GetUsersWeeklyPicksAsync("u1", 1, 2025, 1))
                .ReturnsAsync(new List<PlayerModel> { weeklyPick });

            _commonDataServiceMock.Setup(x =>
                x.GetUsersWeeklyPicksAsync("u1", 1, 2025, 2))
                .ReturnsAsync(new List<PlayerModel> { survivorPick });

            _commonDataServiceMock.Setup(x =>
                x.GetTeamLockStatusListAsync(1, 2025))
                .ReturnsAsync(new List<TeamLockStatusModel>
                {
            new TeamLockStatusModel { TeamId = 1, IsLocked = true },
            new TeamLockStatusModel { TeamId = 2, IsLocked = true }
                });

            // Mock player detail creation and stat population
            _playerStatFactoryMock.Setup(f => f.CreatePlayerPositionDetails(It.IsAny<List<PlayerModel>>(), "QB"))
                .Returns((List<PlayerModel> picks, string _) => picks.First());

            _playerStatFactoryMock.Setup(f => f.CreatePlayerPositionDetails(It.IsAny<List<PlayerModel>>(), It.IsNotIn("QB")))
                .Returns(new PlayerModel()); // Empty for other positions

            _timeProviderMock.Setup(tp => tp.Now).Returns(DateTime.UtcNow);
            _commonDataServiceMock.Setup(c => c.DaylightSavingsAdjustment()).Returns(0);

            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(() => new ApplicationDbContext(options));

            var service = new FootballDataService(
                factoryMock.Object,
                _commonDataServiceMock.Object,
                _weeklyPickFactoryMock.Object,
                _timeProviderMock.Object,
                _playerStatFactoryMock.Object
            );

            // Act
            var result = await service.GetMyTeamViewModelAsync("u1", selectedWeek: 1, year: 2025, currentWeek: 2);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Paid);
            Assert.True(result.Survivor);
            Assert.Equal("Watch out!", result.AlertMessage);
            Assert.Equal(5, result.WeeklyPicksLeft); // 1 weekly pick made
            Assert.Equal(5, result.SurvivorPicksLeft); // 1 survivor pick made
            Assert.Equal("Weekly QB", result.QuarterBack?.FullName);
            Assert.Equal("Survivor QB", result.SurvivorQuarterBack?.FullName);
            Assert.True(result.QuarterBack.Locked); // Team 1 is locked
            Assert.True(result.SurvivorQuarterBack.Locked); // Survivor is always locked after week 1
        }

        [Fact]
        public async Task CreateRankingsViewModelAsync_ReturnsRanksAndPrizePots()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            await using (var seedContext = new ApplicationDbContext(options))
            {
                seedContext.ApplicationUsers.Add(new ApplicationUser
                {
                    Id = "u1",
                    UserName = "User1",
                    Paid = true,
                    Survival = true,
                    UserTeamName = "Team1",
                    PaypalEmail = "test@test.com"
                });

                seedContext.WeeklyUserTeams.Add(new WeeklyUserTeam
                {
                    UserId = "u1",
                    Position = "QB",
                    PlayerId = 1,
                    Week = 1,
                    Year = 2025,
                    GameTypeId = 1
                });

                seedContext.WeeklyStats.Add(new WeeklyStat
                {
                    PlayerId = 1,
                    Week = 1,
                    Year = 2025,
                    TotalPoints = 30
                });

                seedContext.PrizePots.AddRange(
                    new PrizePot { GameTypeId = 2, Place = 1, Prize = 100 },
                    new PrizePot { GameTypeId = 3, Place = 1, Prize = 50 }
                );

                await seedContext.SaveChangesAsync();
            }

            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(() => new ApplicationDbContext(options));

            var service = new FootballDataService(factoryMock.Object, _commonDataServiceMock.Object, _weeklyPickFactoryMock.Object, _timeProviderMock.Object, _playerStatFactoryMock.Object);

            var result = await service.CreateRankingsViewModelAsync(2025);

            Assert.NotNull(result);
            Assert.Single(result.WeeklyRanks);
            Assert.Single(result.SurvivalRanks);
            Assert.Equal(100, result.PrizePotSurvivor.First().Prize);
            Assert.Equal(50, result.PrizePotTotalPoints.First().Prize);
        }

        [Fact]
        public async Task GetUserRankingsAsync_ReturnsOrderedRankings()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            await using (var seedContext = new ApplicationDbContext(options))
            {
                seedContext.ApplicationUsers.AddRange(
                    new ApplicationUser { Id = "u1", UserName = "Alpha", Paid = true, UserTeamName = "Team1", PaypalEmail = "test@test.com" },
                    new ApplicationUser { Id = "u2", UserName = "Beta", Paid = true, UserTeamName = "Team1", PaypalEmail = "test@test.com" }
                );

                seedContext.WeeklyUserTeams.AddRange(
                    new WeeklyUserTeam { UserId = "u1", PlayerId = 1, Week = 1, Year = 2025, GameTypeId = 1, Position = "QB" },
                    new WeeklyUserTeam { UserId = "u2", PlayerId = 2, Week = 1, Year = 2025, GameTypeId = 1, Position = "QB" }
                );

                seedContext.WeeklyStats.AddRange(
                    new WeeklyStat { PlayerId = 1, Week = 1, Year = 2025, TotalPoints = 10 },
                    new WeeklyStat { PlayerId = 2, Week = 1, Year = 2025, TotalPoints = 20 }
                );

                await seedContext.SaveChangesAsync();
            }

            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(() => new ApplicationDbContext(options));

            var service = new FootballDataService(factoryMock.Object, _commonDataServiceMock.Object, _weeklyPickFactoryMock.Object, _timeProviderMock.Object, _playerStatFactoryMock.Object);

            var result = await service.GetUserRankingsAsync(2025, 1);

            Assert.Equal(2, result.Count);
            Assert.Equal("Team1", result[0].DisplayText); // Both use same UserTeamName
            Assert.Equal(1, result[0].Rank);
            Assert.Equal(2, result[1].Rank);
        }

        [Fact]
        public async Task GetAllPlayersWeeklyRosterAsync_ReturnsFormattedWeeklyPicks()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            await using (var seedContext = new ApplicationDbContext(options))
            {
                seedContext.ApplicationUsers.Add(new ApplicationUser
                {
                    Id = "u1",
                    UserName = "Tester",
                    Paid = true,
                    UserTeamName = "Team1",
                    PaypalEmail = "test@test.com"
                });

                seedContext.WeeklyUserTeams.Add(new WeeklyUserTeam
                {
                    UserId = "u1",
                    PlayerId = 1,
                    Week = 1,
                    Year = 2025,
                    GameTypeId = 1,
                    Position = "QB"
                });

                seedContext.Players.Add(new Player
                {
                    PlayerId = 1,
                    FullName = "QB Guy",
                    Position = "QB",
                    TeamId = 1,
                    PlayerPictureImageUrl = "url",
                    First = "Quarter",
                    Last = "Back"
                });

                seedContext.WeeklyStats.Add(new WeeklyStat
                {
                    PlayerId = 1,
                    Week = 1,
                    Year = 2025,
                    TotalPoints = 15
                });

                seedContext.UserTieBreakers.Add(new UserTieBreaker
                {
                    UserId = "u1",
                    ScheduleId = 999,
                    TotalScore = 45,
                    TeamSchedule = new TeamSchedule { TeamScheduleId = 999, Week = 1, Year = 2025 }
                });

                await seedContext.SaveChangesAsync();
            }

            _commonDataServiceMock.Setup(x => x.GetTeamLockStatusListAsync(1, 2025))
                .ReturnsAsync(new List<TeamLockStatusModel>());

            _weeklyPickFactoryMock.Setup(f => f.CreateUserWeeklyPick(
                It.IsAny<int>(),
                It.IsAny<List<Player>>(),
                It.IsAny<IEnumerable<WeeklyUserTeam>>(),
                It.IsAny<List<TeamLockStatusModel>>(),
                It.IsAny<ApplicationUser>(),
                It.IsAny<List<WeeklyStat>>(),
                It.IsAny<List<UserTieBreaker>>(),
                It.IsAny<List<TeamSchedule>>()))
                .Returns(new WeeklyPicksModel
                {
                    UserId = "u1",
                    PlayerTotalPoints = 15,
                    QuarterBackName = "QB Guy"
                });

            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(() => new ApplicationDbContext(options));

            var service = new FootballDataService(factoryMock.Object, _commonDataServiceMock.Object, _weeklyPickFactoryMock.Object, _timeProviderMock.Object, _playerStatFactoryMock.Object);
            var result = await service.GetAllPlayersWeeklyRosterAsync(1, 2025, 1, new List<TeamSchedule>());

            Assert.Single(result);
            Assert.Equal("u1", result[0].UserId);
            Assert.Equal("QB Guy", result[0].QuarterBackName);
            Assert.Equal(15, result[0].PlayerTotalPoints);
        }

        [Fact]
        public async Task PopulateStatsAsync_PopulatesPlayerStats()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            await using (var seedContext = new ApplicationDbContext(options))
            {
                seedContext.WeeklyStats.Add(new WeeklyStat
                {
                    PlayerId = 1,
                    Week = 1,
                    Year = 2025,
                    TotalPoints = 25,
                    PassingYards = 300
                });

                await seedContext.SaveChangesAsync();
            }

            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(() => new ApplicationDbContext(options));

            var service = new FootballDataService(factoryMock.Object, _commonDataServiceMock.Object, _weeklyPickFactoryMock.Object, _timeProviderMock.Object, _playerStatFactoryMock.Object);

            var playerModel = new PlayerModel
            {
                PlayerId = 1
            };

            await service.PopulateStatsAsync(2025, playerModel);

            Assert.NotNull(playerModel.PlayerStats);
            Assert.Single(playerModel.PlayerStats);
            Assert.Equal(25, playerModel.PlayerStats[0].TotalPoints);
            Assert.Equal(300, playerModel.PlayerStats[0].PassingYards);
        }

    }
}