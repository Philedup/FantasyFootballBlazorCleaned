using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class WeeklyUserTeam
    {
        [Key]
        public int WeeklyUserTeamId { get; set; }
        public int PlayerId { get; set; }
        [ForeignKey(nameof(PlayerId))]
        public Player Player { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser UserProfile { get; set; }
        public int? Week { get; set; }
        public int? Year { get; set; }
        [StringLength(50)]
        public string Position { get; set; }
        public int? GameTypeId { get; set; }

    }
}
