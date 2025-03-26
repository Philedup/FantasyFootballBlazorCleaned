using FantasyFootballBlazor.Data;
using FantasyFootballBlazor.Data.Models.Entities;
using FantasyFootballBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using Xunit;
using FantasyFootball.Shared;

namespace FantasyFootballBlazor.Tests.Services;

public class UserServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
    private readonly Mock<AuthenticationStateProvider> _authStateProviderMock;

    public UserServiceTests()
    {
        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _authStateProviderMock = new Mock<AuthenticationStateProvider>();
    }

    private UserService CreateService() =>
        new UserService(_authStateProviderMock.Object,
                        new DbContextFactoryStub(_dbOptions));

    [Fact]
    public async Task ChangePasswordAsync_UpdatesPassword_WhenOldPasswordIsCorrect()
    {
        var user = new ApplicationUser
        {
            Id = "u1",
            UserName = "testuser",
            Email = "test@test.com",
            UserTeamName = "Team1",
            PaypalEmail = "test@test.com"
        };

        var hasher = new PasswordHasher<ApplicationUser>();
        user.PasswordHash = hasher.HashPassword(user, "oldpass");

        await using (var context = new ApplicationDbContext(_dbOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        var service = CreateService();
        var result = await service.ChangePasswordAsync("u1", "oldpass", "newpass");

        Assert.True(result);
    }

    [Fact]
    public async Task GetUserIdAsync_ReturnsCorrectId()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "u123")
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
        _authStateProviderMock.Setup(p => p.GetAuthenticationStateAsync())
            .ReturnsAsync(new AuthenticationState(principal));

        var service = CreateService();
        var userId = await service.GetUserIdAsync();

        Assert.Equal("u123", userId);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsUserModel_WhenAuthenticated()
    {
        var user = new ApplicationUser
        {
            Id = "u1",
            UserName = "testuser",
            Email = "test@test.com",
            UserTeamName = "Team1",
            PaypalEmail = "test@test.com",
            Paid = true,
            Survival = true
        };

        await using (var context = new ApplicationDbContext(_dbOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "u1")
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
        _authStateProviderMock.Setup(p => p.GetAuthenticationStateAsync())
            .ReturnsAsync(new AuthenticationState(principal));

        var service = CreateService();
        var model = await service.GetCurrentUserAsync();

        Assert.NotNull(model);
        Assert.Equal("Team1", model.UserTeamName);
        Assert.Equal("test@test.com", model.PaypalEmail);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsCorrectUser()
    {
        await using (var context = new ApplicationDbContext(_dbOptions))
        {
            context.Users.Add(new ApplicationUser
            {
                Id = "u1",
                UserName = "testuser",
                Email = "test@test.com",
                UserTeamName = "Team1",
                PaypalEmail = "test@test.com"
            });
            await context.SaveChangesAsync();
        }

        var service = CreateService();
        var user = await service.GetUserByEmailAsync("test@test.com");

        Assert.NotNull(user);
        Assert.Equal("testuser", user.UserName);
    }

    [Fact]
    public async Task ResetPasswordAsync_ResetsPassword_WhenTokenIsValid()
    {
        var user = new ApplicationUser
        {
            Id = "u1",
            Email = "test@test.com",
            SecurityStamp = "abc123",
            UserName = "resetuser",
            UserTeamName = "Team1",
            PaypalEmail = "test@test.com"
        };

        await using (var context = new ApplicationDbContext(_dbOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("u1:test@test.com:abc123"));
        var service = CreateService();
        var result = await service.ResetPasswordAsync("test@test.com", token, "newpass");

        Assert.True(result);
    }

    [Fact]
    public async Task GeneratePasswordResetTokenAsync_ReturnsEncodedToken()
    {
        var user = new ApplicationUser
        {
            Id = "u1",
            Email = "test@test.com",
            SecurityStamp = "abc123",
            UserName = "resetuser",
            UserTeamName = "Team1",
            PaypalEmail = "test@test.com"
        };

        await using (var context = new ApplicationDbContext(_dbOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        var service = CreateService();
        var token = await service.GeneratePasswordResetTokenAsync("test@test.com");

        Assert.NotNull(token);
        var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        Assert.Equal("u1:test@test.com:abc123", decoded);
    }

    [Fact]
    public async Task UpdateUserProfileAsync_UpdatesFields()
    {
        var user = new ApplicationUser
        {
            Id = "u1",
            UserName = "olduser",
            Email = "old@test.com",
            UserTeamName = "Team1",
            PaypalEmail = "test@test.com"
        };

        await using (var context = new ApplicationDbContext(_dbOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "u1")
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
        _authStateProviderMock.Setup(p => p.GetAuthenticationStateAsync())
            .ReturnsAsync(new AuthenticationState(principal));

        var model = new EditProfileModel
        {
            UserName = "newuser",
            Email = "new@test.com",
            PaypalEmail = "new@test.com",
            UserTeamName = "New Team"
        };

        var service = CreateService();
        var result = await service.UpdateUserProfileAsync(model);

        Assert.True(result);

        await using var verifyContext = new ApplicationDbContext(_dbOptions);
        var updated = await verifyContext.Users.FirstOrDefaultAsync(u => u.Id == "u1");

        Assert.Equal("newuser", updated.UserName);
        Assert.Equal("new@test.com", updated.Email);
        Assert.Equal("New Team", updated.UserTeamName);
    }

    [Fact]
    public async Task IsUsernameUniqueAsync_ReturnsFalse_WhenTaken()
    {
        await using (var context = new ApplicationDbContext(_dbOptions))
        {
            context.Users.Add(new ApplicationUser
            {
                Id = "u2",
                UserName = "dupe",
                UserTeamName = "Team1",
                PaypalEmail = "test@test.com"
            });
            await context.SaveChangesAsync();
        }

        var service = CreateService();
        var result = await service.IsUsernameUniqueAsync("dupe", "u1");
        Assert.False(result);
    }

    [Fact]
    public async Task IsEmailUniqueAsync_ReturnsFalse_WhenTaken()
    {
        await using (var context = new ApplicationDbContext(_dbOptions))
        {
            context.Users.Add(new ApplicationUser
            {
                Id = "u2",
                Email = "dupe@test.com",
                UserTeamName = "Team1",
                PaypalEmail = "test@test.com"
            });
            await context.SaveChangesAsync();
        }

        var service = CreateService();
        var result = await service.IsEmailUniqueAsync("dupe@test.com", "u1");
        Assert.False(result);
    }
}

// Helper for IDbContextFactory stub
public class DbContextFactoryStub : IDbContextFactory<ApplicationDbContext>
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public DbContextFactoryStub(DbContextOptions<ApplicationDbContext> options)
    {
        _options = options;
    }

    public ApplicationDbContext CreateDbContext() => new ApplicationDbContext(_options);
}
