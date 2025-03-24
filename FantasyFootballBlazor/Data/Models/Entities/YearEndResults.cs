using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    [Table("YearEndResults")]
    public class YearEndResults
    {
        [Key]
        public int YearEndResultId { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser UserProfile { get; set; }
        public int Year { get; set; }
        public int GameTypeId { get; set; }
        [ForeignKey(nameof(GameTypeId))]
        public virtual GameType GameType { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalEarned { get; set; }
        public int TotalPoints { get; set; }
        public int Rank { get; set; }
    }
}