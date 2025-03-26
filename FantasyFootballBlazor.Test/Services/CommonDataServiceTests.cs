using Moq;
using FantasyFootballBlazor.Services;
using FantasyFootball.Shared.Services;
using Microsoft.EntityFrameworkCore;
using FantasyFootballBlazor.Data;
using FantasyFootballBlazor.Data.Models.Entities;
namespace FantasyFootballBlazor.Tests.Services
{
    public class CommonDataServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CommonDataService _service;

        public CommonDataServiceTests()
        {
            // Setup in-memory DB
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            _dbContext = new ApplicationDbContext(options);

            // Seed fake NFL year
            _dbContext.NflYears.Add(new Data.Models.Entities.NflYear { NflYearId = 2024, Name = "2024" });
            _dbContext.SaveChanges();

            // Setup mock time provider
            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(t => t.Now).Returns(DateTime.UtcNow);

            // Wrap DbContext in a factory
            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(_dbContext);

            _service = new CommonDataService(factoryMock.Object, timeProvider.Object);
        }

        [Fact]
        public async Task GetCurrentYearAsync_ReturnsLatestYear()
        {
            // Act
            var result = await _service.GetCurrentYearAsync();

            // Assert
            Assert.Equal(2024, result);
        }

        [Theory]
        [InlineData("QB")]
        [InlineData("RB")]
        [InlineData("WR")]
        [InlineData("TE")]
        [InlineData("K")]
        [InlineData("DEF")]
        public void GetPlayerPositions_ContainsExpectedPosition(string expected)
        {
            var positions = _service.GetPlayerPositions();
            Assert.Contains(positions, p => p.Text == expected && p.Value == expected);
        }

        [Fact]
        public void DaylightSavingsAdjustment_ReturnsExpectedOffset()
        {
            // Act
            var result = _service.DaylightSavingsAdjustment();

            // Assert
            Assert.True(result == -5 || result == -6); // CDT or CST
        }

        [Fact]
        public async Task GetUsersWeeklyPicksAsync_ReturnsPlayersWithStats()
        {
            // Arrange
            var userId = "user123";
            var week = 1;
            var year = 2024;
            var gameTypeId = 1;

            // Create player and weekly stats
            var player = new Player
            {
                PlayerId = 1001,
                First = "John",
                Last = "Doe",
                FullName = "John Doe",
                Position = "QB",
                TeamId = 1,
                LastUpdatedDateTime = DateTime.UtcNow,
                PlayerPictureImageUrl = "http://example.com/john.jpg",
                WeeklyStats = new List<WeeklyStat>
        {
            new WeeklyStat
            {
                Week = week,
                Year = year,
                TotalPoints = 25
            }
        }
            };

            _dbContext.Players.Add(player);

            // Add user's weekly team pick
            _dbContext.WeeklyUserTeams.Add(new WeeklyUserTeam
            {
                UserId = userId,
                PlayerId = player.PlayerId,
                Week = week,
                Year = year,
                GameTypeId = gameTypeId,
                Position = player.Position
            });

            _dbContext.SaveChanges();

            // Act
            var result = await _service.GetUsersWeeklyPicksAsync(userId, week, year, gameTypeId);

            // Assert
            Assert.Single(result);
            var returnedPlayer = result.First();
            Assert.Equal(player.FullName, returnedPlayer.FullName);
            Assert.Equal(player.Position, returnedPlayer.Position);
            Assert.Equal(25, returnedPlayer.TotalPoints);
        }


        [Fact]
        public async Task GetTeamLockStatusListAsync_ReturnsCorrectLockStatus()
        {
            // Arrange
            var week = 1;
            var year = 2024;
            var now = DateTime.UtcNow;

            // Set the time provider to just before the game
            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(t => t.Now).Returns(now);

            // Create a fresh service using this mocked time
            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(_dbContext);
            var service = new CommonDataService(factoryMock.Object, timeProvider.Object);

            _dbContext.TeamSchedules.Add(new TeamSchedule
            {
                TeamScheduleId = 1,
                Week = week,
                Year = year,
                GameStartDateTime = now.AddMinutes(10),
                HomeTeamId = 1,
                AwayTeamId = 2
            });

            _dbContext.SaveChanges();

            // Act
            var result = await service.GetTeamLockStatusListAsync(week, year);

            // Assert
            Assert.Equal(2, result.Count); // Home + Away team
            Assert.All(result, t => Assert.False(t.IsLocked)); // Game hasn't started yet
        }

    }
}
