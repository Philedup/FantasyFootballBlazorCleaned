using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasyFootballBlazor.Data.Models.Entities
{
    public class WeeklyWinner
    {
        [Key]
        public int WeeklyWinnerId { get; set; }
        public int Week { get; set; }
        public int Year { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser UserProfile { get; set; }
        public int? Place { get; set; }
    }
}
