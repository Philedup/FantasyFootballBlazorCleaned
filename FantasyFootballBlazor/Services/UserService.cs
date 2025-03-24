using FantasyFootball.Shared;
using FantasyFootball.Shared.Services;
using FantasyFootballBlazor.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace FantasyFootballBlazor.Services
{
    public class UserService : IUserService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public UserService(AuthenticationStateProvider authenticationStateProvider, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Changes the password for a given user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="oldPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password the user wants to set.</param>
        /// <returns>Returns <c>true</c> if the password change was successful; otherwise, <c>false</c>.</returns>
        public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Fetch the user
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                Console.WriteLine("❌ User not found.");
                return false;
            }

            // Verify old password
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                Console.WriteLine("❌ Incorrect old password.");
                return false;
            }

            // Hash and update new password
            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
            context.Users.Update(user);

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Password changed successfully.");

            return true;
        }

        /// <summary>
        /// Retrieves the current authenticated user's unique identifier.
        /// </summary>
        /// <returns>User ID as a string if authenticated; otherwise, null.</returns>
        public async Task<string> GetUserIdAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated != true)
            {
                Console.WriteLine("❌ User is NOT authenticated.");
                return null;
            }

            Console.WriteLine($"✅ User is authenticated: {user.Identity.Name}");

            return user.FindFirst("sub")?.Value ??
                   user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Retrieves the details of the currently authenticated user.
        /// </summary>
        /// <returns>UserModel with user details if found; otherwise, null.</returns>
        public async Task<UserModel> GetCurrentUserAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated != true)
            {
                Console.WriteLine("❌ User is NOT authenticated.");
                return null;
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                Console.WriteLine("⚠️ User ID not found in claims.");
                return null;
            }

            await using var context = _dbContextFactory.CreateDbContext();

            var appUser = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (appUser == null)
            {
                Console.WriteLine($"⚠️ No user found with ID: {userId}");
                return null;
            }

            return new UserModel
            {
                UserId = appUser.Id,
                UserName = appUser.UserName,
                Admin = appUser.Admin,
                Email = appUser.Email,
                Paid = appUser.Paid,
                PaypalEmail = appUser.PaypalEmail,
                Survival = appUser.Survival,
                UserTeamName = appUser.UserTeamName
            };
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email of the user to find.</param>
        /// <returns>UserModel if a matching user is found; otherwise, null.</returns>
        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            var appUser = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (appUser == null)
            {
                Console.WriteLine($"⚠️ No user found with email: {email}");
                return null;
            }

            return new UserModel
            {
                UserId = appUser.Id,
                UserName = appUser.UserName,
                Admin = appUser.Admin,
                Email = appUser.Email,
                Paid = appUser.Paid,
                PaypalEmail = appUser.PaypalEmail,
                Survival = appUser.Survival,
                UserTeamName = appUser.UserTeamName
            };
        }

        /// <summary>
        /// Resets a user's password using a dynamically validated token.
        /// </summary>
        /// <param name="email">The email of the user resetting the password.</param>
        /// <param name="resetToken">The provided reset token.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>True if password reset was successful, otherwise false.</returns>
        public async Task<bool> ResetPasswordAsync(string email, string resetToken, string newPassword)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Fetch the user from the database
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                Console.WriteLine($"⚠️ ResetPasswordAsync: No user found with email {email}.");
                return false;
            }

            try
            {
                // Decode and reconstruct the expected token
                var decodedBytes = WebEncoders.Base64UrlDecode(resetToken);
                var decodedToken = Encoding.UTF8.GetString(decodedBytes);
                var expectedToken = $"{user.Id}:{user.Email}:{user.SecurityStamp}";

                // Validate the token dynamically
                if (decodedToken != expectedToken)
                {
                    Console.WriteLine($"❌ ResetPasswordAsync: Invalid reset token for {email}.");
                    return false;
                }

                // Hash the new password
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, newPassword);

                // Save changes to the database
                context.Users.Update(user);
                await context.SaveChangesAsync();

                Console.WriteLine($"✅ Password reset successful for user {email}.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ResetPasswordAsync: Error while processing reset for {email}. {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Generates a password reset token without storing it in the database.
        /// </summary>
        /// <param name="email">The email of the user requesting the password reset.</param>
        /// <returns>A base64-encoded reset token if successful; otherwise, null.</returns>
        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            // Fetch the user from the database
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                Console.WriteLine($"⚠️ GeneratePasswordResetTokenAsync: No user found with email {email}.");
                return null; // Prevent revealing if an email exists
            }

            // Create a token using the user's unique details
            var rawToken = $"{user.Id}:{user.Email}:{user.SecurityStamp}";
            var tokenBytes = Encoding.UTF8.GetBytes(rawToken);

            // Convert to a Base64 string for safe transmission
            var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);

            Console.WriteLine($"✅ Generated reset token for {email}");
            return encodedToken;
        }

        /// <summary>
        /// Updates the profile information of the currently authenticated user.
        /// </summary>
        /// <param name="model">The updated profile information.</param>
        /// <returns>True if update was successful; otherwise, false.</returns>
        public async Task<bool> UpdateUserProfileAsync(EditProfileModel model)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated != true)
            {
                Console.WriteLine("❌ User is NOT authenticated.");
                return false;
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                Console.WriteLine("⚠️ User ID not found in claims.");
                return false;
            }

            await using var context = _dbContextFactory.CreateDbContext();

            var appUser = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (appUser == null)
            {
                Console.WriteLine($"⚠️ No user found with ID: {userId}");
                return false;
            }

            appUser.UserName = model.UserName;
            appUser.Email = model.Email;
            appUser.UserTeamName = model.UserTeamName;
            appUser.PaypalEmail = model.PaypalEmail;
            appUser.NormalizedUserName = model.UserName.ToUpper();
            appUser.NormalizedEmail = model.Email.ToUpper();

            context.Users.Update(appUser);
            await context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Checks if a username is unique among all users except the one being updated.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <param name="userId">The ID of the current user (to exclude from check).</param>
        /// <returns>True if username is unique; otherwise, false.</returns>
        public async Task<bool> IsUsernameUniqueAsync(string username, string userId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            return !await context.Users
                .AsNoTracking()
                .AnyAsync(u => u.UserName == username && u.Id != userId);
        }

        /// <summary>
        /// Checks if an email is unique among all users except the one being updated.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <param name="userId">The ID of the current user (to exclude from check).</param>
        /// <returns>True if email is unique; otherwise, false.</returns>
        public async Task<bool> IsEmailUniqueAsync(string email, string userId)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            return !await context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email && u.Id != userId);
        }
    }
}
