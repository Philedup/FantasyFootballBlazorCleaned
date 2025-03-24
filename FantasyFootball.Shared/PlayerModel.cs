using System.ComponentModel.DataAnnotations;

namespace FantasyFootball.Shared
{
    public class PlayerModel
    {
        [StringLength(50)]
        public string FullName { get; set; }
        [StringLength(50)]
        public string First { get; set; }
        [StringLength(50)]
        public string Last { get; set; }
        [StringLength(50)]
        public string Position { get; set; }
        [StringLength(50)]
        public int PlayerId { get; set; }
        public int? TeamId { get; set; }
        [StringLength(1000)]
        public string PlayerPictureImageUrl { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }
        public int TotalPoints { get; set; }
        public bool Locked { get; set; }
        public List<WeeklyStatModel> PlayerStats { get; set; }
        public string Opponent { get; set; }
    }
}
