using FantasyFootballBlazor.Data;
using FantasyFootballBlazor.Data.Models.Entities;
using FantasyFootballBlazor.Services;
using FantasyFootball.Shared;
using FantasyFootball.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FantasyFootballBlazor.Tests.Services
{
    public class PlayersServiceTests
    {
        private readonly Mock<ICommonDataService> _commonDataServiceMock;
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

        public PlayersServiceTests()
        {
            _commonDataServiceMock = new Mock<ICommonDataService>();
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private PlayersService CreateService(DbContextOptions<ApplicationDbContext> options)
        {
            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext())
                .Returns(() => new ApplicationDbContext(options));

            return new PlayersService(factoryMock.Object, _commonDataServiceMock.Object);
        }

        [Fact]
        public async Task GetAllPlayersAsync_FiltersByPositionAndSortsDescending()
        {
            // Arrange
            var year = 2025;
            var week = 1;

            await using (var context = new ApplicationDbContext(_dbOptions))
            {
                context.Players.Add(new Player
                {
                    PlayerId = 1,
                    FullName = "Alpha QB",
                    First = "Alpha",
                    Last = "QB",
                    Position = "QB",
                    TeamId = 1,
                    LastUpdatedDateTime = DateTime.UtcNow,
                    PlayerPictureImageUrl = "url1",
                    WeeklyStats = new List<WeeklyStat>
            {
                new WeeklyStat { Year = year, TotalPoints = 50 }
            }
                });

                context.Players.Add(new Player
                {
                    PlayerId = 2,
                    FullName = "Beta QB",
                    First = "Beta",
                    Last = "QB",
                    Position = "QB",
                    TeamId = 2,
                    LastUpdatedDateTime = DateTime.UtcNow,
                    PlayerPictureImageUrl = "url2",
                    WeeklyStats = new List<WeeklyStat>
            {
                new WeeklyStat { Year = year, TotalPoints = 10 }
            }
                });

                context.Teams.AddRange(
                    new Team { TeamId = 1, Code = "AAA", City = "City1", Name = "c1" },
                    new Team { TeamId = 2, Code = "BBB", City = "City2", Name = "c2" });

                await context.SaveChangesAsync();
            }

            _commonDataServiceMock.Setup(x =>
                x.GetUsersWeeklyPicksAsync("u1", week, year, 1))
                .ReturnsAsync(new List<PlayerModel>());

            _commonDataServiceMock.Setup(x =>
                x.GetTeamLockStatusListAsync(week, year))
                .ReturnsAsync(new List<TeamLockStatusModel>
                {
            new TeamLockStatusModel { TeamId = 1, IsLocked = false, OpponentId = 2 },
            new TeamLockStatusModel { TeamId = 2, IsLocked = true, OpponentId = 1 }
                });

            var service = CreateService(_dbOptions);

            // Act
            var result = await service.GetAllPlayersAsync("QB", "", "u1", 1, year, week);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Players.Count);
            Assert.Equal("Alpha QB", result.Players.First().FullName); // Highest total points
            Assert.Equal("BBB", result.Players.First().Opponent);
        }


        [Fact]
        public async Task AddPlayerAsync_AddsWeeklyPick_WhenTeamNotLocked()
        {
            // Arrange
            var playerId = 99;
            var year = 2025;
            var week = 1;

            await using (var context = new ApplicationDbContext(_dbOptions))
            {
                context.Players.Add(new Player
                {
                    PlayerId = playerId,
                    First = "John",
                    Last = "Doe",
                    FullName = "John Doe",
                    Position = "QB",
                    TeamId = 1,
                    LastUpdatedDateTime = DateTime.UtcNow,
                    PlayerPictureImageUrl = "pic1"
                });
                await context.SaveChangesAsync();
            }

            _commonDataServiceMock.Setup(x => x.GetTeamLockStatusListAsync(week, year))
                .ReturnsAsync(new List<TeamLockStatusModel>
                {
            new TeamLockStatusModel { TeamId = 1, IsLocked = false }
                });

            var service = CreateService(_dbOptions);

            // Act
            var result = await service.AddPlayerAsync("u1", playerId, week, year, 1);

            // Assert
            Assert.True(result);

            await using var verifyContext = new ApplicationDbContext(_dbOptions);
            var pick = await verifyContext.WeeklyUserTeams.FirstOrDefaultAsync();
            Assert.NotNull(pick);
            Assert.Equal(playerId, pick.PlayerId);
            Assert.Equal("u1", pick.UserId);
        }

        [Fact]
        public async Task AddPlayerAsync_DoesNotAdd_WhenTeamLocked()
        {
            // Arrange
            var playerId = 100;
            var year = 2025;
            var week = 1;

            await using (var context = new ApplicationDbContext(_dbOptions))
            {
                context.Players.Add(new Player
                {
                    PlayerId = playerId,
                    First = "Jane",
                    Last = "Smith",
                    FullName = "Jane Smith",
                    Position = "QB",
                    TeamId = 1,
                    LastUpdatedDateTime = DateTime.UtcNow,
                    PlayerPictureImageUrl = "pic2"
                });
                await context.SaveChangesAsync();
            }

            _commonDataServiceMock.Setup(x => x.GetTeamLockStatusListAsync(week, year))
                .ReturnsAsync(new List<TeamLockStatusModel>
                {
            new TeamLockStatusModel { TeamId = 1, IsLocked = true }
                });

            var service = CreateService(_dbOptions);

            // Act
            var result = await service.AddPlayerAsync("u1", playerId, week, year, 1);

            // Assert
            Assert.True(result);

            await using var verifyContext = new ApplicationDbContext(_dbOptions);
            Assert.Empty(verifyContext.WeeklyUserTeams);
        }

        [Fact]
        public async Task AddPlayerAsync_AddsSurvivorPicks_AllWeeks()
        {
            // Arrange
            var playerId = 77;
            var year = 2025;
            var week = 1;

            await using (var context = new ApplicationDbContext(_dbOptions))
            {
                context.Players.Add(new Player
                {
                    PlayerId = playerId,
                    First = "Survivor",
                    Last = "Guy",
                    FullName = "Survivor Guy",
                    Position = "WR",
                    TeamId = 3,
                    LastUpdatedDateTime = DateTime.UtcNow,
                    PlayerPictureImageUrl = "pic3"
                });

                await context.SaveChangesAsync();
            }

            _commonDataServiceMock.Setup(x => x.GetTeamLockStatusListAsync(week, year))
                .ReturnsAsync(new List<TeamLockStatusModel>
                {
            new TeamLockStatusModel { TeamId = 3, IsLocked = false }
                });

            var service = CreateService(_dbOptions);

            // Act
            var result = await service.AddPlayerAsync("u1", playerId, week, year, 2); // Survivor game

            // Assert
            Assert.True(result);

            await using var verifyContext = new ApplicationDbContext(_dbOptions);
            var picks = await verifyContext.WeeklyUserTeams
                .Where(x => x.UserId == "u1" && x.GameTypeId == 2)
                .ToListAsync();

            Assert.Equal(18, picks.Count); // All weeks
            Assert.All(picks, p => Assert.Equal(playerId, p.PlayerId));
        }

    }
}
