namespace FantasyFootball.Shared.Services
{
    public interface IUserService
    {
        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<string> GetUserIdAsync();
        Task<UserModel> GetCurrentUserAsync();
        Task<bool> UpdateUserProfileAsync(EditProfileModel model);
        Task<bool> IsUsernameUniqueAsync(string username, string userId);
        Task<bool> IsEmailUniqueAsync(string email, string userId);
        Task<UserModel> GetUserByEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string userId, string resetToken, string newPassword);
        Task<string?> GeneratePasswordResetTokenAsync(string userId);
    }
}
