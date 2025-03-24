using FantasyFootballBlazor.Data.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FantasyFootballBlazor.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [StringLength(50)]
        public string UserTeamName { get; set; }
        public bool? Paid { get; set; }
        public bool? Survival { get; set; }
        [StringLength(256)]
        public string PaypalEmail { get; set; }
        public bool? Admin { get; set; }
        public ICollection<WeeklyUserTeam> UserPicks { get; set; }
    }

}
