using System.ComponentModel.DataAnnotations;


namespace FantasyFootball.Shared
{
    public class EditProfileModel
    {
        [Required(ErrorMessage = "User name is required."), StringLength(50)]
        [MinLength(6, ErrorMessage = "Username must be at least 6 characters.")]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Team name is required.")]
        public string UserTeamName { get; set; } = "";

        [Required]
        [StringLength(256)]
        public string PaypalEmail { get; set; } = "";
    }
}