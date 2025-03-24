using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerId { get; set; }
        [StringLength(50)]
        public string FullName { get; set; }
        [StringLength(50)]
        public string First { get; set; }
        [StringLength(50)]
        public string Last { get; set; }
        [StringLength(50)]
        public string Position { get; set; }
        public int? TeamId { get; set; }
        [StringLength(1000)]
        public string PlayerPictureImageUrl { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }
        public virtual ICollection<WeeklyStat> WeeklyStats { get; set; }
        public virtual ICollection<WeeklyUserTeam> WeeklyUserTeams { get; set; }

    }
}
