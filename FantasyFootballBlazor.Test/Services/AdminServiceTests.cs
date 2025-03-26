using Moq;
using Microsoft.EntityFrameworkCore;
using FantasyFootballBlazor.Data;
using FantasyFootballBlazor.Services;
using FantasyFootball.Shared.Services;
using FantasyFootball.Shared;
using FantasyFootballBlazor.Tests.Helpers;
using FantasyFootballBlazor.Data.Models.Entities;

namespace FantasyFootballBlazor.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ICommonDataService> _commonDataServiceMock;
        private readonly MockNavigationManager _mockNavigationManager;
        private readonly AdminService _service;

        public AdminServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);

            var dbFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            dbFactoryMock.Setup(f => f.CreateDbContext()).Returns(_dbContext);

            _userServiceMock = new Mock<IUserService>();
            _commonDataServiceMock = new Mock<ICommonDataService>();
            _mockNavigationManager = new MockNavigationManager(); // Custom helper

            _service = new AdminService(
                dbFactoryMock.Object,
                _commonDataServiceMock.Object,
                _userServiceMock.Object,
                _mockNavigationManager
            );
        }

        [Fact]
        public async Task EnsureAdminAccessAsync_WhenNotAdmin_RedirectsAndReturnsFalse()
        {
            // Arrange: Simulate a non-admin user
            _userServiceMock.Setup(u => u.GetCurrentUserAsync())
                .ReturnsAsync(new UserModel { Admin = false });

            // Act
            var result = await _service.EnsureAdminAccessAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("/", _mockNavigationManager.NavigatedTo);
            Assert.True(_mockNavigationManager.ForceLoad);
        }

        [Fact]
        public async Task EnsureAdminAccessAsync_WhenAdmin_ReturnsTrue()
        {
            // Arrange: Simulate an admin user
            _userServiceMock.Setup(u => u.GetCurrentUserAsync())
                .ReturnsAsync(new UserModel { Admin = true });

            // Act
            var result = await _service.EnsureAdminAccessAsync();

            // Assert
            Assert.True(result);
            Assert.Null(_mockNavigationManager.NavigatedTo); // No redirect expected
        }

        [Fact]
        public async Task EnsureAdminAccessAsync_WhenNullUser_RedirectsAndReturnsFalse()
        {
            // Arrange: Simulate no user
            _userServiceMock.Setup(u => u.GetCurrentUserAsync())
                .ReturnsAsync((UserModel?)null);

            // Act
            var result = await _service.EnsureAdminAccessAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("/", _mockNavigationManager.NavigatedTo);
            Assert.True(_mockNavigationManager.ForceLoad);
        }

        [Fact]
        public async Task SaveYearlyUserDataAsync_SavesNewRecord()
        {
            // Arrange: define a shared in-memory DB name
            var dbName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            // Create factory that always returns a fresh context using the same in-memory DB
            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(() => new ApplicationDbContext(options));

            var userServiceMock = new Mock<IUserService>();
            var commonDataServiceMock = new Mock<ICommonDataService>();
            var navManager = new MockNavigationManager();

            var service = new AdminService(factoryMock.Object, commonDataServiceMock.Object, userServiceMock.Object, navManager);

            // Create test data
            var data = new YearlyUserDataViewModel
            {
                AddPlayer = new AddYearlyPlayerRankingModel
                {
                    Rank = 1,
                    Year = 2025,
                    GameTypeId = 1,
                    TotalEarned = 100,
                    TotalPoints = 200,
                    UserId = "test-user"
                }
            };

            // Act: save the data
            var result = await service.SaveYearlyUserDataAsync(data);

            // Assert: use a fresh context to validate result
            using var verifyContext = new ApplicationDbContext(options);
            var record = await verifyContext.YearEndResults.FirstOrDefaultAsync();

            Assert.True(result);
            Assert.NotNull(record);
            Assert.Equal("test-user", record.UserId);
            Assert.Equal(1, record.GameTypeId);
            Assert.Equal(100, record.TotalEarned);
            Assert.Equal(200, record.TotalPoints);
        }

        [Fact]
        public async Task DeleteYearlyUserDataAsync_DeletesExistingRecord()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString(); // Use same DB for both contexts

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            // Seed initial record in a seeding context
            using (var seedContext = new ApplicationDbContext(options))
            {
                seedContext.YearEndResults.Add(new YearEndResults
                {
                    Rank = 1,
                    Year = 2025,
                    GameTypeId = 2,
                    UserId = "delete-user",
                    TotalEarned = 50,
                    TotalPoints = 100
                });

                await seedContext.SaveChangesAsync();
            }

            // Create a factory mock that always returns a new context with the same DB
            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            factoryMock.Setup(f => f.CreateDbContext()).Returns(() => new ApplicationDbContext(options));

            // Create required mocks
            var userServiceMock = new Mock<IUserService>();
            var commonDataServiceMock = new Mock<ICommonDataService>();
            var navManager = new MockNavigationManager();

            // Create the service
            var service = new AdminService(factoryMock.Object, commonDataServiceMock.Object, userServiceMock.Object, navManager);

            // Act
            var result = await service.DeleteYearlyUserDataAsync("delete-user", 2025, 2);

            // Assert with a new context
            using var verifyContext = new ApplicationDbContext(options);
            var deleted = await verifyContext.YearEndResults.FirstOrDefaultAsync(x => x.UserId == "delete-user");

            Assert.True(result);
            Assert.Null(deleted); // Record should be deleted
        }


        [Fact]
        public async Task GetUserListViewModelAsync_ReturnsUsers()
        {
            // Arrange: add a sample user to the database
            _dbContext.ApplicationUsers.Add(new ApplicationUser
            {
                Id = "u1",
                UserName = "testuser",
                Email = "test@example.com",
                Paid = true,
                PaypalEmail = "paypal@example.com",
                Survival = true,
                UserTeamName = "The Champs"
            });

            await _dbContext.SaveChangesAsync();

            // Act: retrieve the user list view model
            var result = await _service.GetUserListViewModelAsync();

            // Assert: validate that the expected user is returned
            Assert.Single(result.Users);
            var user = result.Users.First();
            Assert.Equal("testuser", user.UserName);
            Assert.Equal("The Champs", user.UserTeamName);
            Assert.True(user.Paid);
        }

        [Fact]
        public void CreateUpdateWeeklyDataViewModel_Returns18Weeks()
        {
            // Act: create the weekly data view model
            var result = _service.CreateUpdateWeeklyDataViewModel();

            // Assert: ensure 18 weeks are returned with correct values
            Assert.Equal(18, result.Weeks.Count);
            Assert.Contains(result.Weeks, w => w.Text == "1" && w.Value == "1");
            Assert.Contains(result.Weeks, w => w.Text == "18" && w.Value == "18");
        }

    }
}
